import React, { useState } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { clientsService } from '../services/entityService';
import { Client } from '../types';

const ClientsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Client | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const columns = [
    { key: 'id' as keyof Client, label: 'ID' },
    { key: 'shortName' as keyof Client, label: 'Short Name' },
    { key: 'name' as keyof Client, label: 'Name' },
  ];

  const fields: FormField<Client>[] = [
    { name: 'shortName', label: 'Short Name', type: 'text' },
    { name: 'name', label: 'Name', type: 'text', required: true },
  ];

  const handleEdit = (item: Client) => {
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
    await clientsService.delete(id);
  };

  const handleDuplicate = async (id: number) => {
    await clientsService.duplicate(id);
    setRefreshKey((prev) => prev + 1);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Clients"
        service={clientsService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
        onDuplicate={handleDuplicate}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Client' : 'Create Client'}
          service={clientsService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default ClientsPage;
