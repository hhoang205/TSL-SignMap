import { useState } from 'react'
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Button,
  Chip,
  IconButton,
  TextField,
  InputAdornment,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material'
import {
  Search as SearchIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  AccountBalanceWallet as WalletIcon,
} from '@mui/icons-material'
import { useQuery, useMutation, useQueryClient } from 'react-query'
import { userApi, walletApi } from '../services/api'
import { useSnackbar } from 'notistack'

const Users = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedUser, setSelectedUser] = useState(null)
  const [walletDialogOpen, setWalletDialogOpen] = useState(false)
  const { enqueueSnackbar } = useSnackbar()
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery(
    ['users', page, rowsPerPage, searchTerm],
    () => {
      const params = {
        pageNumber: page + 1,
        pageSize: rowsPerPage,
        ...(searchTerm && { username: searchTerm }),
      }
      return userApi.getAll(params).then((res) => res.data)
    }
  )

  const users = data?.data || []
  const totalCount = data?.count || 0

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const handleOpenWalletDialog = async (userId) => {
    try {
      const response = await walletApi.getByUserId(userId)
      setSelectedUser({ userId, ...response.data })
      setWalletDialogOpen(true)
    } catch (error) {
      enqueueSnackbar('Failed to load wallet information', { variant: 'error' })
    }
  }

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  if (error) {
    return <Typography color="error">Error loading users</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Users</Typography>
        <TextField
          placeholder="Search users..."
          variant="outlined"
          size="small"
          value={searchTerm}
          onChange={(e) => {
            setSearchTerm(e.target.value)
            setPage(0)
          }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
          sx={{ minWidth: 300 }}
        />
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Username</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Full Name</TableCell>
              <TableCell>Phone</TableCell>
              <TableCell>Role</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((user) => (
              <TableRow key={user.id}>
                <TableCell>{user.id}</TableCell>
                <TableCell>{user.username}</TableCell>
                <TableCell>{user.email}</TableCell>
                <TableCell>
                  {user.firstname} {user.lastname}
                </TableCell>
                <TableCell>{user.phoneNumber || '-'}</TableCell>
                <TableCell>
                  <Chip
                    label={user.role || 'User'}
                    size="small"
                    color={user.role === 'Admin' ? 'error' : 'default'}
                  />
                </TableCell>
                <TableCell>
                  <IconButton
                    size="small"
                    onClick={() => handleOpenWalletDialog(user.id)}
                    color="primary"
                  >
                    <WalletIcon />
                  </IconButton>
                  <IconButton size="small" color="default">
                    <EditIcon />
                  </IconButton>
                  <IconButton size="small" color="error">
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={totalCount}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />

      {/* Wallet Dialog */}
      <Dialog open={walletDialogOpen} onClose={() => setWalletDialogOpen(false)} maxWidth="sm">
        <DialogTitle>Wallet Information</DialogTitle>
        <DialogContent>
          {selectedUser && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="body1">
                <strong>User ID:</strong> {selectedUser.userId}
              </Typography>
              <Typography variant="body1">
                <strong>Balance:</strong> {selectedUser.balance || 0} coins
              </Typography>
              <Typography variant="body2" color="textSecondary">
                Created: {new Date(selectedUser.createdAt).toLocaleString()}
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setWalletDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default Users

