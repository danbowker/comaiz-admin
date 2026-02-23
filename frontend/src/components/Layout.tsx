import React from 'react';
import { NavLink, Outlet, useNavigate, useSearchParams } from 'react-router-dom';
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

  const navLinkClass = ({ isActive }: { isActive: boolean }) =>
    isActive ? 'nav-link active' : 'nav-link';

  return (
    <div className="layout">
      <nav className="navbar">
        <div className="navbar-brand">
          <h1>Comaiz Admin</h1>
        </div>
        <div className="navbar-menu">
          <NavLink to={buildLink('/')} end className={navLinkClass}>Dashboard</NavLink>
          <NavLink to={buildLink('/clients')} className={navLinkClass}>Clients</NavLink>
          <NavLink to={buildLink('/contracts')} className={navLinkClass}>Contracts</NavLink>
          <NavLink to={buildLink('/contract-rates')} className={navLinkClass}>Contract Rates</NavLink>
          <NavLink to={buildLink('/user-contract-rates')} className={navLinkClass}>User Contract Rates</NavLink>
          <NavLink to={buildLink('/fixed-costs')} className={navLinkClass}>Fixed Costs</NavLink>
          <NavLink to={buildLink('/tasks')} className={navLinkClass}>Tasks</NavLink>
          <NavLink to={buildLink('/work-records')} className={navLinkClass}>Work Records</NavLink>
          <NavLink to={buildLink('/weekly-summary')} className={navLinkClass}>Weekly Summary</NavLink>
          <NavLink to={buildLink('/invoices')} className={navLinkClass}>Invoices</NavLink>
          <NavLink to={buildLink('/invoice-items')} className={navLinkClass}>Invoice Items</NavLink>
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
