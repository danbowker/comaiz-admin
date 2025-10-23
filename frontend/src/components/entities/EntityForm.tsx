import React, { useState, useEffect } from 'react';
import { EntityService } from '../../services/entityService';
import './EntityForm.css';

export interface FormField<T> {
  name: keyof T;
  label: string;
  type: 'text' | 'number' | 'date' | 'select';
  required?: boolean;
  options?: { value: any; label: string }[];
}

interface EntityFormProps<T extends { id?: number }> {
  title: string;
  service: EntityService<T>;
  fields: FormField<T>[];
  item: T | null;
  onClose: () => void;
  onSave: () => void;
}

function EntityForm<T extends { id?: number }>({
  title,
  service,
  fields,
  item,
  onClose,
  onSave,
}: EntityFormProps<T>) {
  const [formData, setFormData] = useState<Partial<T>>(item || {});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    setFormData(item || {});
  }, [item]);

  const handleChange = (name: keyof T, value: any) => {
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      if (formData.id) {
        await service.update(formData as T);
      } else {
        await service.create(formData as Omit<T, 'id'>);
      }
      onSave();
      onClose();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to save item');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{title}</h2>
          <button className="close-btn" onClick={onClose}>
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-body">
            {error && <div className="error-message">{error}</div>}

            {fields.map((field) => (
              <div key={String(field.name)} className="form-field">
                <label htmlFor={String(field.name)}>
                  {field.label}
                  {field.required && <span className="required">*</span>}
                </label>

                {field.type === 'select' ? (
                  <select
                    id={String(field.name)}
                    value={String(formData[field.name] ?? '')}
                    onChange={(e) =>
                      handleChange(
                        field.name,
                        e.target.value === '' ? null : Number(e.target.value)
                      )
                    }
                    required={field.required}
                    disabled={loading}
                  >
                    <option value="">Select...</option>
                    {field.options?.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </select>
                ) : (
                  <input
                    type={field.type}
                    id={String(field.name)}
                    value={String(formData[field.name] ?? '')}
                    onChange={(e) =>
                      handleChange(
                        field.name,
                        field.type === 'number'
                          ? e.target.value === ''
                            ? null
                            : Number(e.target.value)
                          : e.target.value
                      )
                    }
                    required={field.required}
                    disabled={loading}
                  />
                )}
              </div>
            ))}
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
}

export default EntityForm;
