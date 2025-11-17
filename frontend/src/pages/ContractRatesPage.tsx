import React, { useState, useEffect, useMemo } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { contractRatesService, contractsService, usersService } from '../services/entityService';
import { ContractRate, Contract, ApplicationUser } from '../types';
import { useContractSelection } from '../contexts/ContractSelectionContext';

const ContractRatesPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<ContractRate | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [contracts, setContracts] = useState<Contract[]>([]);
  const [users, setUsers] = useState<ApplicationUser[]>([]);
  const { selectedContractId } = useContractSelection();

  useEffect(() => {
    loadContracts();
    loadUsers();
  }, []);

  const loadContracts = async () => {
    try {
      const data = await contractsService.getAll();
      setContracts(data);
    } catch (err) {
      console.error('Failed to load contracts', err);
    }
  };

  const loadUsers = async () => {
    try {
      const data = await usersService.getAll();
      setUsers(data);
    } catch (err) {
      console.error('Failed to load users', err);
    }
  };

  const getUserDisplayName = (user: ApplicationUser | undefined): string => {
    if (!user) return '';
    return user.userName || user.email || user.id;
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
    { key: 'rate' as keyof ContractRate, label: 'Rate' },
    { 
      key: 'applicationUserId' as keyof ContractRate, 
      label: 'User',
      render: (item: ContractRate) => {
        if (item.applicationUser) {
          return getUserDisplayName(item.applicationUser);
        }
        if (item.applicationUserId) {
          const user = users.find(u => u.id === item.applicationUserId);
          return getUserDisplayName(user);
        }
        return '';
      }
    },
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
    { name: 'rate', label: 'Rate', type: 'number' },
    {
      name: 'applicationUserId',
      label: 'User',
      type: 'select',
      required: false,
      options: users.map((u) => ({ value: u.id, label: getUserDisplayName(u) })),
    },
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

  const handleDuplicate = async (id: number) => {
    await contractRatesService.duplicate(id);
    setRefreshKey((prev) => prev + 1);
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
        onDuplicate={handleDuplicate}
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
