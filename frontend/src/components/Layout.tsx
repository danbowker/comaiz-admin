import React from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Layout.css';

const Layout: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="layout">
      <nav className="navbar">
        <div className="navbar-brand">
          <h1>Comaiz Admin</h1>
        </div>
        <div className="navbar-menu">
          <Link to="/">Dashboard</Link>
          <Link to="/clients">Clients</Link>
          <Link to="/contracts">Contracts</Link>
          <Link to="/contract-rates">Contract Rates</Link>
          <Link to="/fixed-costs">Fixed Costs</Link>
          <Link to="/tasks">Tasks</Link>
          <Link to="/work-records">Work Records</Link>
          <Link to="/invoices">Invoices</Link>
          <Link to="/invoice-items">Invoice Items</Link>
        </div>
        <div className="navbar-user">
          <span>Welcome, {user?.username}</span>
          <button onClick={handleLogout}>Logout</button>
        </div>
      </nav>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
