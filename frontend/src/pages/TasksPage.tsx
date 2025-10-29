import React, { useState } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { tasksService } from '../services/entityService';
import { Task } from '../types';

const TasksPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Task | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const columns = [
    { key: 'id' as keyof Task, label: 'ID' },
    { key: 'name' as keyof Task, label: 'Name' },
  ];

  const fields: FormField<Task>[] = [
    { name: 'name', label: 'Name', type: 'text', required: true },
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
