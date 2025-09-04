import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, hasAuthParams, useAuth } from 'react-oidc-context';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { CssBaseline, Box } from '@mui/material';
import { oidcConfig } from './services/authConfig';
import { Layout } from './components/Layout';
import { LoginPage } from './pages/LoginPage';
import { ClientsPage } from './pages/ClientsPage';
import { WorkersPage } from './pages/WorkersPage';

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
    },
  },
});

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const auth = useAuth();

  if (auth.isLoading) {
    return <div>Loading...</div>;
  }

  if (!auth.isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

const AppContent: React.FC = () => {
  const auth = useAuth();

  // Handle the callback after authentication
  React.useEffect(() => {
    if (hasAuthParams() && !auth.isAuthenticated) {
      auth.signinRedirect();
    }
  }, [auth]);

  if (auth.isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        Loading...
      </Box>
    );
  }

  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/callback" element={<div>Processing authentication...</div>} />
        <Route
          path="/*"
          element={
            <ProtectedRoute>
              <Layout>
                <Routes>
                  <Route path="/" element={<Navigate to="/clients" replace />} />
                  <Route path="/clients" element={<ClientsPage />} />
                  <Route path="/workers" element={<WorkersPage />} />
                  <Route path="/contracts" element={<div>Contracts (Coming Soon)</div>} />
                  <Route path="/work-records" element={<div>Work Records (Coming Soon)</div>} />
                  <Route path="/invoices" element={<div>Invoices (Coming Soon)</div>} />
                  <Route path="/costs" element={<div>Fixed Costs (Coming Soon)</div>} />
                </Routes>
              </Layout>
            </ProtectedRoute>
          }
        />
      </Routes>
    </Router>
  );
};

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider {...oidcConfig}>
        <AppContent />
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
