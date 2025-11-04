import React, { useState, useEffect, useMemo } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { fixedCostsService, contractsService } from '../services/entityService';
import { FixedCost, Contract } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';

const FixedCostsPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<FixedCost | null>(null);
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
    { key: 'id' as keyof FixedCost, label: 'ID' },
    { 
      key: 'contractId' as keyof FixedCost, 
      label: 'Contract',
      render: (item: FixedCost) => {
        const contract = contracts.find(c => c.id === item.contractId);
        return contract?.description || item.contractId;
      }
    },
    { key: 'name' as keyof FixedCost, label: 'Name' },
    { key: 'amount' as keyof FixedCost, label: 'Amount' },
  ];

  const fields: FormField<FixedCost>[] = [
    {
      name: 'contractId',
      label: 'Contract',
      type: 'select',
      required: true,
      options: contracts.map((c) => ({ value: c.id, label: c.description || `Contract ${c.id}` })),
      defaultValue: selectedContractId || undefined,
    },
    { name: 'name', label: 'Name', type: 'text' },
    { name: 'amount', label: 'Amount', type: 'number' },
  ];

  const handleEdit = (item: FixedCost) => {
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
    await fixedCostsService.delete(id);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="Fixed Costs"
        service={fixedCostsService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
        queryParams={queryParams}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit Fixed Cost' : 'Create Fixed Cost'}
          service={fixedCostsService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default FixedCostsPage;
