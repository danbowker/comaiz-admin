import React, { useState, useEffect } from 'react';
import { tasksService } from '../../services/entityService';
import { Task, Contract, ContractRate, TaskContractRate } from '../../types';
import './EntityForm.css';

interface TaskFormProps {
  task: Task | null;
  contracts: Contract[];
  contractRates: ContractRate[];
  onClose: () => void;
  onSave: () => void;
  defaultContractId?: number;
}

const TaskForm: React.FC<TaskFormProps> = ({
  task,
  contracts,
  contractRates,
  onClose,
  onSave,
  defaultContractId,
}) => {
  const [formData, setFormData] = useState<Partial<Task>>({
    name: '',
    contractId: defaultContractId,
    taskContractRates: [],
  });
  const [selectedContractRateId, setSelectedContractRateId] = useState<number | ''>('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Filter contract rates based on selected contract
  const filteredContractRates = formData.contractId
    ? contractRates.filter(cr => cr.contractId === formData.contractId)
    : contractRates;

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

  const handleAddContractRate = () => {
    if (!selectedContractRateId) return;

    const contractRateId = Number(selectedContractRateId);
    
    // Check if already added
    const alreadyAdded = formData.taskContractRates?.some(
      tcr => tcr.contractRateId === contractRateId
    );
    
    if (alreadyAdded) {
      setError('This contract rate has already been added');
      return;
    }

    const newTaskContractRate: TaskContractRate = {
      contractRateId,
      contractRate: contractRates.find(cr => cr.id === contractRateId),
    };

    setFormData(prev => ({
      ...prev,
      taskContractRates: [...(prev.taskContractRates || []), newTaskContractRate],
    }));
    
    setSelectedContractRateId('');
    setError('');
  };

  const handleRemoveContractRate = (contractRateId: number) => {
    setFormData(prev => ({
      ...prev,
      taskContractRates: prev.taskContractRates?.filter(
        tcr => tcr.contractRateId !== contractRateId
      ),
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      if (formData.id) {
        await tasksService.update(formData as Task);
      } else {
        await tasksService.create(formData as Omit<Task, 'id'>);
      }
      onSave();
      onClose();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to save task');
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
              <select
                id="contractId"
                value={formData.contractId || ''}
                onChange={(e) =>
                  handleChange('contractId', e.target.value === '' ? null : Number(e.target.value))
                }
                disabled={loading}
              >
                <option value="">Select...</option>
                {contracts.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.description || `Contract ${c.id}`}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-field">
              <label>Contract Rates</label>
              <div style={{ marginBottom: '10px' }}>
                {formData.taskContractRates && formData.taskContractRates.length > 0 ? (
                  <ul style={{ listStyle: 'none', padding: 0 }}>
                    {formData.taskContractRates.map((tcr) => {
                      const rate = tcr.contractRate || contractRates.find(cr => cr.id === tcr.contractRateId);
                      return (
                        <li
                          key={tcr.contractRateId}
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
                          <span>{rate?.description || `Rate ${tcr.contractRateId}`}</span>
                          <button
                            type="button"
                            onClick={() => handleRemoveContractRate(tcr.contractRateId)}
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
                  <p style={{ color: '#666', fontSize: '14px' }}>No contract rates added</p>
                )}
              </div>
              
              <div style={{ display: 'flex', gap: '8px' }}>
                <select
                  value={selectedContractRateId}
                  onChange={(e) => setSelectedContractRateId(e.target.value === '' ? '' : Number(e.target.value))}
                  disabled={loading || !formData.contractId}
                  style={{ flex: 1 }}
                >
                  <option value="">Select contract rate...</option>
                  {filteredContractRates.map((cr) => (
                    <option key={cr.id} value={cr.id}>
                      {cr.description}
                      {cr.applicationUser && ` (${cr.applicationUser.userName})`}
                    </option>
                  ))}
                </select>
                <button
                  type="button"
                  onClick={handleAddContractRate}
                  disabled={loading || !selectedContractRateId || !formData.contractId}
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
