import React, { useState, useEffect, useMemo } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { contractsService, clientsService } from '../services/entityService';
import { Contract, Client, ChargeType, RecordState } from '../types';
import { useShowCompleteContracts } from '../hooks/useFilterPreferences';

const ContractsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Contract | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [clients, setClients] = useState<Client[]>([]);
  const { showComplete, toggleShowComplete } = useShowCompleteContracts();

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
    { 
      key: 'plannedStart' as keyof Contract, 
      label: 'Planned Start',
      render: (item: Contract) => item.plannedStart ? new Date(item.plannedStart).toLocaleDateString() : ''
    },
    { 
      key: 'plannedEnd' as keyof Contract, 
      label: 'Planned End',
      render: (item: Contract) => item.plannedEnd ? new Date(item.plannedEnd).toLocaleDateString() : ''
    },
    { 
      key: 'state' as keyof Contract, 
      label: 'Status',
      render: (item: Contract) => (
        <span className={`status-badge ${item.state === RecordState.Complete ? 'complete' : 'active'}`}>
          {item.state === RecordState.Complete ? 'Complete' : 'Active'}
        </span>
      )
    },
  ];

  // Query params to filter by state on the backend
  const queryParams = useMemo(() => {
    if (!showComplete) {
      return { state: RecordState.Active };
    }
    return {};
  }, [showComplete]);

  // Row class function to style complete items
  const getRowClassName = (item: Contract) => {
    return item.state === RecordState.Complete ? 'row-complete' : '';
  };

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
    { name: 'plannedStart', label: 'Planned Start', type: 'date' },
    { name: 'plannedEnd', label: 'Planned End', type: 'date' },
    {
      name: 'state',
      label: 'Status',
      type: 'select',
      required: true,
      options: [
        { value: RecordState.Active, label: 'Active' },
        { value: RecordState.Complete, label: 'Complete' },
      ],
      defaultValue: RecordState.Active,
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
        queryParams={queryParams}
        getRowClassName={getRowClassName}
        filterToggle={{
          label: 'Show complete contracts',
          checked: showComplete,
          onChange: toggleShowComplete,
        }}
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
