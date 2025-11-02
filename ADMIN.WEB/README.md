# TSL Admin Panel - React Frontend

This is the admin web interface for the Traffic Sign Location Management System (TSL).

## Features

- **Dashboard**: Overview of system statistics and recent activity
- **User Management**: View and manage user accounts
- **Traffic Signs**: Manage traffic signs in the system
- **Contributions**: Review and approve/reject user contributions
- **Votes**: View voting history and statistics
- **Coin Wallets**: Manage user coin balances and adjustments
- **Payments**: Monitor payment transactions
- **Notifications**: Create and manage system notifications
- **Feedback**: View and manage user feedback

## Technology Stack

- **React 18** - UI library
- **Vite** - Build tool and dev server
- **Material-UI (MUI)** - Component library
- **React Router** - Routing
- **React Query** - Data fetching and caching
- **Axios** - HTTP client
- **Recharts** - Charts and graphs
- **Notistack** - Toast notifications

## Setup

1. Install dependencies:
```bash
npm install
```

2. Configure API endpoint in `.env` file:
```
VITE_API_BASE_URL=http://localhost:5000/api
```

3. Run development server:
```bash
npm run dev
```

4. Build for production:
```bash
npm run build
```

## Project Structure

```
ADMIN.WEB/
├── src/
│   ├── components/       # Reusable components
│   │   └── Layout/       # Layout components (Header, Sidebar, MainLayout)
│   ├── pages/           # Page components
│   ├── services/        # API service functions
│   ├── contexts/        # React contexts (Auth, etc.)
│   ├── utils/           # Utility functions and constants
│   ├── styles/          # Theme and styling
│   ├── App.jsx          # Main app component with routing
│   └── main.jsx         # Entry point
├── public/              # Static assets
├── package.json         # Dependencies
├── vite.config.js       # Vite configuration
└── README.md
```

## API Integration

The frontend communicates with the backend API at the base URL specified in `.env`. All API calls are handled through:
- `src/utils/api.js` - Axios instance with interceptors
- `src/services/api.js` - API service functions

## Authentication

The app uses JWT tokens stored in localStorage. The `AuthContext` manages authentication state and provides login/logout functionality.

## Environment Variables

- `VITE_API_BASE_URL` - Base URL for the backend API (default: http://localhost:5000/api)

