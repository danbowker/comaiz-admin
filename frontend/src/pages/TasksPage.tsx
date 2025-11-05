import React, { useState, useEffect, useMemo, useCallback } from 'react';
import EntityList from '../components/entities/EntityList';
import TaskForm from '../components/entities/TaskForm';
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

  const loadRelatedData = useCallback(async () => {
    try {
      const filterParams = selectedContractId ? { contractId: selectedContractId } : undefined;
      const [contractsData, ratesData] = await Promise.all([
        contractsService.getAll(),
        contractRatesService.getAll(filterParams),
      ]);
      setContracts(contractsData);
      setContractRates(ratesData);
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
      key: 'taskContractRates' as keyof Task, 
      label: 'Contract Rates',
      render: (item: Task) => {
        if (item.taskContractRates && item.taskContractRates.length > 0) {
          return item.taskContractRates.map(tcr => {
            const rate = tcr.contractRate || contractRates.find(r => r.id === tcr.contractRateId);
            return rate?.description || `Rate ${tcr.contractRateId}`;
          }).join(', ');
        }
        return 'None';
      }
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
        <TaskForm
          task={selectedItem}
          contracts={contracts}
          contractRates={contractRates}
          onClose={handleClose}
          onSave={handleSave}
          defaultContractId={selectedContractId || undefined}
        />
      )}
    </>
  );
};

export default TasksPage;

