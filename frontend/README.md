# Comaiz Admin Frontend

React-based frontend for the Comaiz Admin application, providing a full CRUD interface for managing a small consultancy business.

## Features

- **User Authentication**: JWT-based login with token management
- **Entity Management**: Full CRUD operations for:
  - Clients
  - Workers
  - Contracts
  - Contract Rates
  - Fixed Costs
  - Work Records
  - Invoices
  - Invoice Items
- **Responsive Design**: Works on desktop and mobile devices
- **Real-time Validation**: Form validation and error handling
- **Secure**: Automatic token refresh and secure API communication

## Technology Stack

- React 18 with TypeScript
- React Router for navigation
- Axios for API communication
- CSS for styling

## Getting Started

### Prerequisites

- Node.js 20 or higher
- npm or yarn

### Installation

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

### Development

To run the frontend in development mode:

```bash
npm start
```

This will start the development server at `http://localhost:3000`.

**Note**: You'll need the backend API running at `http://localhost:7057` or configure the `REACT_APP_API_URL` environment variable.

### Configuration

Create a `.env.local` file in the `frontend` directory to configure the API endpoint:

```
REACT_APP_API_URL=http://localhost:7057/api
```

For production, the frontend will use `/api` as the base URL, expecting to be served from the same domain as the backend.

### Building for Production

To create a production build:

```bash
npm run build
```

The build artifacts will be stored in the `build/` directory. These are automatically copied to the backend's `wwwroot` folder during Docker build.

## Project Structure

```
frontend/
├── public/             # Static files
├── src/
│   ├── components/     # React components
│   │   ├── auth/       # Authentication components
│   │   ├── entities/   # Generic entity management components
│   │   ├── Layout.tsx  # Main layout with navigation
│   │   └── Layout.css
│   ├── contexts/       # React contexts (Auth)
│   ├── pages/          # Page components for each entity
│   ├── services/       # API services
│   ├── types/          # TypeScript type definitions
│   ├── App.tsx         # Main app component with routing
│   └── index.tsx       # Entry point
└── package.json
```

## Usage

### Login

1. Navigate to the application (default: `http://localhost:3000` in dev, or your deployed URL)
2. You'll be redirected to the login page
3. Use one of the default credentials:
   - Username: `admin`, Password: `Admin@123`
   - Username: `testuser`, Password: `Test@123`

### Managing Entities

After logging in, use the navigation menu to access different entity management pages. Each page provides:

- **List View**: View all entities with pagination
- **Create**: Click "Create New" to add a new entity
- **Edit**: Click "Edit" on any row to modify an entity
- **Delete**: Click "Delete" to remove an entity (with confirmation)

### Entity Relationships

Some entities have relationships with others:
- **Contracts** require a Client
- **Contract Rates** require a Contract
- **Work Records** require a Contract and Worker
- **Invoices** require a Client
- **Invoice Items** require an Invoice and Cost

Make sure to create parent entities before creating dependent entities.

## API Integration

The frontend communicates with the backend API using Axios. The API client is configured in `src/services/api.ts` with:

- Automatic JWT token attachment to requests
- Automatic redirect to login on 401 errors
- Request/response interceptors for error handling

## Authentication

The application uses JWT tokens for authentication:

1. Login credentials are sent to `/api/auth/login`
2. The token is stored in localStorage
3. The token is automatically included in all API requests
4. On token expiration (401 error), the user is redirected to login

## Development Notes

### Known Dependencies Issues

The React app uses create-react-app which has some known vulnerabilities in development dependencies. These do not affect production builds and are related to webpack-dev-server and other dev-only packages. The vulnerabilities are:

- `nth-check` - Development only, no production impact
- `postcss` - Development only, no production impact  
- `webpack-dev-server` - Development only, no production impact

To address these in the future, consider migrating to Vite or another modern build tool.

### Adding New Entities

To add a new entity to the frontend:

1. Add the entity type to `src/types/index.ts`
2. Create a service in `src/services/entityService.ts`
3. Create a page component in `src/pages/` (following the pattern of existing pages)
4. Add a route in `src/App.tsx`
5. Add a navigation link in `src/components/Layout.tsx`

## Testing

To run tests:

```bash
npm test
```

## Contributing

When contributing to the frontend:

1. Follow the existing code style and patterns
2. Use TypeScript for type safety
3. Add proper error handling
4. Test your changes locally before committing

## License

This project is part of the Comaiz Admin application.
