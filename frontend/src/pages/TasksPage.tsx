import React, { useState, useEffect } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { tasksService, contractsService } from '../services/entityService';
import { Task, Contract } from '../types';

const TasksPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Task | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [contracts, setContracts] = useState<Contract[]>([]);

  useEffect(() => {
    loadRelatedData();
  }, []);

  const loadRelatedData = async () => {
    try {
      const contractsData = await contractsService.getAll();
      setContracts(contractsData);
    } catch (err) {
      console.error('Failed to load related data', err);
    }
  };

  const columns = [
    { key: 'id' as keyof Task, label: 'ID' },
    { key: 'name' as keyof Task, label: 'Name' },
    { 
      key: 'contractId' as keyof Task, 
      label: 'Contract',
      render: (item: Task) => {
        if (item.contractId) {
          const contract = contracts.find(c => c.id === item.contractId);
          return contract?.description || `Contract ${item.contractId}`;
        }
        return 'N/A';
      }
    },
  ];

  const fields: FormField<Task>[] = [
    { name: 'name', label: 'Name', type: 'text', required: true },
    {
      name: 'contractId',
      label: 'Contract',
      type: 'select',
      required: false,
      options: contracts.map((c) => ({ value: c.id, label: c.description || `Contract ${c.id}` })),
    },
  ];

  const handleEdit = (item: Task) => {
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
    await tasksService.delete(id);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Tasks"
        service={tasksService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Task' : 'Create Task'}
          service={tasksService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default TasksPage;
