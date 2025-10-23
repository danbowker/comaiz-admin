import React, { useState, useEffect } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { workRecordsService, contractsService, workersService, contractRatesService } from '../services/entityService';
import { WorkRecord, Contract, Worker, ContractRate } from '../types';

const WorkRecordsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<WorkRecord | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [contracts, setContracts] = useState<Contract[]>([]);
  const [workers, setWorkers] = useState<Worker[]>([]);
  const [contractRates, setContractRates] = useState<ContractRate[]>([]);

  useEffect(() => {
    loadRelatedData();
  }, []);

  const loadRelatedData = async () => {
    try {
      const [contractsData, workersData, ratesData] = await Promise.all([
        contractsService.getAll(),
        workersService.getAll(),
        contractRatesService.getAll(),
      ]);
      setContracts(contractsData);
      setWorkers(workersData);
      setContractRates(ratesData);
    } catch (err) {
      console.error('Failed to load related data', err);
    }
  };

  const columns = [
    { key: 'id' as keyof WorkRecord, label: 'ID' },
    { 
      key: 'contractId' as keyof WorkRecord, 
      label: 'Contract',
      render: (item: WorkRecord) => {
        const contract = contracts.find(c => c.id === item.contractId);
        return contract?.description || item.contractId;
      }
    },
    { 
      key: 'workerId' as keyof WorkRecord, 
      label: 'Worker',
      render: (item: WorkRecord) => {
        const worker = workers.find(w => w.id === item.workerId);
        return worker?.name || item.workerId;
      }
    },
    { key: 'startDate' as keyof WorkRecord, label: 'Start Date' },
    { key: 'endDate' as keyof WorkRecord, label: 'End Date' },
    { key: 'hours' as keyof WorkRecord, label: 'Hours' },
  ];

  const fields: FormField<WorkRecord>[] = [
    {
      name: 'contractId',
      label: 'Contract',
      type: 'select',
      required: true,
      options: contracts.map((c) => ({ value: c.id, label: c.description || `Contract ${c.id}` })),
    },
    {
      name: 'workerId',
      label: 'Worker',
      type: 'select',
      required: true,
      options: workers.map((w) => ({ value: w.id, label: w.name || `Worker ${w.id}` })),
    },
    { name: 'startDate', label: 'Start Date', type: 'date', required: true },
    { name: 'endDate', label: 'End Date', type: 'date', required: true },
    { name: 'hours', label: 'Hours', type: 'number', required: true },
    {
      name: 'contractRateId',
      label: 'Contract Rate',
      type: 'select',
      options: contractRates.map((r) => ({ value: r.id, label: r.description })),
    },
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
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Work Record' : 'Create Work Record'}
          service={workRecordsService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default WorkRecordsPage;
