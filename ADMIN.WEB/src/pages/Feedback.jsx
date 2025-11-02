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
  Chip,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
} from '@mui/material'
import { Visibility as VisibilityIcon } from '@mui/icons-material'
import { useQuery } from 'react-query'
import { feedbackApi } from '../services/api'
import { STATUS_OPTIONS } from '../utils/constants'

const Feedback = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [filterStatus, setFilterStatus] = useState('')
  const [selectedFeedback, setSelectedFeedback] = useState(null)
  const [dialogOpen, setDialogOpen] = useState(false)

  const { data, isLoading, error } = useQuery(
    ['feedbacks', page, rowsPerPage, filterStatus],
    () => feedbackApi.getAll().then((res) => res.data),
    {
      keepPreviousData: true,
    }
  )

  const feedbacks = Array.isArray(data) ? data : []

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const filteredFeedbacks = filterStatus
    ? feedbacks.filter((f) => f.status === filterStatus)
    : feedbacks

  const paginatedFeedbacks = filteredFeedbacks.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  )

  const handleViewFeedback = (feedback) => {
    setSelectedFeedback(feedback)
    setDialogOpen(true)
  }

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  if (error) {
    return <Typography color="error">Error loading feedback</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Feedback</Typography>
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
            {STATUS_OPTIONS.FEEDBACK.map((status) => (
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
              <TableCell>Content</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedFeedbacks.map((feedback) => (
              <TableRow key={feedback.id}>
                <TableCell>{feedback.id}</TableCell>
                <TableCell>{feedback.username || feedback.userId}</TableCell>
                <TableCell>
                  {feedback.content?.length > 50
                    ? `${feedback.content.substring(0, 50)}...`
                    : feedback.content}
                </TableCell>
                <TableCell>
                  <Chip
                    label={feedback.status}
                    size="small"
                    color={
                      feedback.status === 'Resolved'
                        ? 'success'
                        : feedback.status === 'Dismissed'
                        ? 'error'
                        : 'warning'
                    }
                  />
                </TableCell>
                <TableCell>{new Date(feedback.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>
                  <IconButton
                    size="small"
                    color="default"
                    onClick={() => handleViewFeedback(feedback)}
                  >
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
        count={filteredFeedbacks.length}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>Feedback Details</DialogTitle>
        <DialogContent>
          {selectedFeedback && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                ID: {selectedFeedback.id}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                User: {selectedFeedback.username || selectedFeedback.userId}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Status: {selectedFeedback.status}
              </Typography>
              <Typography variant="body2" color="textSecondary" gutterBottom>
                Created: {new Date(selectedFeedback.createdAt).toLocaleString()}
              </Typography>
              <Typography variant="body1" sx={{ mt: 2 }}>
                {selectedFeedback.content}
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default Feedback

