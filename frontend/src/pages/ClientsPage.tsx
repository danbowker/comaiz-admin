import React, { useState } from 'react';
import { GridColDef } from '@mui/x-data-grid';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Box,
  CircularProgress,
  Alert,
} from '@mui/material';
import { CrudTable } from '../components/CrudTable';
import { useCrud } from '../hooks/useApi';
import { apiService } from '../services/apiService';
import { Client } from '../types';

const ClientForm: React.FC<{
  item?: Client;
  onSubmit: (item: Client | Omit<Client, 'id'>) => Promise<void>;
  onCancel: () => void;
}> = ({ item, onSubmit, onCancel }) => {
  const [formData, setFormData] = useState({
    shortName: item?.shortName || '',
    name: item?.name || '',
  });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setSubmitting(true);
      setError(null);
      
      if (item) {
        await onSubmit({ ...item, ...formData });
      } else {
        await onSubmit(formData);
      }
      
      onCancel(); // Close dialog on success
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setSubmitting(false);
    }
  };

  const handleChange = (field: keyof typeof formData) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    setFormData(prev => ({
      ...prev,
      [field]: e.target.value,
    }));
  };

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      
      <TextField
        fullWidth
        label="Short Name"
        value={formData.shortName}
        onChange={handleChange('shortName')}
        margin="normal"
        required
      />
      
      <TextField
        fullWidth
        label="Full Name"
        value={formData.name}
        onChange={handleChange('name')}
        margin="normal"
        required
      />
      
      <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 1, mt: 3 }}>
        <Button onClick={onCancel} disabled={submitting}>
          Cancel
        </Button>
        <Button
          type="submit"
          variant="contained"
          disabled={submitting}
          startIcon={submitting ? <CircularProgress size={20} /> : null}
        >
          {submitting ? 'Saving...' : (item ? 'Update' : 'Create')}
        </Button>
      </Box>
    </Box>
  );
};

const ClientFormDialog: React.FC<{
  open: boolean;
  item?: Client;
  onSubmit: (item: Client | Omit<Client, 'id'>) => Promise<void>;
  onCancel: () => void;
}> = ({ open, item, onSubmit, onCancel }) => {
  return (
    <Dialog open={open} onClose={onCancel} maxWidth="sm" fullWidth>
      <DialogTitle>
        {item ? 'Edit Client' : 'Add New Client'}
      </DialogTitle>
      <DialogContent>
        <ClientForm item={item} onSubmit={onSubmit} onCancel={onCancel} />
      </DialogContent>
    </Dialog>
  );
};

export const ClientsPage: React.FC = () => {
  const [formDialogOpen, setFormDialogOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<Client | undefined>();

  const {
    data,
    loading,
    error,
    create,
    update,
    delete: deleteClient,
  } = useCrud(
    apiService.getClients,
    apiService.createClient,
    apiService.updateClient,
    apiService.deleteClient
  );

  const columns: GridColDef[] = [
    {
      field: 'id',
      headerName: 'ID',
      width: 90,
      type: 'number',
    },
    {
      field: 'shortName',
      headerName: 'Short Name',
      width: 150,
      editable: false,
    },
    {
      field: 'name',
      headerName: 'Full Name',
      width: 250,
      editable: false,
    },
  ];

  const handleAdd = () => {
    setEditingItem(undefined);
    setFormDialogOpen(true);
  };

  const handleEdit = (item: Client) => {
    setEditingItem(item);
    setFormDialogOpen(true);
  };

  const handleFormSubmit = async (item: Client | Omit<Client, 'id'>) => {
    if ('id' in item) {
      await update(item);
    } else {
      await create(item);
    }
  };

  const handleFormCancel = () => {
    setFormDialogOpen(false);
    setEditingItem(undefined);
  };

  return (
    <>
      <CrudTable
        title="Clients"
        data={data}
        columns={columns}
        loading={loading}
        error={error}
        onAdd={handleAdd}
        onEdit={handleEdit}
        onDelete={deleteClient}
        FormComponent={ClientForm}
      />
      
      <ClientFormDialog
        open={formDialogOpen}
        item={editingItem}
        onSubmit={handleFormSubmit}
        onCancel={handleFormCancel}
      />
    </>
  );
};