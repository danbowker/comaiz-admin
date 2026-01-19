import React, { useState } from 'react';
import CreateFixedCostForm from './CreateFixedCostForm';
import CreateLabourCostForm from './CreateLabourCostForm';
import CreateMileageCostForm from './CreateMileageCostForm';
import { Invoice, FixedCost, Task, ApplicationUser, WorkRecord } from '../../types';

interface CreateInvoiceItemMenuProps {
  onClose: () => void;
  onSave: () => void;
  invoices: Invoice[];
  fixedCosts: FixedCost[];
  tasks: Task[];
  users: ApplicationUser[];
  workRecords: WorkRecord[];
  defaultContractId?: number;
}

type FormType = 'menu' | 'fixed-cost' | 'labour-cost' | 'mileage-cost';

const CreateInvoiceItemMenu: React.FC<CreateInvoiceItemMenuProps> = ({
  onClose,
  onSave,
  invoices,
  fixedCosts,
  tasks,
  users,
  workRecords,
  defaultContractId,
}) => {
  const [formType, setFormType] = useState<FormType>('menu');

  const handleFormSave = () => {
    onSave();
    onClose();
  };

  if (formType === 'fixed-cost') {
    return (
      <CreateFixedCostForm
        onClose={onClose}
        onSave={handleFormSave}
        invoices={invoices}
        fixedCosts={fixedCosts}
        defaultContractId={defaultContractId}
      />
    );
  }

  if (formType === 'labour-cost') {
    return (
      <CreateLabourCostForm
        onClose={onClose}
        onSave={handleFormSave}
        invoices={invoices}
        tasks={tasks}
        users={users}
        workRecords={workRecords}
        defaultContractId={defaultContractId}
      />
    );
  }

  if (formType === 'mileage-cost') {
    return (
      <CreateMileageCostForm
        onClose={onClose}
        onSave={handleFormSave}
        invoices={invoices}
        defaultContractId={defaultContractId}
      />
    );
  }

  // Menu selection
  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Create New Invoice Item</h2>
          <button className="close-btn" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>
        <div className="form-body">
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <button
              type="button"
              className="btn-submit"
              onClick={() => setFormType('fixed-cost')}
              style={{ padding: '1rem', fontSize: '1rem' }}
            >
              Fixed Cost
            </button>
            <button
              type="button"
              className="btn-submit"
              onClick={() => setFormType('labour-cost')}
              style={{ padding: '1rem', fontSize: '1rem' }}
            >
              Labour Cost
            </button>
            <button
              type="button"
              className="btn-submit"
              onClick={() => setFormType('mileage-cost')}
              style={{ padding: '1rem', fontSize: '1rem' }}
            >
              Mileage Cost
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CreateInvoiceItemMenu;
