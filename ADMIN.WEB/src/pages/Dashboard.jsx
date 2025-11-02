import { useState, useEffect } from 'react'
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material'
import { useQuery } from 'react-query'
import {
  People as PeopleIcon,
  Traffic as TrafficIcon,
  Assignment as AssignmentIcon,
  AccountBalanceWallet as WalletIcon,
} from '@mui/icons-material'
import { walletApi, contributionApi, userApi, trafficSignApi } from '../services/api'
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts'

const StatCard = ({ title, value, icon: Icon, color }) => (
  <Card sx={{ height: '100%' }}>
    <CardContent>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Box>
          <Typography color="textSecondary" gutterBottom variant="body2">
            {title}
          </Typography>
          <Typography variant="h4">{value}</Typography>
        </Box>
        <Icon sx={{ fontSize: 48, color: color || 'primary.main' }} />
      </Box>
    </CardContent>
  </Card>
)

const Dashboard = () => {
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalSigns: 0,
    pendingContributions: 0,
    totalCoins: 0,
  })

  const { data: walletSummary, isLoading: walletLoading } = useQuery(
    'walletSummary',
    () => walletApi.getSummary().then((res) => res.data)
  )

  const { data: contributions, isLoading: contributionsLoading } = useQuery(
    'contributions',
    () => contributionApi.getAll().then((res) => res.data.data || res.data)
  )

  const { data: users, isLoading: usersLoading } = useQuery(
    'users',
    () => userApi.getAll().then((res) => res.data.data || res.data)
  )

  const { data: signs, isLoading: signsLoading } = useQuery(
    'signs',
    () => trafficSignApi.getAll().then((res) => res.data)
  )

  useEffect(() => {
    if (walletSummary) {
      setStats((prev) => ({
        ...prev,
        totalCoins: walletSummary.totalBalance || 0,
      }))
    }
    if (contributions) {
      const pending = Array.isArray(contributions)
        ? contributions.filter((c) => c.status === 'Pending').length
        : 0
      setStats((prev) => ({
        ...prev,
        pendingContributions: pending,
      }))
    }
    if (users) {
      const total = Array.isArray(users) ? users.length : 0
      setStats((prev) => ({
        ...prev,
        totalUsers: total,
      }))
    }
    if (signs) {
      const total = Array.isArray(signs) ? signs.length : 0
      setStats((prev) => ({
        ...prev,
        totalSigns: total,
      }))
    }
  }, [walletSummary, contributions, users, signs])

  const isLoading = walletLoading || contributionsLoading || usersLoading || signsLoading

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
        <CircularProgress />
      </Box>
    )
  }

  const recentContributions = Array.isArray(contributions)
    ? contributions.slice(0, 5).map((c) => ({
        id: c.id,
        user: c.username || 'Unknown',
        action: c.action,
        status: c.status,
        date: new Date(c.createdAt).toLocaleDateString(),
      }))
    : []

  return (
    <Box>
      <Typography variant="h4" gutterBottom sx={{ mb: 3 }}>
        Dashboard
      </Typography>

      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Users"
            value={stats.totalUsers}
            icon={PeopleIcon}
            color="primary.main"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Traffic Signs"
            value={stats.totalSigns}
            icon={TrafficIcon}
            color="success.main"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Pending Contributions"
            value={stats.pendingContributions}
            icon={AssignmentIcon}
            color="warning.main"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Coins"
            value={stats.totalCoins.toFixed(0)}
            icon={WalletIcon}
            color="info.main"
          />
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Recent Contributions
              </Typography>
              <TableContainer>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>ID</TableCell>
                      <TableCell>User</TableCell>
                      <TableCell>Action</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Date</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {recentContributions.length > 0 ? (
                      recentContributions.map((row) => (
                        <TableRow key={row.id}>
                          <TableCell>{row.id}</TableCell>
                          <TableCell>{row.user}</TableCell>
                          <TableCell>{row.action}</TableCell>
                          <TableCell>{row.status}</TableCell>
                          <TableCell>{row.date}</TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={5} align="center">
                          No recent contributions
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                System Statistics
              </Typography>
              <Box sx={{ mt: 2 }}>
                <Typography variant="body2" color="textSecondary">
                  Wallet Statistics
                </Typography>
                {walletSummary && (
                  <Box sx={{ mt: 1 }}>
                    <Typography variant="body1">
                      Total Wallets: {walletSummary.totalWallets || 0}
                    </Typography>
                    <Typography variant="body1">
                      Average Balance: {(walletSummary.averageBalance || 0).toFixed(2)} coins
                    </Typography>
                    <Typography variant="body1">
                      Zero Balance: {walletSummary.walletsWithZeroBalance || 0}
                    </Typography>
                  </Box>
                )}
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  )
}

export default Dashboard

