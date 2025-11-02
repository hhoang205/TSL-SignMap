import { Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider, useAuth } from './contexts/AuthContext'
import ProtectedRoute from './components/ProtectedRoute'
import MainLayout from './components/Layout/MainLayout'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Users from './pages/Users'
import TrafficSigns from './pages/TrafficSigns'
import Contributions from './pages/Contributions'
import Votes from './pages/Votes'
import Wallets from './pages/Wallets'
import Payments from './pages/Payments'
import Notifications from './pages/Notifications'
import Feedback from './pages/Feedback'
import { ROUTES } from './utils/constants'

const AppRoutes = () => {
  const { isAuthenticated, loading } = useAuth()

  if (loading) {
    return <div>Loading...</div>
  }

  return (
    <Routes>
      <Route
        path={ROUTES.LOGIN}
        element={isAuthenticated ? <Navigate to={ROUTES.DASHBOARD} replace /> : <Login />}
      />
      <Route
        path={ROUTES.DASHBOARD}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Dashboard />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.USERS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Users />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.TRAFFIC_SIGNS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <TrafficSigns />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.CONTRIBUTIONS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Contributions />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.VOTES}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Votes />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.WALLETS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Wallets />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.PAYMENTS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Payments />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.NOTIFICATIONS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Notifications />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path={ROUTES.FEEDBACK}
        element={
          <ProtectedRoute>
            <MainLayout>
              <Feedback />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<Navigate to={ROUTES.DASHBOARD} replace />} />
    </Routes>
  )
}

const App = () => {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  )
}

export default App

