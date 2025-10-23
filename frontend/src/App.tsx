import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/auth/ProtectedRoute';
import Login from './components/auth/Login';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import ClientsPage from './pages/ClientsPage';
import WorkersPage from './pages/WorkersPage';
import ContractsPage from './pages/ContractsPage';
import ContractRatesPage from './pages/ContractRatesPage';
import FixedCostsPage from './pages/FixedCostsPage';
import WorkRecordsPage from './pages/WorkRecordsPage';
import InvoicesPage from './pages/InvoicesPage';
import InvoiceItemsPage from './pages/InvoiceItemsPage';
import './App.css';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Dashboard />} />
            <Route path="clients" element={<ClientsPage />} />
            <Route path="workers" element={<WorkersPage />} />
            <Route path="contracts" element={<ContractsPage />} />
            <Route path="contract-rates" element={<ContractRatesPage />} />
            <Route path="fixed-costs" element={<FixedCostsPage />} />
            <Route path="work-records" element={<WorkRecordsPage />} />
            <Route path="invoices" element={<InvoicesPage />} />
            <Route path="invoice-items" element={<InvoiceItemsPage />} />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
