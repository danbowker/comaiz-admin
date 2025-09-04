import React, { useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  Container,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Google as GoogleIcon } from '@mui/icons-material';
import { useAuth } from 'react-oidc-context';
import { useNavigate } from 'react-router-dom';

export const LoginPage: React.FC = () => {
  const auth = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    // Redirect to dashboard if already authenticated
    if (auth.isAuthenticated) {
      navigate('/clients');
    }
  }, [auth.isAuthenticated, navigate]);

  const handleLogin = () => {
    auth.signinRedirect();
  };

  if (auth.isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
        flexDirection="column"
        gap={2}
      >
        <CircularProgress />
        <Typography>Loading...</Typography>
      </Box>
    );
  }

  return (
    <Container component="main" maxWidth="sm">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Paper elevation={3} sx={{ padding: 4, width: '100%', textAlign: 'center' }}>
          <Typography component="h1" variant="h4" gutterBottom>
            Comaiz Admin
          </Typography>
          <Typography variant="h6" color="text.secondary" gutterBottom>
            Consultancy Management System
          </Typography>
          
          <Box sx={{ mt: 4, mb: 2 }}>
            <Typography variant="body1" gutterBottom>
              Please sign in with your Google account to continue
            </Typography>
          </Box>

          {auth.error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              Authentication failed: {auth.error.message}
            </Alert>
          )}

          <Button
            fullWidth
            variant="contained"
            size="large"
            startIcon={<GoogleIcon />}
            onClick={handleLogin}
            sx={{ mt: 3, mb: 2, py: 1.5 }}
          >
            Sign in with Google
          </Button>

          <Typography variant="body2" color="text.secondary" sx={{ mt: 3 }}>
            This application uses Google OAuth for authentication.
            <br />
            You must be authorized to access this system.
          </Typography>
        </Paper>
      </Box>
    </Container>
  );
};