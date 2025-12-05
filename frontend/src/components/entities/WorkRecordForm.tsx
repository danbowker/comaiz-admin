import React, { useState, useEffect, useMemo } from 'react';
import { workRecordsService, tasksService, usersService, contractsService } from '../../services/entityService';
import { WorkRecord, Task, ApplicationUser, Contract, RecordState } from '../../types';
import { useShowCompleteTasks } from '../../hooks/useFilterPreferences';
import { isTaskEffectivelyActive } from '../../utils/stateFilter';
import { useAuth } from '../../contexts/AuthContext';
import { useContractSelection } from '../../contexts/ContractSelectionContext';
import './EntityForm.css';

interface WorkRecordFormProps {
  workRecord: WorkRecord | null;
  onClose: () => void;
  onSave: () => void;
}

const WorkRecordForm: React.FC<WorkRecordFormProps> = ({
  workRecord,
  onClose,
  onSave,
}) => {
  const [formData, setFormData] = useState<Partial<WorkRecord>>({
    taskId: undefined,
    applicationUserId: undefined,
    startDate: '',
    endDate: '',
    hours: 0,
  });
  const [tasks, setTasks] = useState<Task[]>([]);
  const [users, setUsers] = useState<ApplicationUser[]>([]);
  const [contracts, setContracts] = useState<Contract[]>([]);
  const [loading, setLoading] = useState(false);
  const [dataLoading, setDataLoading] = useState(true);
  const [error, setError] = useState('');
  const { showComplete, toggleShowComplete } = useShowCompleteTasks();
  const { user } = useAuth();
  const { selectedContractId } = useContractSelection();

  // Load related data
  useEffect(() => {
    const loadData = async () => {
      try {
        setDataLoading(true);
        const filterParams = selectedContractId ? { contractId: selectedContractId } : undefined;
        const [tasksData, usersData, contractsData] = await Promise.all([
          tasksService.getAll(filterParams),
          usersService.getAll(),
          contractsService.getAll(),
        ]);
        setTasks(tasksData);
        setUsers(usersData);
        setContracts(contractsData);
      } catch (err) {
        console.error('Failed to load data', err);
        setError('Failed to load form data');
      } finally {
        setDataLoading(false);
      }
    };
    loadData();
  }, [selectedContractId]);

  // Initialize form when data is loaded
  useEffect(() => {
    if (dataLoading) return;

    if (workRecord) {
      setFormData({
        id: workRecord.id,
        taskId: workRecord.taskId,
        applicationUserId: workRecord.applicationUserId,
        startDate: workRecord.startDate,
        endDate: workRecord.endDate,
        hours: workRecord.hours,
        invoiceItemId: workRecord.invoiceItemId,
      });
    } else {
      // Default to current user for new records
      const currentUser = users.find(u => u.userName === user?.username || u.email === user?.email);
      setFormData({
        applicationUserId: currentUser?.id,
        startDate: '',
        endDate: '',
        hours: 0,
      });
    }
  }, [workRecord, users, user, dataLoading]);

  // Filter tasks based on effective active state
  const availableTasks = useMemo(() => {
    if (showComplete) {
      return tasks;
    }
    return tasks.filter((task) => isTaskEffectivelyActive(task, contracts));
  }, [tasks, contracts, showComplete]);

  // Check if selected task is effectively active
  const selectedTask = formData.taskId
    ? tasks.find((t) => t.id === formData.taskId)
    : null;
  const isSelectedTaskEffectivelyActive = selectedTask
    ? isTaskEffectivelyActive(selectedTask, contracts)
    : true;

  // Whether to disable inactive task options and show warnings (only for new records)
  const isCreatingNewRecord = !workRecord;
  const shouldWarnAboutInactiveTask = isCreatingNewRecord && formData.taskId && !isSelectedTaskEffectivelyActive;

  const handleChange = (name: keyof WorkRecord, value: any) => {
    setFormData((prev) => ({ ...prev, [name]: value }));
    setError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    // Validation: don't allow adding work to inactive tasks (for new records)
    if (isCreatingNewRecord && formData.taskId && !isSelectedTaskEffectivelyActive) {
      setError('Cannot add work records to an inactive task. Please select an active task.');
      setLoading(false);
      return;
    }

    try {
      if (formData.id) {
        await workRecordsService.update(formData as WorkRecord);
      } else {
        await workRecordsService.create(formData as Omit<WorkRecord, 'id'>);
      }
      onSave();
      onClose();
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to save work record');
    } finally {
      setLoading(false);
    }
  };

  if (dataLoading) {
    return (
      <div className="modal-overlay">
        <div className="modal-content">
          <div className="modal-header">
            <h2>Loading...</h2>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{workRecord ? 'Edit Work Record' : 'Create Work Record'}</h2>
          <button className="close-btn" onClick={onClose}>
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-body">
            {error && <div className="error-message">{error}</div>}

            <div className="form-field">
              <label htmlFor="taskId">Task</label>
              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                <select
                  id="taskId"
                  value={formData.taskId || ''}
                  onChange={(e) =>
                    handleChange('taskId', e.target.value === '' ? undefined : Number(e.target.value))
                  }
                  disabled={loading}
                >
                  <option value="">Select...</option>
                  {availableTasks.map((t) => {
                    const effectivelyActive = isTaskEffectivelyActive(t, contracts);
                    let label = t.name;
                    if (!effectivelyActive) {
                      if (t.state === RecordState.Active) {
                        label += ' (Contract Complete)';
                      } else {
                        label += ' (Complete)';
                      }
                    }
                    return (
                      <option
                        key={t.id}
                        value={t.id}
                        disabled={isCreatingNewRecord && !effectivelyActive}
                      >
                        {label}
                      </option>
                    );
                  })}
                </select>
                <label style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px', color: '#666', cursor: 'pointer' }}>
                  <input
                    type="checkbox"
                    checked={showComplete}
                    onChange={toggleShowComplete}
                  />
                  Show complete tasks
                </label>
                {shouldWarnAboutInactiveTask && (
                  <div style={{ color: '#dc3545', fontSize: '13px' }}>
                    ⚠ Cannot add work records to an inactive task
                  </div>
                )}
              </div>
            </div>

            <div className="form-field">
              <label htmlFor="applicationUserId">User</label>
              <select
                id="applicationUserId"
                value={formData.applicationUserId || ''}
                onChange={(e) =>
                  handleChange('applicationUserId', e.target.value === '' ? undefined : e.target.value)
                }
                disabled={loading}
              >
                <option value="">Select...</option>
                {users.map((u) => (
                  <option key={u.id} value={u.id}>
                    {u.userName || u.email || `User ${u.id}`}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-field">
              <label htmlFor="startDate">
                Start Date<span className="required">*</span>
              </label>
              <input
                type="date"
                id="startDate"
                value={formData.startDate || ''}
                onChange={(e) => handleChange('startDate', e.target.value)}
                required
                disabled={loading}
              />
            </div>

            <div className="form-field">
              <label htmlFor="endDate">
                End Date<span className="required">*</span>
              </label>
              <input
                type="date"
                id="endDate"
                value={formData.endDate || ''}
                onChange={(e) => handleChange('endDate', e.target.value)}
                required
                disabled={loading}
              />
            </div>

            <div className="form-field">
              <label htmlFor="hours">
                Hours<span className="required">*</span>
              </label>
              <input
                type="number"
                id="hours"
                value={formData.hours ?? ''}
                onChange={(e) => handleChange('hours', e.target.value === '' ? 0 : Number(e.target.value))}
                required
                disabled={loading}
                step="0.25"
                min="0"
              />
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

export default WorkRecordForm;
