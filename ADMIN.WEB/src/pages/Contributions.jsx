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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextareaAutosize,
} from '@mui/material'
import {
  Check as CheckIcon,
  Close as CloseIcon,
  Visibility as VisibilityIcon,
} from '@mui/icons-material'
import { useQuery, useMutation, useQueryClient } from 'react-query'
import { contributionApi } from '../services/api'
import { useSnackbar } from 'notistack'
import { STATUS_OPTIONS } from '../utils/constants'

const Contributions = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [filterStatus, setFilterStatus] = useState('')
  const [dialogOpen, setDialogOpen] = useState(false)
  const [selectedContribution, setSelectedContribution] = useState(null)
  const [actionType, setActionType] = useState('')
  const [adminNote, setAdminNote] = useState('')
  const { enqueueSnackbar } = useSnackbar()
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery(
    ['contributions', page, rowsPerPage, filterStatus],
    () => {
      if (filterStatus) {
        return contributionApi.getByStatus(filterStatus).then((res) => res.data)
      }
      return contributionApi.getAll().then((res) => res.data.data || res.data)
    },
    {
      keepPreviousData: true,
    }
  )

  const approveMutation = useMutation(
    (id) => contributionApi.approve(id, { adminNote }),
    {
      onSuccess: () => {
        enqueueSnackbar('Contribution approved successfully', { variant: 'success' })
        queryClient.invalidateQueries('contributions')
        setDialogOpen(false)
      },
      onError: (error) => {
        enqueueSnackbar(error.response?.data?.message || 'Failed to approve contribution', {
          variant: 'error',
        })
      },
    }
  )

  const rejectMutation = useMutation(
    (id) => contributionApi.reject(id, { adminNote }),
    {
      onSuccess: () => {
        enqueueSnackbar('Contribution rejected', { variant: 'success' })
        queryClient.invalidateQueries('contributions')
        setDialogOpen(false)
      },
      onError: (error) => {
        enqueueSnackbar(error.response?.data?.message || 'Failed to reject contribution', {
          variant: 'error',
        })
      },
    }
  )

  const contributions = Array.isArray(data) ? data : []

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const handleApprove = (contribution) => {
    setSelectedContribution(contribution)
    setActionType('approve')
    setAdminNote('')
    setDialogOpen(true)
  }

  const handleReject = (contribution) => {
    setSelectedContribution(contribution)
    setActionType('reject')
    setAdminNote('')
    setDialogOpen(true)
  }

  const handleSubmitAction = () => {
    if (actionType === 'approve') {
      approveMutation.mutate(selectedContribution.id)
    } else if (actionType === 'reject') {
      rejectMutation.mutate(selectedContribution.id)
    }
  }

  const filteredContributions = filterStatus
    ? contributions
    : contributions.filter((c) => c.status === filterStatus || !filterStatus)

  const paginatedContributions = filteredContributions.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  )

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  if (error) {
    return <Typography color="error">Error loading contributions</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Contributions</Typography>
        <FormControl size="small" sx={{ minWidth: 200 }}>
          <InputLabel>Filter Status</InputLabel>
          <Select
            value={filterStatus}
            label="Filter Status"
            onChange={(e) => {
              setFilterStatus(e.target.value)
              setPage(0)
            }}
          >
            <MenuItem value="">All</MenuItem>
            {STATUS_OPTIONS.CONTRIBUTION.map((status) => (
              <MenuItem key={status} value={status}>
                {status}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>User</TableCell>
              <TableCell>Action</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedContributions.map((contribution) => (
              <TableRow key={contribution.id}>
                <TableCell>{contribution.id}</TableCell>
                <TableCell>{contribution.username || contribution.userId}</TableCell>
                <TableCell>{contribution.action}</TableCell>
                <TableCell>{contribution.type || '-'}</TableCell>
                <TableCell>
                  <Chip
                    label={contribution.status}
                    size="small"
                    color={
                      contribution.status === 'Approved'
                        ? 'success'
                        : contribution.status === 'Rejected'
                        ? 'error'
                        : 'warning'
                    }
                  />
                </TableCell>
                <TableCell>{new Date(contribution.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>
                  {contribution.status === 'Pending' && (
                    <>
                      <IconButton
                        size="small"
                        color="success"
                        onClick={() => handleApprove(contribution)}
                      >
                        <CheckIcon />
                      </IconButton>
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleReject(contribution)}
                      >
                        <CloseIcon />
                      </IconButton>
                    </>
                  )}
                  <IconButton size="small" color="default">
                    <VisibilityIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={filteredContributions.length}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>
          {actionType === 'approve' ? 'Approve' : 'Reject'} Contribution
        </DialogTitle>
        <DialogContent>
          {selectedContribution && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Contribution ID: {selectedContribution.id}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Action: {selectedContribution.action}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Description: {selectedContribution.description || '-'}
              </Typography>
              <TextField
                fullWidth
                multiline
                rows={4}
                label="Admin Note"
                value={adminNote}
                onChange={(e) => setAdminNote(e.target.value)}
                sx={{ mt: 2 }}
                placeholder="Add a note for this action..."
              />
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            color={actionType === 'approve' ? 'success' : 'error'}
            onClick={handleSubmitAction}
            disabled={approveMutation.isLoading || rejectMutation.isLoading}
          >
            {actionType === 'approve' ? 'Approve' : 'Reject'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default Contributions

