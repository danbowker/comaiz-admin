import React, { useState } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { workersService } from '../services/entityService';
import { Worker } from '../types';

const WorkersPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Worker | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const columns = [
    { key: 'id' as keyof Worker, label: 'ID' },
    { key: 'name' as keyof Worker, label: 'Name' },
  ];

  const fields: FormField<Worker>[] = [
    { name: 'name', label: 'Name', type: 'text', required: true },
  ];

  const handleEdit = (item: Worker) => {
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
    await workersService.delete(id);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Workers"
        service={workersService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Worker' : 'Create Worker'}
          service={workersService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default WorkersPage;
