import React from 'react';
import { Link, Outlet, useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import ContractPicker from './ContractPicker';
import VersionInfo from './VersionInfo';
import './Layout.css';

const Layout: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  // Helper function to build link with preserved contract parameter
  const buildLink = (path: string): string => {
    const contractParam = searchParams.get('contract');
    if (contractParam) {
      return `${path}?contract=${contractParam}`;
    }
    return path;
  };

  return (
    <div className="layout">
      <nav className="navbar">
        <div className="navbar-brand">
          <h1>Comaiz Admin</h1>
        </div>
        <div className="navbar-menu">
          <Link to={buildLink('/')}>Dashboard</Link>
          <Link to={buildLink('/clients')}>Clients</Link>
          <Link to={buildLink('/contracts')}>Contracts</Link>
          <Link to={buildLink('/contract-rates')}>Contract Rates</Link>
          <Link to={buildLink('/fixed-costs')}>Fixed Costs</Link>
          <Link to={buildLink('/tasks')}>Tasks</Link>
          <Link to={buildLink('/work-records')}>Work Records</Link>
          <Link to={buildLink('/invoices')}>Invoices</Link>
          <Link to={buildLink('/invoice-items')}>Invoice Items</Link>
        </div>
        <div className="navbar-controls">
          <ContractPicker />
          <div className="navbar-user">
            <span>Welcome, {user?.username}</span>
            <button onClick={handleLogout}>Logout</button>
          </div>
          <VersionInfo />
        </div>
      </nav>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
