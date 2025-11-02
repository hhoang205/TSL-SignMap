import { Navigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { Box, CircularProgress } from '@mui/material'
import { ROUTES } from '../utils/constants'

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth()

  if (loading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh',
        }}
      >
        <CircularProgress />
      </Box>
    )
  }

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} replace />
  }

  return children
}

export default ProtectedRoute

