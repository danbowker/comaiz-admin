import { AuthProviderProps } from 'react-oidc-context';

export const oidcConfig: AuthProviderProps = {
  authority: 'https://accounts.google.com',
  client_id: process.env.REACT_APP_GOOGLE_CLIENT_ID || '',
  redirect_uri: `${window.location.origin}/callback`,
  post_logout_redirect_uri: window.location.origin,
  response_type: 'id_token',
  scope: 'openid email profile',
  automaticSilentRenew: true,
  includeIdTokenInSilentRenew: true,
  filterProtocolClaims: true,
  loadUserInfo: true,
  onSigninCallback: (user) => {
    // Store the token for API calls
    if (user?.id_token) {
      localStorage.setItem('access_token', user.id_token);
    }
  },
};