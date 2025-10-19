# Authentication and Authorization Guide

This guide explains how to use authentication and authorization in the Comaiz Admin API.

## Overview

The API uses:
- **ASP.NET Identity** for user and role management
- **JWT (JSON Web Tokens)** for authentication
- **Bearer token authentication** for securing API endpoints

## Quick Start

### Default Users

The application automatically creates two default users on startup:

1. **Admin User**
   - Username: `admin`
   - Password: `Admin@123`
   - Role: `Admin`
   - Email: `admin@comaiz.local`

2. **Test User**
   - Username: `testuser`
   - Password: `Test@123`
   - Role: `User`
   - Email: `testuser@comaiz.local`

⚠️ **Security Note**: Change these default passwords immediately in production environments!

## Using Swagger UI

### 1. Access Swagger UI

Navigate to: `https://localhost:7057/swagger` (or your configured port)

### 2. Login to Get a Token

1. Locate the **Auth** section in Swagger
2. Expand the `POST /api/auth/login` endpoint
3. Click "Try it out"
4. Enter credentials:
   ```json
   {
     "username": "admin",
     "password": "Admin@123"
   }
   ```
5. Click "Execute"
6. Copy the `token` value from the response

### 3. Authorize Swagger

1. Click the **"Authorize"** button at the top of the Swagger page (lock icon)
2. In the dialog, enter: `Bearer YOUR_TOKEN_HERE`
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Click "Authorize"
4. Click "Close"

### 4. Test Protected Endpoints

Now you can test any API endpoint. The authorization token will be included automatically in all requests.

## Using the API Programmatically

### Login Request

**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@comaiz.local",
  "roles": ["Admin"]
}
```

### Using the Token

Include the token in the `Authorization` header of all subsequent requests:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example with cURL

```bash
# Login
curl -X POST https://localhost:7057/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'

# Use the token (replace TOKEN with the actual token)
curl -X GET https://localhost:7057/api/clients \
  -H "Authorization: Bearer TOKEN"
```

### Example with PowerShell

```powershell
# Login
$loginBody = @{
    username = "admin"
    password = "Admin@123"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "https://localhost:7057/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body $loginBody

$token = $loginResponse.token

# Use the token
$headers = @{
    Authorization = "Bearer $token"
}

$clients = Invoke-RestMethod -Uri "https://localhost:7057/api/clients" `
    -Method Get `
    -Headers $headers
```

### Example with C#

```csharp
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

// Login
var loginRequest = new { username = "admin", password = "Admin@123" };
var loginResponse = await httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

// Use the token
httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", authResult.Token);

// Make authenticated requests
var clients = await httpClient.GetFromJsonAsync<List<Client>>("/api/clients");
```

## User Management

### Register a New User

**Endpoint:** `POST /api/auth/register`

**Request Body:**
```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "SecurePassword@123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "newuser",
  "email": "newuser@example.com",
  "roles": ["User"]
}
```

New users are automatically assigned the "User" role.

### Creating Users in the Database (Advanced)

For manual user creation or role assignment, use the Entity Framework tools:

#### 1. Add User Migration Script

Create a file: `Scripts/add-user.sql`

```sql
-- This is automatically handled by ASP.NET Identity
-- Users should be created via the API or seeding code
```

#### 2. Create User Programmatically

Add to `DatabaseSeeder.cs`:

```csharp
var newUser = new ApplicationUser
{
    UserName = "customuser",
    Email = "custom@example.com",
    EmailConfirmed = true
};

var result = await userManager.CreateAsync(newUser, "Password@123");
if (result.Succeeded)
{
    await userManager.AddToRoleAsync(newUser, "User");
}
```

## Roles and Permissions

### Available Roles

- **Admin**: Full access to all API endpoints
- **User**: Standard user access

### Assigning Roles

Roles can be assigned:
1. During user registration (default: "User")
2. Programmatically via `UserManager.AddToRoleAsync()`
3. Via custom admin endpoints (not implemented in base version)

## Password Requirements

The API enforces the following password requirements:
- Minimum 6 characters
- At least one digit
- At least one lowercase letter
- At least one uppercase letter
- Non-alphanumeric characters are optional

## JWT Configuration

JWT settings are configured in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "Your-Secret-Key-Here-Minimum-32-Characters",
    "Issuer": "comaiz-api",
    "Audience": "comaiz-client",
    "ExpirationMinutes": 60
  }
}
```

### Production Configuration

⚠️ **Important**: For production:

1. Use environment variables or user secrets for the `SecretKey`:
   ```bash
   dotnet user-secrets set "JwtSettings:SecretKey" "your-production-secret-key"
   ```

2. Or use environment variables:
   ```bash
   export JwtSettings__SecretKey="your-production-secret-key"
   ```

3. Generate a strong secret key:
   ```bash
   openssl rand -base64 64
   ```

## Token Expiration

Tokens expire after 60 minutes by default. After expiration:
1. The API will return a 401 Unauthorized response
2. Users must login again to get a new token
3. Consider implementing refresh tokens for production use

## Troubleshooting

### "401 Unauthorized" Error

**Causes:**
- Token is missing
- Token is expired
- Token is invalid
- Token format is incorrect

**Solutions:**
1. Verify the token is included in the header: `Authorization: Bearer TOKEN`
2. Login again to get a fresh token
3. Check that the token hasn't been modified

### "JWT SecretKey must be configured" Error

**Cause:** The JWT secret key is not configured or is too short.

**Solution:** Ensure `JwtSettings:SecretKey` in `appsettings.json` or environment variables is at least 32 characters long.

### Login Returns 401

**Causes:**
- Incorrect username or password
- User doesn't exist

**Solutions:**
1. Verify credentials
2. Ensure the user was created (check database or use default users)
3. Reset password if needed

## Security Best Practices

1. **Use HTTPS**: Always use HTTPS in production
2. **Strong Secrets**: Use cryptographically strong secret keys (64+ characters)
3. **Environment Variables**: Store secrets in environment variables, not in code
4. **Token Expiration**: Keep token expiration times short (15-60 minutes)
5. **Secure Storage**: Don't store tokens in browser local storage for sensitive apps
6. **Change Default Passwords**: Immediately change default user passwords
7. **Implement Refresh Tokens**: For production, implement refresh token flow
8. **Rate Limiting**: Implement rate limiting on authentication endpoints
9. **Log Authentication**: Monitor and log authentication attempts
10. **Role-Based Access**: Use role-based authorization for sensitive operations

## Integration with Tests

Integration tests automatically authenticate using the test user:

```csharp
// In test setup
var token = await AuthHelper.GetAuthTokenAsync(client);
AuthHelper.SetAuthToken(client, token);

// Now all requests are authenticated
var response = await client.GetAsync("/api/clients");
```

## Next Steps

- Implement password reset functionality
- Add email confirmation
- Implement refresh tokens
- Add two-factor authentication
- Create admin panel for user management
- Add audit logging for authentication events
