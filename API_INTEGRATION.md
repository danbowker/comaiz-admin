# API Integration Guide

This document describes how the React frontend integrates with the .NET backend API.

## API Endpoints

The backend provides the following RESTful API endpoints:

### Authentication

#### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "admin",
  "email": "admin@example.com",
  "roles": ["Admin"]
}
```

### Clients

- `GET /api/clients` - Get all clients
- `GET /api/clients/{id}` - Get client by ID
- `POST /api/clients` - Create new client
- `PUT /api/clients` - Update existing client
- `DELETE /api/clients/{id}` - Delete client

Example Client object:
```json
{
  "id": 1,
  "shortName": "ACME",
  "name": "ACME Corporation"
}
```

### Workers

- `GET /api/workers` - Get all workers
- `GET /api/workers/{id}` - Get worker by ID
- `POST /api/workers` - Create new worker
- `PUT /api/workers` - Update existing worker
- `DELETE /api/workers/{id}` - Delete worker

Example Worker object:
```json
{
  "id": 1,
  "name": "John Doe"
}
```

### Contracts

- `GET /api/contracts` - Get all contracts
- `GET /api/contracts/{id}` - Get contract by ID
- `POST /api/contracts` - Create new contract
- `PUT /api/contracts` - Update existing contract
- `DELETE /api/contracts/{id}` - Delete contract

Example Contract object:
```json
{
  "id": 1,
  "clientId": 1,
  "description": "Web Development Project",
  "price": 50000.00,
  "schedule": "2024-01-01 to 2024-12-31",
  "chargeType": 1
}
```

ChargeType enum:
- `0` - Fixed
- `1` - TimeAndMaterials

### Contract Rates

- `GET /api/contractrates` - Get all contract rates
- `GET /api/contractrates/{id}` - Get contract rate by ID
- `POST /api/contractrates` - Create new contract rate
- `PUT /api/contractrates` - Update existing contract rate
- `DELETE /api/contractrates/{id}` - Delete contract rate

Example ContractRate object:
```json
{
  "id": 1,
  "contractId": 1,
  "description": "Senior Developer Rate",
  "rate": 150.00
}
```

### Fixed Costs

- `GET /api/fixedcosts` - Get all fixed costs
- `GET /api/fixedcosts/{id}` - Get fixed cost by ID
- `POST /api/fixedcosts` - Create new fixed cost
- `PUT /api/fixedcosts` - Update existing fixed cost
- `DELETE /api/fixedcosts/{id}` - Delete fixed cost

Example FixedCost object:
```json
{
  "id": 1,
  "contractId": 1,
  "invoiceItemId": null,
  "name": "Server Hosting",
  "amount": 1000.00
}
```

### Work Records

- `GET /api/workrecords` - Get all work records
- `GET /api/workrecords/{id}` - Get work record by ID
- `POST /api/workrecords` - Create new work record
- `PUT /api/workrecords` - Update existing work record
- `DELETE /api/workrecords/{id}` - Delete work record

Example WorkRecord object:
```json
{
  "id": 1,
  "contractId": 1,
  "invoiceItemId": null,
  "startDate": "2024-01-01",
  "endDate": "2024-01-07",
  "hours": 40,
  "workerId": 1,
  "contractRateId": 1
}
```

### Invoices

- `GET /api/invoices` - Get all invoices
- `GET /api/invoices/{id}` - Get invoice by ID
- `POST /api/invoices` - Create new invoice
- `PUT /api/invoices` - Update existing invoice
- `DELETE /api/invoices/{id}` - Delete invoice

Example Invoice object:
```json
{
  "id": 1,
  "date": "2024-01-31",
  "purchaseOrder": "PO-2024-001",
  "clientId": 1
}
```

### Invoice Items

- `GET /api/invoiceitems` - Get all invoice items
- `GET /api/invoiceitems/{id}` - Get invoice item by ID
- `POST /api/invoiceitems` - Create new invoice item
- `PUT /api/invoiceitems` - Update existing invoice item
- `DELETE /api/invoiceitems/{id}` - Delete invoice item

Example InvoiceItem object:
```json
{
  "id": 1,
  "invoiceId": 1,
  "costId": 1,
  "quantity": 40,
  "unit": 1,
  "rate": 150.00,
  "vatRate": 0.20,
  "price": 6000.00
}
```

## Authentication

All endpoints except `/api/auth/login` require authentication.

### How Authentication Works

1. **Login**: User submits credentials to `/api/auth/login`
2. **Token Received**: Server returns JWT token
3. **Token Storage**: Frontend stores token in localStorage
4. **Token Usage**: Token is sent with every API request in the Authorization header
5. **Token Expiry**: When token expires (401 response), user is redirected to login

### Frontend Implementation

The frontend uses axios interceptors for automatic token management:

```typescript
// Request interceptor (in src/services/api.ts)
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  }
);

// Response interceptor
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expired - redirect to login
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);
```

## Error Handling

The API returns standard HTTP status codes:

- `200 OK` - Successful GET request
- `201 Created` - Successful POST request
- `204 No Content` - Successful PUT/DELETE request
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Missing or invalid authentication token
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### Frontend Error Handling

The frontend handles errors at multiple levels:

1. **Component Level**: Try/catch blocks in async operations
2. **Service Level**: Error interceptor in axios
3. **UI Level**: Error messages displayed to user

Example:
```typescript
try {
  await clientsService.create(newClient);
  // Success handling
} catch (err: any) {
  setError(err.response?.data?.message || 'Failed to create client');
}
```

## CORS

### Production
CORS is not required in production because the frontend and backend are served from the same origin (same Docker container).

### Development
When running frontend (port 3000) and backend (port 5057 or 7057) separately, CORS is required and automatically enabled.

The backend is configured with CORS support for development:
- Allows requests from `http://localhost:3000` and `https://localhost:3000`
- Allows all headers and methods
- Supports credentials (cookies, authorization headers)

This is automatically enabled when the backend runs in Development environment (`ASPNETCORE_ENVIRONMENT=Development`).

If you encounter CORS errors during local development:
1. Verify the backend is running in Development mode
2. Check the console output for CORS configuration
3. Ensure your `.env.local` has the correct backend URL
4. Restart both services after making configuration changes

## Data Validation

### Frontend Validation

The frontend performs basic validation:
- Required field checks
- Type validation (number, date, text)
- Form state management

### Backend Validation

The backend performs comprehensive validation:
- Model validation attributes
- Database constraints
- Business logic validation

Always rely on backend validation as the source of truth.

## Best Practices

### When Creating/Updating Entities

1. **Required Fields**: Check the entity model for required fields
2. **Foreign Keys**: Ensure related entities exist before creating dependent entities
3. **Dates**: Use ISO 8601 format (YYYY-MM-DD) for date fields
4. **Numbers**: Use proper numeric types (no strings for numbers)

### Recommended Creation Order

For a complete workflow:

1. Create **Clients**
2. Create **Workers**
3. Create **Contracts** (requires Client)
4. Create **Contract Rates** (requires Contract)
5. Create **Work Records** (requires Contract, Worker, optional ContractRate)
6. Create **Fixed Costs** (requires Contract)
7. Create **Invoices** (requires Client)
8. Create **Invoice Items** (requires Invoice, Cost)

### Error Recovery

If an API call fails:

1. Check the error message returned by the API
2. Verify all required fields are provided
3. Ensure foreign key references exist
4. Check authentication token is valid
5. Retry the operation if it was a temporary issue

## Testing API Endpoints

### Using Swagger UI

1. Navigate to `/swagger` on your backend
2. Click "Authorize" and enter: `Bearer YOUR_TOKEN`
3. Try out endpoints directly from the UI

### Using curl

```bash
# Get token
TOKEN=$(curl -X POST http://localhost:7057/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}' \
  | jq -r '.token')

# Use token to get clients
curl http://localhost:7057/api/clients \
  -H "Authorization: Bearer $TOKEN"
```

### Using the Frontend

The frontend provides the most user-friendly way to interact with the API. Simply:
1. Login at the root URL
2. Use the navigation to access different entity pages
3. Perform CRUD operations through the UI

## API Versioning

Currently, the API is at version 1 (implicit). Future versions may be added if breaking changes are introduced.

## Rate Limiting

There is currently no rate limiting implemented. Consider adding rate limiting for production deployments.

## Monitoring

Monitor API performance and errors through:
- Application logs
- Docker container logs
- Database query logs
- Frontend error tracking (browser console)

## Support

For API-related issues:
- Check Swagger documentation at `/swagger`
- Review backend logs
- Consult the [Authentication Guide](AUTHENTICATION.md)
- See entity models in `comaiz.data/Models/`
