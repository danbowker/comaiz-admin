import React, { useState, useEffect, useMemo } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { tasksService, contractsService, contractRatesService } from '../services/entityService';
import { Task, Contract, ContractRate } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';

const TasksPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Task | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [contracts, setContracts] = useState<Contract[]>([]);
  const [contractRates, setContractRates] = useState<ContractRate[]>([]);
  const { selectedContractId } = useContractSelection();

  useEffect(() => {
    loadRelatedData();
  }, []);

  const loadRelatedData = async () => {
    try {
      const [contractsData, ratesData] = await Promise.all([
        contractsService.getAll(),
        contractRatesService.getAll(),
      ]);
      setContracts(contractsData);
      setContractRates(ratesData);
    } catch (err) {
      console.error('Failed to load related data', err);
    }
  };

  const queryParams = useMemo(() => {
    return selectedContractId ? { contractId: selectedContractId } : undefined;
  }, [selectedContractId]);

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
    { 
      key: 'contractRateId' as keyof Task, 
      label: 'Contract Rate',
      render: (item: Task) => {
        if (item.contractRateId) {
          const rate = contractRates.find(r => r.id === item.contractRateId);
          return rate?.description || `Rate ${item.contractRateId}`;
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
      defaultValue: selectedContractId || undefined,
    },
    {
      name: 'contractRateId',
      label: 'Contract Rate',
      type: 'select',
      required: false,
      options: contractRates.map((r) => ({ value: r.id, label: r.description })),
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
        queryParams={queryParams}
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
