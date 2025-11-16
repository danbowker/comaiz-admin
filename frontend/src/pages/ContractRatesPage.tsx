import React, { useState, useEffect, useMemo } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { contractRatesService, contractsService } from '../services/entityService';
import { ContractRate, Contract } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';

const ContractRatesPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<ContractRate | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [contracts, setContracts] = useState<Contract[]>([]);
  const { selectedContractId } = useContractSelection();

  useEffect(() => {
    loadContracts();
  }, []);

  const loadContracts = async () => {
    try {
      const data = await contractsService.getAll();
      setContracts(data);
    } catch (err) {
      console.error('Failed to load contracts', err);
    }
  };



  const queryParams = useMemo(() => {
    if (selectedContractId) {
      return { contractId: selectedContractId };
    }
    return {};
  }, [selectedContractId]);

  const columns = [
    { key: 'id' as keyof ContractRate, label: 'ID' },
    { 
      key: 'contractId' as keyof ContractRate, 
      label: 'Contract',
      render: (item: ContractRate) => {
        const contract = contracts.find(c => c.id === item.contractId);
        return contract?.description || item.contractId;
      }
    },
    { key: 'description' as keyof ContractRate, label: 'Description' },
    { key: 'invoiceDescription' as keyof ContractRate, label: 'Invoice Description' },
    { key: 'rate' as keyof ContractRate, label: 'Rate' },
  ];

  const fields: FormField<ContractRate>[] = [
    {
      name: 'contractId',
      label: 'Contract',
      type: 'select',
      required: true,
      options: contracts.map((c) => ({ value: c.id, label: c.description || `Contract ${c.id}` })),
      defaultValue: selectedContractId || undefined,
    },
    { name: 'description', label: 'Description', type: 'text', required: true },
    { name: 'invoiceDescription', label: 'Invoice Description', type: 'text', required: true },
    { name: 'rate', label: 'Rate', type: 'number' },
  ];

  const handleEdit = (item: ContractRate) => {
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
    await contractRatesService.delete(id);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Contract Rates"
        service={contractRatesService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
        queryParams={queryParams}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Contract Rate' : 'Create Contract Rate'}
          service={contractRatesService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default ContractRatesPage;
