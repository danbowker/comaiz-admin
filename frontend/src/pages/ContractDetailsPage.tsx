import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getContractDetails, contractsService, clientsService } from '../services/entityService';
import { ContractDetails, Contract, Client, ChargeType } from '../types';
import './ContractDetailsPage.css';

const ContractDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [details, setDetails] = useState<ContractDetails | null>(null);
  const [contract, setContract] = useState<Contract | null>(null);
  const [client, setClient] = useState<Client | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadData = async () => {
      if (!id) return;

      try {
        setLoading(true);
        const contractId = parseInt(id, 10);
        
        const [detailsData, contractData] = await Promise.all([
          getContractDetails(contractId),
          contractsService.getById(contractId)
        ]);
        
        setDetails(detailsData);
        setContract(contractData);
        
        // Load client data
        if (contractData.clientId) {
          const clientData = await clientsService.getById(contractData.clientId);
          setClient(clientData);
        }
        
        setError('');
      } catch (err: any) {
        setError(err.response?.data?.message || 'Failed to load contract details');
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [id]);

  const formatCurrency = (value: number | null | undefined): string => {
    if (value === null || value === undefined) return '-';
    return new Intl.NumberFormat('en-GB', { style: 'currency', currency: 'GBP' }).format(value);
  };

  const formatDate = (dateString: string | null | undefined): string => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (error) {
    return (
      <div className="contract-details">
        <div className="error-message">{error}</div>
        <Link to="/contracts" className="back-link">← Back to Contracts</Link>
      </div>
    );
  }

  if (!details || !contract) {
    return (
      <div className="contract-details">
        <div className="error-message">Contract not found</div>
        <Link to="/contracts" className="back-link">← Back to Contracts</Link>
      </div>
    );
  }

  return (
    <div className="contract-details">
      <div className="details-header">
        <Link to="/contracts" className="back-link">← Back to Contracts</Link>
        <h2>Contract Details</h2>
      </div>

      <div className="contract-summary">
        <h3>Summary</h3>
        <div className="summary-grid">
          <div className="summary-item">
            <span className="label">Contract ID:</span>
            <span className="value">{contract.id}</span>
          </div>
          <div className="summary-item">
            <span className="label">Client:</span>
            <span className="value">{client?.name || contract.clientId}</span>
          </div>
          <div className="summary-item">
            <span className="label">Description:</span>
            <span className="value">{contract.description || '-'}</span>
          </div>
          <div className="summary-item">
            <span className="label">Charge Type:</span>
            <span className="value">{ChargeType[contract.chargeType]}</span>
          </div>
          <div className="summary-item">
            <span className="label">Contract Price:</span>
            <span className="value">{formatCurrency(contract.price)}</span>
          </div>
          <div className="summary-item">
            <span className="label">Planned Start:</span>
            <span className="value">{formatDate(contract.plannedStart)}</span>
          </div>
          <div className="summary-item">
            <span className="label">Planned End:</span>
            <span className="value">{formatDate(contract.plannedEnd)}</span>
          </div>
        </div>
      </div>

      <div className="invoice-tracking">
        <h3>Invoice Tracking</h3>
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Item</th>
                <th>Total Invoiced</th>
                <th>Total Paid</th>
                <th>Remaining</th>
                <th>Last Invoice End Date</th>
              </tr>
            </thead>
            <tbody>
              {/* Contract Summary Row */}
              <tr className="contract-row">
                <td><strong>Contract Total</strong></td>
                <td>{formatCurrency(details.totalInvoiced)}</td>
                <td>{formatCurrency(details.totalPaid)}</td>
                <td>{formatCurrency(details.remaining)}</td>
                <td>{formatDate(details.lastInvoiceEndDate)}</td>
              </tr>
              
              {/* Task Rows */}
              {details.tasks.map((task) => (
                <tr key={task.taskId} className="task-row">
                  <td className="task-name">{task.name}</td>
                  <td>{formatCurrency(task.totalInvoiced)}</td>
                  <td>{formatCurrency(task.totalPaid)}</td>
                  <td>-</td>
                  <td>{formatDate(task.lastInvoiceEndDate)}</td>
                </tr>
              ))}

              {details.tasks.length === 0 && (
                <tr>
                  <td colSpan={5} className="no-data">No tasks found for this contract</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default ContractDetailsPage;
