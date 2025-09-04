import React, { useState } from 'react';
import { GridColDef } from '@mui/x-data-grid';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  TextField,
  Button,
  Box,
  CircularProgress,
  Alert,
} from '@mui/material';
import { CrudTable } from '../components/CrudTable';
import { useCrud } from '../hooks/useApi';
import { apiService } from '../services/apiService';
import { Worker } from '../types';

const WorkerForm: React.FC<{
  item?: Worker;
  onSubmit: (item: Worker | Omit<Worker, 'id'>) => Promise<void>;
  onCancel: () => void;
}> = ({ item, onSubmit, onCancel }) => {
  const [formData, setFormData] = useState({
    firstName: item?.firstName || '',
    lastName: item?.lastName || '',
    email: item?.email || '',
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
      
      onCancel();
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
        label="First Name"
        value={formData.firstName}
        onChange={handleChange('firstName')}
        margin="normal"
        required
      />
      
      <TextField
        fullWidth
        label="Last Name"
        value={formData.lastName}
        onChange={handleChange('lastName')}
        margin="normal"
        required
      />
      
      <TextField
        fullWidth
        label="Email"
        type="email"
        value={formData.email}
        onChange={handleChange('email')}
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

const WorkerFormDialog: React.FC<{
  open: boolean;
  item?: Worker;
  onSubmit: (item: Worker | Omit<Worker, 'id'>) => Promise<void>;
  onCancel: () => void;
}> = ({ open, item, onSubmit, onCancel }) => {
  return (
    <Dialog open={open} onClose={onCancel} maxWidth="sm" fullWidth>
      <DialogTitle>
        {item ? 'Edit Worker' : 'Add New Worker'}
      </DialogTitle>
      <DialogContent>
        <WorkerForm item={item} onSubmit={onSubmit} onCancel={onCancel} />
      </DialogContent>
    </Dialog>
  );
};

export const WorkersPage: React.FC = () => {
  const [formDialogOpen, setFormDialogOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<Worker | undefined>();

  const {
    data,
    loading,
    error,
    create,
    update,
    delete: deleteWorker,
  } = useCrud(
    apiService.getWorkers,
    apiService.createWorker,
    apiService.updateWorker,
    apiService.deleteWorker
  );

  const columns: GridColDef[] = [
    {
      field: 'id',
      headerName: 'ID',
      width: 90,
      type: 'number',
    },
    {
      field: 'firstName',
      headerName: 'First Name',
      width: 150,
    },
    {
      field: 'lastName',
      headerName: 'Last Name',
      width: 150,
    },
    {
      field: 'email',
      headerName: 'Email',
      width: 250,
    },
  ];

  const handleAdd = () => {
    setEditingItem(undefined);
    setFormDialogOpen(true);
  };

  const handleEdit = (item: Worker) => {
    setEditingItem(item);
    setFormDialogOpen(true);
  };

  const handleFormSubmit = async (item: Worker | Omit<Worker, 'id'>) => {
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
        title="Workers"
        data={data}
        columns={columns}
        loading={loading}
        error={error}
        onAdd={handleAdd}
        onEdit={handleEdit}
        onDelete={deleteWorker}
        FormComponent={WorkerForm}
      />
      
      <WorkerFormDialog
        open={formDialogOpen}
        item={editingItem}
        onSubmit={handleFormSubmit}
        onCancel={handleFormCancel}
      />
    </>
  );
};