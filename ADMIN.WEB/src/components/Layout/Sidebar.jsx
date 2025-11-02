import { List, ListItem, ListItemButton, ListItemIcon, ListItemText, Drawer, Box, Typography } from '@mui/material'
import { useNavigate, useLocation } from 'react-router-dom'
import DashboardIcon from '@mui/icons-material/Dashboard'
import PeopleIcon from '@mui/icons-material/People'
import TrafficIcon from '@mui/icons-material/Traffic'
import AssignmentIcon from '@mui/icons-material/Assignment'
import HowToVoteIcon from '@mui/icons-material/HowToVote'
import AccountBalanceWalletIcon from '@mui/icons-material/AccountBalanceWallet'
import PaymentIcon from '@mui/icons-material/Payment'
import NotificationsIcon from '@mui/icons-material/Notifications'
import FeedbackIcon from '@mui/icons-material/Feedback'
import { ROUTES } from '../../utils/constants'

const drawerWidth = 240

const menuItems = [
  { text: 'Dashboard', icon: DashboardIcon, path: ROUTES.DASHBOARD },
  { text: 'Users', icon: PeopleIcon, path: ROUTES.USERS },
  { text: 'Traffic Signs', icon: TrafficIcon, path: ROUTES.TRAFFIC_SIGNS },
  { text: 'Contributions', icon: AssignmentIcon, path: ROUTES.CONTRIBUTIONS },
  { text: 'Votes', icon: HowToVoteIcon, path: ROUTES.VOTES },
  { text: 'Wallets', icon: AccountBalanceWalletIcon, path: ROUTES.WALLETS },
  { text: 'Payments', icon: PaymentIcon, path: ROUTES.PAYMENTS },
  { text: 'Notifications', icon: NotificationsIcon, path: ROUTES.NOTIFICATIONS },
  { text: 'Feedback', icon: FeedbackIcon, path: ROUTES.FEEDBACK },
]

const Sidebar = () => {
  const navigate = useNavigate()
  const location = useLocation()

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
        },
      }}
    >
      <Box sx={{ p: 2, borderBottom: '1px solid #e0e0e0' }}>
        <Typography variant="h6" component="div" sx={{ fontWeight: 600 }}>
          TSL Admin Panel
        </Typography>
      </Box>
      <List>
        {menuItems.map((item) => {
          const Icon = item.icon
          const isActive = location.pathname === item.path
          
          return (
            <ListItem key={item.text} disablePadding>
              <ListItemButton
                selected={isActive}
                onClick={() => navigate(item.path)}
                sx={{
                  '&.Mui-selected': {
                    backgroundColor: 'primary.main',
                    color: 'white',
                    '&:hover': {
                      backgroundColor: 'primary.dark',
                    },
                    '& .MuiListItemIcon-root': {
                      color: 'white',
                    },
                  },
                }}
              >
                <ListItemIcon sx={{ color: isActive ? 'white' : 'inherit' }}>
                  <Icon />
                </ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItemButton>
            </ListItem>
          )
        })}
      </List>
    </Drawer>
  )
}

export default Sidebar

