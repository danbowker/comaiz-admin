import React, { useState, useEffect } from 'react';
import EntityList from '../components/entities/EntityList';
import EntityForm, { FormField } from '../components/entities/EntityForm';
import { userContractRatesService, contractRatesService, usersService } from '../services/entityService';
import { UserContractRate, ContractRate, ApplicationUser } from '../types';

const UserContractRatesPage: React.FC = () => {
  const [showForm, setShowForm] = useState(false);
  const [selectedItem, setSelectedItem] = useState<UserContractRate | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [contractRates, setContractRates] = useState<ContractRate[]>([]);
  const [users, setUsers] = useState<ApplicationUser[]>([]);

  useEffect(() => {
    loadContractRates();
    loadUsers();
  }, []);

  const loadContractRates = async () => {
    try {
      const data = await contractRatesService.getAll();
      setContractRates(data);
    } catch (err) {
      console.error('Failed to load contract rates', err);
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

  const columns = [
    { key: 'id' as keyof UserContractRate, label: 'ID' },
    { 
      key: 'contractRateId' as keyof UserContractRate, 
      label: 'Contract Rate',
      render: (item: UserContractRate) => {
        if (item.contractRate) {
          return item.contractRate.description;
        }
        const contractRate = contractRates.find(cr => cr.id === item.contractRateId);
        return contractRate?.description || item.contractRateId;
      }
    },
    { 
      key: 'applicationUserId' as keyof UserContractRate, 
      label: 'User',
      render: (item: UserContractRate) => {
        if (item.applicationUser) {
          return getUserDisplayName(item.applicationUser);
        }
        const user = users.find(u => u.id === item.applicationUserId);
        return getUserDisplayName(user);
      }
    },
  ];

  const fields: FormField<UserContractRate>[] = [
    {
      name: 'contractRateId',
      label: 'Contract Rate',
      type: 'select',
      required: true,
      options: contractRates.map((cr) => ({ value: cr.id, label: cr.description || `Rate ${cr.id}` })),
    },
    {
      name: 'applicationUserId',
      label: 'User',
      type: 'select',
      required: true,
      options: users.map((u) => ({ value: u.id, label: getUserDisplayName(u) })),
    },
  ];

  const handleEdit = (item: UserContractRate) => {
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
    await userContractRatesService.delete(id);
  };

  return (
    <>
      <EntityList
        key={refreshKey}
        title="User Contract Rates"
        service={userContractRatesService}
        columns={columns}
        onEdit={handleEdit}
        onCreate={handleCreate}
        onDelete={handleDelete}
      />
      {showForm && (
        <EntityForm
          title={selectedItem ? 'Edit User Contract Rate' : 'Create User Contract Rate'}
          service={userContractRatesService}
          fields={fields}
          item={selectedItem}
          onClose={handleClose}
          onSave={handleSave}
        />
      )}
    </>
  );
};

export default UserContractRatesPage;
