import React, { useState, useEffect, useMemo, useCallback } from 'react';
import EntityList from '../components/entities/EntityList';
import TaskForm from '../components/entities/TaskForm';
import { tasksService, contractsService, contractRatesService, userContractRatesService } from '../services/entityService';
import { Task, Contract, ContractRate, UserContractRate, RecordState } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';
import { useShowCompleteTasks } from '../hooks/useFilterPreferences';
import { isTaskEffectivelyActive } from '../utils/stateFilter';

const TasksPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Task | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [contracts, setContracts] = useState<Contract[]>([]);
  const [contractRates, setContractRates] = useState<ContractRate[]>([]);
  const [userContractRates, setUserContractRates] = useState<UserContractRate[]>([]);
  const { selectedContractId } = useContractSelection();
  const { showComplete, toggleShowComplete } = useShowCompleteTasks();

  const loadRelatedData = useCallback(async () => {
    try {
      const filterParams = selectedContractId ? { contractId: selectedContractId } : undefined;
      const [contractsData, ratesData, userRatesData] = await Promise.all([
        contractsService.getAll(),
        contractRatesService.getAll(filterParams),
        userContractRatesService.getAll(filterParams),
      ]);
      setContracts(contractsData);
      setContractRates(ratesData);
      setUserContractRates(userRatesData);
    } catch (err) {
      console.error('Failed to load related data', err);
    }
  }, [selectedContractId]);

  useEffect(() => {
    loadRelatedData();
  }, [loadRelatedData]);

  const queryParams = useMemo(() => {
    const params: Record<string, any> = {};
    if (selectedContractId) {
      params.contractId = selectedContractId;
    }
    // Use backend state filtering when not showing complete items
    if (!showComplete) {
      params.state = RecordState.Active;
    }
    return params;
  }, [selectedContractId, showComplete]);

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
      label: 'Assignments',
      render: (item: Task) => {
        if (item.taskContractRates && item.taskContractRates.length > 0) {
          return item.taskContractRates.map(tcr => {
            const ucr = tcr.userContractRate || userContractRates.find(u => u.id === tcr.userContractRateId);
            const rate = ucr?.contractRate || contractRates.find(r => r.id === ucr?.contractRateId);
            const userName = ucr?.applicationUser?.userName || ucr?.applicationUser?.email || 'User';
            return `${userName} - ${rate?.description || 'Rate'}`;
          }).join(', ');
        }
        return 'None';
      }
    },
    { 
      key: 'state' as keyof Task, 
      label: 'Status',
      render: (item: Task) => {
        const effectivelyActive = isTaskEffectivelyActive(item, contracts);
        if (!effectivelyActive && item.state === RecordState.Active) {
          // Task is active but contract is complete
          return (
            <span className="status-badge complete" title="Contract is complete">
              Complete (Contract)
            </span>
          );
        }
        return (
          <span className={`status-badge ${item.state === RecordState.Complete ? 'complete' : 'active'}`}>
            {item.state === RecordState.Complete ? 'Complete' : 'Active'}
          </span>
        );
      }
    },
  ];

  // Row class function to style complete items
  const getRowClassName = (item: Task) => {
    return !isTaskEffectivelyActive(item, contracts) ? 'row-complete' : '';
  };

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

  const handleDuplicate = async (id: number) => {
    const duplicatedItem = await tasksService.duplicate(id);
    setSelectedItem(duplicatedItem);
    setShowForm(true);
    setRefreshKey((prev) => prev + 1);
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
        onDuplicate={handleDuplicate}
        queryParams={queryParams}
        getRowClassName={getRowClassName}
        filterToggle={{
          label: 'Show complete tasks',
          checked: showComplete,
          onChange: toggleShowComplete,
        }}
      />
      {showForm && (
        <TaskForm
          task={selectedItem}
          contracts={contracts}
          contractRates={contractRates}
          userContractRates={userContractRates}
          onClose={handleClose}
          onSave={handleSave}
          defaultContractId={selectedContractId || undefined}
        />
      )}
    </>
  );
};

export default TasksPage;

