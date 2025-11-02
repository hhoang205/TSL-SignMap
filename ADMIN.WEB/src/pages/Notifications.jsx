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
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material'
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material'
import { useQuery, useMutation, useQueryClient } from 'react-query'
import { notificationApi } from '../services/api'
import { useSnackbar } from 'notistack'

const Notifications = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [formData, setFormData] = useState({
    title: '',
    message: '',
    userId: '',
  })
  const { enqueueSnackbar } = useSnackbar()
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery(
    ['notifications', page, rowsPerPage],
    () => notificationApi.getAll().then((res) => res.data),
    {
      keepPreviousData: true,
    }
  )

  const createMutation = useMutation(
    (data) => notificationApi.create(data),
    {
      onSuccess: () => {
        enqueueSnackbar('Notification created successfully', { variant: 'success' })
        queryClient.invalidateQueries('notifications')
        setDialogOpen(false)
        setFormData({ title: '', message: '', userId: '' })
      },
      onError: (error) => {
        enqueueSnackbar(error.response?.data?.message || 'Failed to create notification', {
          variant: 'error',
        })
      },
    }
  )

  const deleteMutation = useMutation(
    (id) => notificationApi.delete(id),
    {
      onSuccess: () => {
        enqueueSnackbar('Notification deleted successfully', { variant: 'success' })
        queryClient.invalidateQueries('notifications')
      },
      onError: (error) => {
        enqueueSnackbar(error.response?.data?.message || 'Failed to delete notification', {
          variant: 'error',
        })
      },
    }
  )

  const notifications = Array.isArray(data) ? data : []

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const handleCreate = () => {
    createMutation.mutate(formData)
  }

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  if (error) {
    return <Typography color="error">Error loading notifications</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Notifications</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => setDialogOpen(true)}
        >
          Create Notification
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Title</TableCell>
              <TableCell>Message</TableCell>
              <TableCell>User ID</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {notifications.map((notification) => (
              <TableRow key={notification.id}>
                <TableCell>{notification.id}</TableCell>
                <TableCell>{notification.title}</TableCell>
                <TableCell>{notification.message}</TableCell>
                <TableCell>{notification.userId || 'All'}</TableCell>
                <TableCell>{new Date(notification.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>
                  <IconButton size="small" color="default">
                    <EditIcon />
                  </IconButton>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={() => deleteMutation.mutate(notification.id)}
                  >
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
        count={notifications.length}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create Notification</DialogTitle>
        <DialogContent>
          <Box sx={{ mt: 2 }}>
            <TextField
              fullWidth
              label="Title"
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              sx={{ mb: 2 }}
            />
            <TextField
              fullWidth
              multiline
              rows={4}
              label="Message"
              value={formData.message}
              onChange={(e) => setFormData({ ...formData, message: e.target.value })}
              sx={{ mb: 2 }}
            />
            <TextField
              fullWidth
              label="User ID (optional, leave empty for all users)"
              type="number"
              value={formData.userId}
              onChange={(e) => setFormData({ ...formData, userId: e.target.value })}
              placeholder="Leave empty to send to all users"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleCreate}
            disabled={createMutation.isLoading || !formData.title || !formData.message}
          >
            {createMutation.isLoading ? 'Creating...' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default Notifications

