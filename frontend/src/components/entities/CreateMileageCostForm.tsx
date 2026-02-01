import React, { useState } from 'react';
import { invoiceItemsService } from '../../services/entityService';
import { Invoice, CreateMileageCostInvoiceItemDto } from '../../types';
import './EntityForm.css';

interface CreateMileageCostFormProps {
  onClose: () => void;
  onSave: () => void;
  invoices: Invoice[];
  defaultContractId?: number;
}

const CreateMileageCostForm: React.FC<CreateMileageCostFormProps> = ({
  onClose,
  onSave,
  invoices,
  defaultContractId,
}) => {
  const [formData, setFormData] = useState<CreateMileageCostInvoiceItemDto>({
    invoiceId: invoices.length > 0 ? invoices[0].id : 0,
    contractId: defaultContractId,
    quantity: 0,
    rate: 0.45, // UK standard mileage rate
    vatRate: 0.2, // Default 20% VAT
    description: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    
    if (!formData.invoiceId || formData.quantity <= 0 || formData.rate <= 0) {
      setError('Please provide valid invoice, distance and rate');
      return;
    }

    setLoading(true);
    try {
      await invoiceItemsService.createMileageCost(formData);
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

  const calculatedPrice = formData.quantity * formData.rate;
  const calculatedPriceIncVAT = calculatedPrice * (1 + formData.vatRate);

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Create Mileage Cost Invoice Item</h2>
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
              <label htmlFor="quantity">Distance (Miles) <span className="required">*</span></label>
              <input
                type="number"
                id="quantity"
                step="0.1"
                value={formData.quantity}
                onChange={(e) => setFormData({ ...formData, quantity: Number(e.target.value) })}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="rate">Rate (per mile) <span className="required">*</span></label>
              <input
                type="number"
                id="rate"
                step="0.01"
                value={formData.rate}
                onChange={(e) => setFormData({ ...formData, rate: Number(e.target.value) })}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="vatRate">VAT Rate (%)</label>
              <input
                type="number"
                id="vatRate"
                step="1"
                value={(formData.vatRate * 100).toFixed(0)}
                onChange={(e) => setFormData({ ...formData, vatRate: Number(e.target.value) / 100 })}
              />
              <small>e.g., 0 for 0%, 20 for 20%</small>
            </div>

            <div className="form-field">
              <label htmlFor="description">Description / Notes</label>
              <textarea
                id="description"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                rows={3}
              />
            </div>

            <div className="form-field" style={{ backgroundColor: '#f0f0f0', padding: '1rem', borderRadius: '4px' }}>
              <strong>Calculated Price:</strong> £{calculatedPrice.toFixed(2)}<br />
              <strong>Price Inc VAT:</strong> £{calculatedPriceIncVAT.toFixed(2)}
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

export default CreateMileageCostForm;
