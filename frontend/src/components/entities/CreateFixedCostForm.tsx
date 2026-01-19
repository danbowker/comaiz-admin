import React, { useState } from 'react';
import { invoiceItemsService } from '../../services/entityService';
import { Invoice, FixedCost, CreateFixedCostInvoiceItemDto } from '../../types';
import './EntityForm.css';

interface CreateFixedCostFormProps {
  onClose: () => void;
  onSave: () => void;
  invoices: Invoice[];
  fixedCosts: FixedCost[];
  defaultContractId?: number;
}

const CreateFixedCostForm: React.FC<CreateFixedCostFormProps> = ({
  onClose,
  onSave,
  invoices,
  fixedCosts,
  defaultContractId,
}) => {
  const [formData, setFormData] = useState<CreateFixedCostInvoiceItemDto>({
    invoiceId: invoices.length > 0 ? invoices[0].id : 0,
    fixedCostId: 0,
    vatRate: 0,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Filter fixed costs by contract if provided
  const filteredFixedCosts = defaultContractId
    ? fixedCosts.filter((fc) => fc.contractId === defaultContractId)
    : fixedCosts;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    
    if (!formData.invoiceId || !formData.fixedCostId) {
      setError('Please select an invoice and fixed cost');
      return;
    }

    setLoading(true);
    try {
      await invoiceItemsService.createFixedCost(formData);
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

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Create Fixed Cost Invoice Item</h2>
          <button className="close-btn" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="form-body">
            {error && <div className="error-message">{error}</div>}
            
            <div className="form-field">
              <label htmlFor="invoiceId">Invoice (Draft) <span className="required">*</span></label>
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
              <label htmlFor="fixedCostId">Fixed Cost <span className="required">*</span></label>
              <select
                id="fixedCostId"
                value={formData.fixedCostId}
                onChange={(e) => setFormData({ ...formData, fixedCostId: Number(e.target.value) })}
                required
              >
                <option value={0}>Select a fixed cost</option>
                {filteredFixedCosts.map((fc) => (
                  <option key={fc.id} value={fc.id}>
                    {fc.name || `Fixed Cost ${fc.id}`} {fc.amount ? `(£${fc.amount})` : ''}
                  </option>
                ))}
              </select>
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
              <small>e.g., 0 for 0%, 0.20 for 20%</small>
            </div>
          </div>

          <div className="form-footer">
            <button type="button" className="btn-cancel" onClick={onClose} disabled={loading}>
              Cancel
            </button>
            <button type="submit" className="btn-submit" disabled={loading}>
              {loading ? 'Creating...' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateFixedCostForm;
