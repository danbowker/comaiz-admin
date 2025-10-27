import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Dashboard.css';

const Dashboard: React.FC = () => {
  const navigate = useNavigate();

  const dashboardItems = [
    { title: 'Clients', description: 'Manage client information and relationships', path: '/clients' },
    { title: 'Contracts', description: 'Manage contracts with clients', path: '/contracts' },
    { title: 'Contract Rates', description: 'Define billing rates for contracts', path: '/contract-rates' },
    { title: 'Work Records', description: 'Record and track work hours', path: '/work-records' },
    { title: 'Fixed Costs', description: 'Track fixed costs', path: '/fixed-costs' },
    { title: 'Invoices', description: 'Create and manage invoices', path: '/invoices' },
    { title: 'Invoice Items', description: 'Manage invoice line items', path: '/invoice-items' },
  ];

  return (
    <div className="dashboard">
      <h1>Welcome to Comaiz Admin</h1>
      <p className="subtitle">Manage your consultancy business efficiently</p>
      
      <div className="dashboard-grid">
        {dashboardItems.map((item) => (
          <div 
            key={item.path}
            className="dashboard-card"
            onClick={() => navigate(item.path)}
            role="button"
            tabIndex={0}
            onKeyPress={(e) => {
              if (e.key === 'Enter' || e.key === ' ') {
                navigate(item.path);
              }
            }}
          >
            <h3>{item.title}</h3>
            <p>{item.description}</p>
          </div>
        ))}
      </div>

      <div className="info-section">
        <h2>Getting Started</h2>
        <ol>
          <li>Start by adding your <strong>Clients</strong></li>
          <li>Create <strong>Contracts</strong> with your clients</li>
          <li>Set up <strong>Contract Rates</strong> for billing</li>
          <li>Record <strong>Work</strong> as it's completed</li>
          <li>Generate <strong>Invoices</strong> for billing</li>
        </ol>
      </div>
    </div>
  );
};

export default Dashboard;
