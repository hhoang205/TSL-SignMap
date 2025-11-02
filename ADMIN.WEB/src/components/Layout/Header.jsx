import { AppBar, Toolbar, Typography, Box, Button, Avatar } from '@mui/material'
import { useAuth } from '../../contexts/AuthContext'
import { useNavigate } from 'react-router-dom'
import LogoutIcon from '@mui/icons-material/Logout'

const Header = () => {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Traffic Sign Location Management System
        </Typography>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Avatar sx={{ bgcolor: 'secondary.main' }}>
            {user?.username?.[0]?.toUpperCase() || 'A'}
          </Avatar>
          <Typography variant="body1">{user?.username || 'Admin'}</Typography>
          <Button
            color="inherit"
            startIcon={<LogoutIcon />}
            onClick={handleLogout}
            sx={{ ml: 2 }}
          >
            Logout
          </Button>
        </Box>
      </Toolbar>
    </AppBar>
  )
}

export default Header

