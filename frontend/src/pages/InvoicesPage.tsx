import React, { useState, useEffect, useMemo } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { invoicesService, clientsService } from '../services/entityService';
import { Invoice, Client } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';

const InvoicesPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Invoice | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [clients, setClients] = useState<Client[]>([]);
  const { selectedContractId } = useContractSelection();

  useEffect(() => {
    loadClients();
  }, []);

  const loadClients = async () => {
    try {
      const data = await clientsService.getAll();
      setClients(data);
    } catch (err) {
      console.error('Failed to load clients', err);
    }
  };

  const queryParams = useMemo(() => {
    if (selectedContractId) {
      return { contractId: selectedContractId };
    }
    return {};
  }, [selectedContractId]);

  const columns = [
    { key: 'id' as keyof Invoice, label: 'ID' },
    { 
      key: 'clientId' as keyof Invoice, 
      label: 'Client',
      render: (item: Invoice) => {
        const client = clients.find(c => c.id === item.clientId);
        return client?.name || item.clientId;
      }
    },
    { key: 'date' as keyof Invoice, label: 'Date' },
    { key: 'purchaseOrder' as keyof Invoice, label: 'Purchase Order' },
  ];

  const fields: FormField<Invoice>[] = [
    {
      name: 'clientId',
      label: 'Client',
      type: 'select',
      required: true,
      options: clients.map((c) => ({ value: c.id, label: c.name || `Client ${c.id}` })),
    },
    { name: 'date', label: 'Date', type: 'date', required: true },
    { name: 'purchaseOrder', label: 'Purchase Order', type: 'text' },
  ];

  const handleEdit = (item: Invoice) => {
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
    await invoicesService.delete(id);
  };

  const handleDuplicate = async (id: number) => {
    const duplicatedItem = await invoicesService.duplicate(id);
    setSelectedItem(duplicatedItem);
    setShowForm(true);
    setRefreshKey((prev) => prev + 1);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Invoices"
        service={invoicesService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
        onDuplicate={handleDuplicate}
        queryParams={queryParams}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Invoice' : 'Create Invoice'}
          service={invoicesService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default InvoicesPage;
