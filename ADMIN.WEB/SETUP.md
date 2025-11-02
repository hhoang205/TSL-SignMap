# Setup Instructions for TSL Admin Panel

## Prerequisites

- Node.js 16+ and npm
- Backend API running on http://localhost:5000 (or update `.env` file)

## Installation Steps

1. **Navigate to the admin web folder:**
```bash
cd ADMIN.WEB
```

2. **Install dependencies:**
```bash
npm install
```

3. **Create `.env` file:**
Create a `.env` file in the `ADMIN.WEB` folder with the following content:
```
VITE_API_BASE_URL=http://localhost:5000/api
```

4. **Start development server:**
```bash
npm run dev
```

The admin panel will be available at http://localhost:3000

## Build for Production

```bash
npm run build
```

The built files will be in the `dist` folder.

## Environment Variables

- `VITE_API_BASE_URL`: Base URL for the backend API (default: http://localhost:5000/api)

## Default Login

You need to have admin credentials set up in the backend. Contact your system administrator for login credentials.

## Troubleshooting

### Port Already in Use
If port 3000 is already in use, Vite will automatically try the next available port.

### API Connection Issues
- Ensure the backend API is running
- Check that the API URL in `.env` is correct
- Verify CORS is configured on the backend

### Build Errors
- Clear `node_modules` and reinstall: `rm -rf node_modules && npm install`
- Check Node.js version: should be 16 or higher

