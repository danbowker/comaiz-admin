import React, { useState, useEffect } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { contractsService, clientsService } from '../services/entityService';
import { Contract, Client, ChargeType } from '../types';

const ContractsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Contract | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [clients, setClients] = useState<Client[]>([]);

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

  const columns = [
    { key: 'id' as keyof Contract, label: 'ID' },
    { 
      key: 'clientId' as keyof Contract, 
      label: 'Client',
      render: (item: Contract) => {
        const client = clients.find(c => c.id === item.clientId);
        return client?.name || item.clientId;
      }
    },
    { key: 'description' as keyof Contract, label: 'Description' },
    { key: 'price' as keyof Contract, label: 'Price' },
    { 
      key: 'chargeType' as keyof Contract, 
      label: 'Charge Type',
      render: (item: Contract) => ChargeType[item.chargeType]
    },
  ];

  const fields: FormField<Contract>[] = [
    {
      name: 'clientId',
      label: 'Client',
      type: 'select',
      required: true,
      options: clients.map((c) => ({ value: c.id, label: c.name || `Client ${c.id}` })),
    },
    { name: 'description', label: 'Description', type: 'text' },
    { name: 'price', label: 'Price', type: 'number' },
    { name: 'schedule', label: 'Schedule', type: 'text' },
    {
      name: 'chargeType',
      label: 'Charge Type',
      type: 'select',
      required: true,
      options: [
        { value: ChargeType.Fixed, label: 'Fixed' },
        { value: ChargeType.TimeAndMaterials, label: 'Time and Materials' },
      ],
    },
  ];

  const handleEdit = (item: Contract) => {
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
    await contractsService.delete(id);
  };

  const handleDuplicate = async (id: number) => {
    const duplicatedItem = await contractsService.duplicate(id);
    setSelectedItem(duplicatedItem);
    setShowForm(true);
    setRefreshKey((prev) => prev + 1);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Contracts"
        service={contractsService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
        onDuplicate={handleDuplicate}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Contract' : 'Create Contract'}
          service={contractsService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default ContractsPage;
