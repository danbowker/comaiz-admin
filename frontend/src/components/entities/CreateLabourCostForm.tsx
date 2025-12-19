import React, { useState, useMemo, useEffect } from 'react';
import { invoiceItemsService } from '../../services/entityService';
import { Invoice, Task, ApplicationUser, WorkRecord, CreateLabourCostInvoiceItemDto, RecordState } from '../../types';
import './EntityForm.css';

interface CreateLabourCostFormProps {
  onClose: () => void;
  onSave: () => void;
  invoices: Invoice[];
  tasks: Task[];
  users: ApplicationUser[];
  workRecords: WorkRecord[];
  defaultContractId?: number;
}

const CreateLabourCostForm: React.FC<CreateLabourCostFormProps> = ({
  onClose,
  onSave,
  invoices,
  tasks,
  users,
  workRecords,
  defaultContractId,
}) => {
  const [formData, setFormData] = useState<CreateLabourCostInvoiceItemDto>({
    invoiceId: invoices.length > 0 ? invoices[0].id : 0,
    contractId: defaultContractId,
    taskId: 0,
    applicationUserId: undefined,
    startDate: undefined,
    endDate: undefined,
    quantity: undefined,
    rate: undefined,
    vatRate: 0.20,
    description: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Filter tasks by contract if provided
  const filteredTasks = useMemo(() => {
    let result = tasks;
    if (defaultContractId) {
      result = result.filter((t) => t.contractId === defaultContractId);
    }
    // Only show active tasks
    result = result.filter((t) => t.state === RecordState.Active);
    return result;
  }, [tasks, defaultContractId]);

  // Filter users who have work records for the selected task
  const filteredUsers = useMemo(() => {
    if (!formData.taskId) return users;
    
    const userIds = new Set(
      workRecords
        .filter((wr) => wr.taskId === formData.taskId)
        .map((wr) => wr.applicationUserId)
        .filter((id): id is string => !!id)
    );
    
    return users.filter((u) => userIds.has(u.id));
  }, [formData.taskId, users, workRecords]);

  // Calculate quantity from work records when dates and user are selected
  const calculatedQuantity = useMemo(() => {
    if (formData.taskId && formData.applicationUserId && formData.startDate && formData.endDate) {
      const startDate = new Date(formData.startDate);
      const endDate = new Date(formData.endDate);
      
      const relevantRecords = workRecords.filter((wr) => {
        if (wr.taskId !== formData.taskId || wr.applicationUserId !== formData.applicationUserId) {
          return false;
        }
        const wrStart = new Date(wr.startDate);
        const wrEnd = new Date(wr.endDate);
        return wrStart >= startDate && wrEnd <= endDate;
      });
      
      return relevantRecords.reduce((sum, wr) => sum + wr.hours, 0);
    }
    return undefined;
  }, [formData.taskId, formData.applicationUserId, formData.startDate, formData.endDate, workRecords]);

  useEffect(() => {
    if (calculatedQuantity !== undefined) {
      setFormData((prev) => ({ ...prev, quantity: calculatedQuantity }));
    }
  }, [calculatedQuantity]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    
    if (!formData.invoiceId || !formData.taskId) {
      setError('Please select an invoice and task');
      return;
    }

    if (!formData.quantity || formData.quantity <= 0) {
      setError('Please provide a valid quantity (hours)');
      return;
    }

    if (!formData.rate || formData.rate <= 0) {
      setError('Please provide a valid rate');
      return;
    }

    setLoading(true);
    try {
      await invoiceItemsService.createLabourCost(formData);
      onSave();
    } catch (err) {
      setError('Failed to create invoice item');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const formatInvoiceLabel = (invoice: Invoice) => {
    const date = new Date(invoice.date).toLocaleDateString();
    return `Invoice ${invoice.id} - ${date}${invoice.purchaseOrder ? ` (${invoice.purchaseOrder})` : ''}`;
  };

  const calculatedPrice = (formData.quantity || 0) * (formData.rate || 0);
  const calculatedPriceIncVAT = calculatedPrice * (1 + formData.vatRate);

  return (
    <div className="entity-form-overlay">
      <div className="entity-form-container" style={{ maxWidth: '600px' }}>
        <div className="entity-form-header">
          <h2>Create Labour Cost Invoice Item</h2>
          <button className="close-button" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>
        <form onSubmit={handleSubmit} className="entity-form-body">
          {error && <div className="error-message">{error}</div>}
          
          <div className="form-field">
            <label htmlFor="invoiceId">Invoice (Draft) *</label>
            <select
              id="invoiceId"
              value={formData.invoiceId}
              onChange={(e) => setFormData({ ...formData, invoiceId: Number(e.target.value) })}
              required
            >
              <option value={0}>Select an invoice</option>
              {invoices.map((inv) => (
                <option key={inv.id} value={inv.id}>
                  {formatInvoiceLabel(inv)}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label htmlFor="taskId">Task *</label>
            <select
              id="taskId"
              value={formData.taskId}
              onChange={(e) => {
                const newTaskId = Number(e.target.value);
                setFormData({ 
                  ...formData, 
                  taskId: newTaskId, 
                  applicationUserId: undefined,
                  quantity: undefined
                });
              }}
              required
            >
              <option value={0}>Select a task</option>
              {filteredTasks.map((task) => (
                <option key={task.id} value={task.id}>
                  {task.name}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label htmlFor="applicationUserId">Worker (optional)</label>
            <select
              id="applicationUserId"
              value={formData.applicationUserId || ''}
              onChange={(e) => setFormData({ ...formData, applicationUserId: e.target.value || undefined })}
            >
              <option value="">Select a worker</option>
              {filteredUsers.map((user) => (
                <option key={user.id} value={user.id}>
                  {user.userName || user.email}
                </option>
              ))}
            </select>
            <small>Filtered to workers who have worked on the selected task</small>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-field">
              <label htmlFor="startDate">Start Date</label>
              <input
                type="date"
                id="startDate"
                value={formData.startDate || ''}
                onChange={(e) => setFormData({ ...formData, startDate: e.target.value || undefined })}
              />
            </div>

            <div className="form-field">
              <label htmlFor="endDate">End Date</label>
              <input
                type="date"
                id="endDate"
                value={formData.endDate || ''}
                onChange={(e) => setFormData({ ...formData, endDate: e.target.value || undefined })}
              />
            </div>
          </div>

          <div className="form-field">
            <label htmlFor="quantity">Quantity (Hours) *</label>
            <input
              type="number"
              id="quantity"
              step="0.01"
              value={formData.quantity || ''}
              onChange={(e) => setFormData({ ...formData, quantity: e.target.value ? Number(e.target.value) : undefined })}
              required
            />
            <small>Auto-calculated from work records if dates and worker are selected</small>
          </div>

          <div className="form-field">
            <label htmlFor="rate">Hourly Rate *</label>
            <input
              type="number"
              id="rate"
              step="0.01"
              value={formData.rate || ''}
              onChange={(e) => setFormData({ ...formData, rate: e.target.value ? Number(e.target.value) : undefined })}
              required
            />
          </div>

          <div className="form-field">
            <label htmlFor="vatRate">VAT Rate (decimal)</label>
            <input
              type="number"
              id="vatRate"
              step="0.01"
              value={formData.vatRate}
              onChange={(e) => setFormData({ ...formData, vatRate: Number(e.target.value) })}
            />
            <small>e.g., 0.20 for 20%</small>
          </div>

          <div className="form-field">
            <label htmlFor="description">Description / Notes</label>
            <textarea
              id="description"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              rows={2}
            />
            <small>Auto-generated from task, worker and dates if left blank</small>
          </div>

          <div className="form-field" style={{ backgroundColor: '#f0f0f0', padding: '1rem', borderRadius: '4px' }}>
            <strong>Calculated Price:</strong> £{calculatedPrice.toFixed(2)}<br />
            <strong>Price Inc VAT:</strong> £{calculatedPriceIncVAT.toFixed(2)}
          </div>

          <div className="form-actions">
            <button type="button" onClick={onClose} disabled={loading}>
              Cancel
            </button>
            <button type="submit" className="button-primary" disabled={loading}>
              {loading ? 'Creating...' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateLabourCostForm;
