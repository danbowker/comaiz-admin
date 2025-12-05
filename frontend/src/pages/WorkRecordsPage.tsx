import React, { useState, useEffect, useMemo, useCallback } from 'react';
import EntityList from '../components/entities/EntityList';
import WorkRecordForm from '../components/entities/WorkRecordForm';
import { workRecordsService, usersService, tasksService } from '../services/entityService';
import { WorkRecord, ApplicationUser, Task } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';

const WorkRecordsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<WorkRecord | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [users, setUsers] = useState<ApplicationUser[]>([]);
  const { selectedContractId } = useContractSelection();

  const loadRelatedData = useCallback(async () => {
    try {
      const filterParams = selectedContractId ? { contractId: selectedContractId } : undefined;
      const [tasksData, usersData] = await Promise.all([
        tasksService.getAll(filterParams),
        usersService.getAll(),
      ]);
      setTasks(tasksData);
      setUsers(usersData);
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
    { key: 'id' as keyof WorkRecord, label: 'ID' },
    { 
      key: 'taskId' as keyof WorkRecord, 
      label: 'Task',
      render: (item: WorkRecord) => {
        const task = tasks.find(t => t.id === item.taskId);
        return task?.name || item.taskId || 'N/A';
      }
    },
    { 
      key: 'applicationUserId' as keyof WorkRecord, 
      label: 'User',
      render: (item: WorkRecord) => {
        const appUser = users.find(u => u.id === item.applicationUserId);
        return appUser?.userName || item.applicationUserId || 'N/A';
      }
    },
    { key: 'startDate' as keyof WorkRecord, label: 'Start Date' },
    { key: 'endDate' as keyof WorkRecord, label: 'End Date' },
    { key: 'hours' as keyof WorkRecord, label: 'Hours' },
  ];

  const handleEdit = (item: WorkRecord) => {
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
    await workRecordsService.delete(id);
  };

  const handleDuplicate = async (id: number) => {
    const duplicatedItem = await workRecordsService.duplicate(id);
    setSelectedItem(duplicatedItem);
    setShowForm(true);
    setRefreshKey((prev) => prev + 1);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Work Records"
        service={workRecordsService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
        onDuplicate={handleDuplicate}
        queryParams={queryParams}
      />
      {showForm && (
        <WorkRecordForm
          workRecord={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default WorkRecordsPage;
