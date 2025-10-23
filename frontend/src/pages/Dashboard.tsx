import React from 'react';
import './Dashboard.css';

const Dashboard: React.FC = () => {
  return (
    <div className="dashboard">
      <h1>Welcome to Comaiz Admin</h1>
      <p className="subtitle">Manage your consultancy business efficiently</p>
      
      <div className="dashboard-grid">
        <div className="dashboard-card">
          <h3>Clients</h3>
          <p>Manage client information and relationships</p>
        </div>
        <div className="dashboard-card">
          <h3>Workers</h3>
          <p>Track workers and their assignments</p>
        </div>
        <div className="dashboard-card">
          <h3>Contracts</h3>
          <p>Manage contracts with clients</p>
        </div>
        <div className="dashboard-card">
          <h3>Work Records</h3>
          <p>Record and track work hours</p>
        </div>
        <div className="dashboard-card">
          <h3>Invoices</h3>
          <p>Create and manage invoices</p>
        </div>
        <div className="dashboard-card">
          <h3>Costs</h3>
          <p>Track fixed costs and contract rates</p>
        </div>
      </div>

      <div className="info-section">
        <h2>Getting Started</h2>
        <ol>
          <li>Start by adding your <strong>Clients</strong> and <strong>Workers</strong></li>
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
