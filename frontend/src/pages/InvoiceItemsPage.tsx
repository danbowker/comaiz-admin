import React, { useState } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { invoiceItemsService } from '../services/entityService';
import { InvoiceItem } from '../types';

const InvoiceItemsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<InvoiceItem | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const columns = [
    { key: 'id' as keyof InvoiceItem, label: 'ID' },
    { key: 'invoiceId' as keyof InvoiceItem, label: 'Invoice ID' },
    { key: 'costId' as keyof InvoiceItem, label: 'Cost ID' },
    { key: 'quantity' as keyof InvoiceItem, label: 'Quantity' },
    { key: 'rate' as keyof InvoiceItem, label: 'Rate' },
    { key: 'price' as keyof InvoiceItem, label: 'Price' },
  ];

  const fields: FormField<InvoiceItem>[] = [
    { name: 'invoiceId', label: 'Invoice ID', type: 'number', required: true },
    { name: 'costId', label: 'Cost ID', type: 'number', required: true },
    { name: 'quantity', label: 'Quantity', type: 'number', required: true },
    { name: 'unit', label: 'Unit', type: 'number', required: true },
    { name: 'rate', label: 'Rate', type: 'number', required: true },
    { name: 'vatRate', label: 'VAT Rate', type: 'number', required: true },
    { name: 'price', label: 'Price', type: 'number', required: true },
  ];

  const handleEdit = (item: InvoiceItem) => {
    setSelectedItem(item);
    setShowForm(true);
  };

  const handleCreate = () => {
    setSelectedItem(null);
    setShowForm(true);
  };

  const handleClose = () => {
    setShowForm(false);
    setSelectedItem(null);
  };

  const handleSave = () => {
    setRefreshKey((prev) => prev + 1);
  };

  const handleDelete = async (id: number) => {
    await invoiceItemsService.delete(id);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Invoice Items"
        service={invoiceItemsService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Invoice Item' : 'Create Invoice Item'}
          service={invoiceItemsService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default InvoiceItemsPage;
