import React, { useState, useEffect, useMemo, useCallback } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { invoiceItemsService, tasksService, fixedCostsService } from '../services/entityService';
import { InvoiceItem, Task, FixedCost } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';

const InvoiceItemsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<InvoiceItem | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [fixedCosts, setFixedCosts] = useState<FixedCost[]>([]);
  const { selectedContractId } = useContractSelection();

  const loadRelatedData = useCallback(async () => {
    try {
      const filterParams = selectedContractId ? { contractId: selectedContractId } : undefined;
      const [tasksData, fixedCostsData] = await Promise.all([
        tasksService.getAll(filterParams),
        fixedCostsService.getAll(filterParams),
      ]);
      setTasks(tasksData);
      setFixedCosts(fixedCostsData);
    } catch (err) {
      console.error('Failed to load related data', err);
    }
  }, [selectedContractId]);

  useEffect(() => {
    loadRelatedData();
  }, [loadRelatedData]);

  const queryParams = useMemo(() => {
    if (selectedContractId) {
      return { contractId: selectedContractId };
    }
    return {};
  }, [selectedContractId]);

  const columns = [
    { key: 'id' as keyof InvoiceItem, label: 'ID' },
    { key: 'invoiceId' as keyof InvoiceItem, label: 'Invoice ID' },
    { 
      key: 'taskId' as keyof InvoiceItem, 
      label: 'Task',
      render: (item: InvoiceItem) => {
        if (item.taskId) {
          const task = tasks.find(t => t.id === item.taskId);
          return task?.name || `Task ${item.taskId}`;
        }
        return 'N/A';
      }
    },
    { 
      key: 'fixedCostId' as keyof InvoiceItem, 
      label: 'Fixed Cost',
      render: (item: InvoiceItem) => {
        if (item.fixedCostId) {
          const fixedCost = fixedCosts.find(fc => fc.id === item.fixedCostId);
          return fixedCost?.name || `Fixed Cost ${item.fixedCostId}`;
        }
        return 'N/A';
      }
    },
    { key: 'quantity' as keyof InvoiceItem, label: 'Quantity' },
    { key: 'rate' as keyof InvoiceItem, label: 'Rate' },
    { key: 'price' as keyof InvoiceItem, label: 'Price' },
  ];

  const fields: FormField<InvoiceItem>[] = [
    { name: 'invoiceId', label: 'Invoice ID', type: 'number', required: true },
    {
      name: 'taskId',
      label: 'Task',
      type: 'select',
      required: false,
      options: tasks.map((t) => ({ value: t.id, label: t.name })),
    },
    {
      name: 'fixedCostId',
      label: 'Fixed Cost',
      type: 'select',
      required: false,
      options: fixedCosts.map((fc) => ({ value: fc.id, label: fc.name || `Fixed Cost ${fc.id}` })),
    },
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

  const handleDuplicate = async (id: number) => {
    await invoiceItemsService.duplicate(id);
    setRefreshKey((prev) => prev + 1);
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
        onDuplicate={handleDuplicate}
        queryParams={queryParams}
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
