import { useState } from 'react'
import {
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  Typography,
  Container,
  Alert,
} from '@mui/material'
import { useAuth } from '../contexts/AuthContext'
import { useNavigate } from 'react-router-dom'
import { ROUTES } from '../utils/constants'
import TrafficIcon from '@mui/icons-material/Traffic'

const Login = () => {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const { login } = useAuth()
  const navigate = useNavigate()

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)

    const result = await login(username, password)

    if (result.success) {
      navigate(ROUTES.DASHBOARD)
    } else {
      setError(result.error || 'Login failed')
    }

    setLoading(false)
  }

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
        }}
      >
        <Card sx={{ width: '100%' }}>
          <CardContent sx={{ p: 4 }}>
            <Box sx={{ textAlign: 'center', mb: 3 }}>
              <TrafficIcon sx={{ fontSize: 60, color: 'primary.main', mb: 2 }} />
              <Typography variant="h4" component="h1" gutterBottom>
                TSL Admin Panel
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Traffic Sign Location Management System
              </Typography>
            </Box>

            {error && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {error}
              </Alert>
            )}

            <Box component="form" onSubmit={handleSubmit}>
              <TextField
                fullWidth
                label="Email or Username"
                variant="outlined"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
                sx={{ mb: 2 }}
                autoComplete="username"
              />
              <TextField
                fullWidth
                label="Password"
                type="password"
                variant="outlined"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                sx={{ mb: 3 }}
                autoComplete="current-password"
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                size="large"
                disabled={loading}
              >
                {loading ? 'Logging in...' : 'Login'}
              </Button>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Container>
  )
}

export default Login

