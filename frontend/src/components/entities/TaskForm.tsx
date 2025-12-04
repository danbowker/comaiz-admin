import React, { useState, useEffect, useMemo } from 'react';
import { tasksService } from '../../services/entityService';
import { Task, Contract, ContractRate, UserContractRate, TaskContractRate, RecordState } from '../../types';
import { isContractActive } from '../../utils/stateFilter';
import { useShowCompleteContracts } from '../../hooks/useFilterPreferences';
import './EntityForm.css';

interface TaskFormProps {
  task: Task | null;
  contracts: Contract[];
  contractRates: ContractRate[];
  userContractRates: UserContractRate[];
  onClose: () => void;
  onSave: () => void;
  defaultContractId?: number;
}

const TaskForm: React.FC<TaskFormProps> = ({
  task,
  contracts,
  contractRates,
  userContractRates,
  onClose,
  onSave,
  defaultContractId,
}) => {
  const [formData, setFormData] = useState<Partial<Task>>({
    name: '',
    contractId: defaultContractId,
    taskContractRates: [],
  });
  const [selectedUserContractRateId, setSelectedUserContractRateId] = useState<number | ''>('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const { showComplete, toggleShowComplete } = useShowCompleteContracts();

  // Filter contracts based on showComplete preference
  const availableContracts = useMemo(() => {
    if (showComplete) {
      return contracts;
    }
    return contracts.filter(isContractActive);
  }, [contracts, showComplete]);

  // Check if the selected contract is complete (for validation)
  const selectedContract = formData.contractId
    ? contracts.find((c) => c.id === formData.contractId)
    : null;
  const isSelectedContractComplete = selectedContract?.state === RecordState.Complete;

  // Filter user contract rates based on selected contract
  const filteredUserContractRates = formData.contractId
    ? userContractRates.filter(ucr => {
        const contractRate = contractRates.find(cr => cr.id === ucr.contractRateId);
        return contractRate?.contractId === formData.contractId;
      })
    : userContractRates;

  useEffect(() => {
    if (task) {
      setFormData({
        ...task,
        taskContractRates: task.taskContractRates || [],
      });
    } else {
      setFormData({
        name: '',
        contractId: defaultContractId,
        taskContractRates: [],
      });
    }
  }, [task, defaultContractId]);

  const handleChange = (name: keyof Task, value: any) => {
    setFormData((prev) => {
      const updated = { ...prev, [name]: value };
      // Clear task contract rates if contract changes
      if (name === 'contractId') {
        updated.taskContractRates = [];
      }
      return updated;
    });
  };

  const handleAddUserContractRate = () => {
    if (!selectedUserContractRateId) return;

    const userContractRateId = Number(selectedUserContractRateId);
    
    // Check if already added
    const alreadyAdded = formData.taskContractRates?.some(
      tcr => tcr.userContractRateId === userContractRateId
    );
    
    if (alreadyAdded) {
      setError('This user/rate combination has already been added');
      return;
    }

    const newTaskContractRate: TaskContractRate = {
      userContractRateId,
      userContractRate: userContractRates.find(ucr => ucr.id === userContractRateId),
    };

    setFormData(prev => ({
      ...prev,
      taskContractRates: [...(prev.taskContractRates || []), newTaskContractRate],
    }));
    
    setSelectedUserContractRateId('');
    setError('');
  };

  const handleRemoveUserContractRate = (userContractRateId: number) => {
    setFormData(prev => ({
      ...prev,
      taskContractRates: prev.taskContractRates?.filter(
        tcr => tcr.userContractRateId !== userContractRateId
      ),
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    // Validate: don't allow creating new tasks on complete contracts
    if (!task && isSelectedContractComplete) {
      setError('Cannot add tasks to a complete contract. Please select an active contract.');
      setLoading(false);
      return;
    }

    try {
      if (formData.id) {
        await tasksService.update(formData as Task);
      } else {
        await tasksService.create(formData as Omit<Task, 'id'>);
      }
      onSave();
      onClose();
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to save task');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{task ? 'Edit Task' : 'Create Task'}</h2>
          <button className="close-btn" onClick={onClose}>
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-body">
            {error && <div className="error-message">{error}</div>}

            <div className="form-field">
              <label htmlFor="name">
                Name<span className="required">*</span>
              </label>
              <input
                type="text"
                id="name"
                value={formData.name || ''}
                onChange={(e) => handleChange('name', e.target.value)}
                required
                disabled={loading}
              />
            </div>

            <div className="form-field">
              <label htmlFor="contractId">Contract</label>
              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                <select
                  id="contractId"
                  value={formData.contractId || ''}
                  onChange={(e) =>
                    handleChange('contractId', e.target.value === '' ? null : Number(e.target.value))
                  }
                  disabled={loading}
                >
                  <option value="">Select...</option>
                  {availableContracts.map((c) => (
                    <option 
                      key={c.id} 
                      value={c.id}
                      disabled={!task && c.state === RecordState.Complete}
                    >
                      {c.description || `Contract ${c.id}`}
                      {c.state === RecordState.Complete ? ' (Complete)' : ''}
                    </option>
                  ))}
                </select>
                <label style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px', color: '#666', cursor: 'pointer' }}>
                  <input
                    type="checkbox"
                    checked={showComplete}
                    onChange={toggleShowComplete}
                  />
                  Show complete contracts
                </label>
                {isSelectedContractComplete && !task && (
                  <div style={{ color: '#dc3545', fontSize: '13px' }}>
                    ⚠ Cannot add new tasks to a complete contract
                  </div>
                )}
              </div>
            </div>

            <div className="form-field">
              <label>User/Rate Assignments</label>
              <div style={{ marginBottom: '10px' }}>
                {formData.taskContractRates && formData.taskContractRates.length > 0 ? (
                  <ul style={{ listStyle: 'none', padding: 0 }}>
                    {formData.taskContractRates.map((tcr) => {
                      const ucr = tcr.userContractRate || userContractRates.find(u => u.id === tcr.userContractRateId);
                      const rate = ucr?.contractRate || contractRates.find(cr => cr.id === ucr?.contractRateId);
                      const userName = ucr?.applicationUser?.userName || ucr?.applicationUser?.email || 'Unknown User';
                      return (
                        <li
                          key={tcr.userContractRateId}
                          style={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            padding: '8px',
                            marginBottom: '4px',
                            backgroundColor: '#f5f5f5',
                            borderRadius: '4px',
                          }}
                        >
                          <span>{userName} - {rate?.description || `Rate ${ucr?.contractRateId}`}</span>
                          <button
                            type="button"
                            onClick={() => handleRemoveUserContractRate(tcr.userContractRateId)}
                            style={{
                              padding: '4px 8px',
                              backgroundColor: '#dc3545',
                              color: 'white',
                              border: 'none',
                              borderRadius: '4px',
                              cursor: 'pointer',
                            }}
                            disabled={loading}
                          >
                            Remove
                          </button>
                        </li>
                      );
                    })}
                  </ul>
                ) : (
                  <p style={{ color: '#666', fontSize: '14px' }}>No user/rate assignments added</p>
                )}
              </div>
              
              <div style={{ display: 'flex', gap: '8px' }}>
                <select
                  value={selectedUserContractRateId}
                  onChange={(e) => setSelectedUserContractRateId(e.target.value === '' ? '' : Number(e.target.value))}
                  disabled={loading || !formData.contractId}
                  style={{ flex: 1 }}
                >
                  <option value="">Select user and rate...</option>
                  {filteredUserContractRates.map((ucr) => {
                    const rate = contractRates.find(cr => cr.id === ucr.contractRateId);
                    const userName = ucr.applicationUser?.userName || ucr.applicationUser?.email || 'Unknown User';
                    return (
                      <option key={ucr.id} value={ucr.id}>
                        {userName} - {rate?.description || `Rate ${ucr.contractRateId}`}
                      </option>
                    );
                  })}
                </select>
                <button
                  type="button"
                  onClick={handleAddUserContractRate}
                  disabled={loading || !selectedUserContractRateId || !formData.contractId}
                  style={{
                    padding: '8px 16px',
                    backgroundColor: '#007bff',
                    color: 'white',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer',
                  }}
                >
                  Add
                </button>
              </div>
            </div>
          </div>

          <div className="form-footer">
            <button
              type="button"
              className="btn-cancel"
              onClick={onClose}
              disabled={loading}
            >
              Cancel
            </button>
            <button type="submit" className="btn-submit" disabled={loading}>
              {loading ? 'Saving...' : 'Save'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default TaskForm;
