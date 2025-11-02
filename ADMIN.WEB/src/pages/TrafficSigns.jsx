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
  Select,
  MenuItem,
  FormControl,
  InputLabel,
} from '@mui/material'
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  LocationOn as LocationIcon,
} from '@mui/icons-material'
import { useQuery, useMutation, useQueryClient } from 'react-query'
import { trafficSignApi } from '../services/api'
import { useSnackbar } from 'notistack'
import { STATUS_OPTIONS } from '../utils/constants'

const TrafficSigns = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [filterStatus, setFilterStatus] = useState('')
  const [dialogOpen, setDialogOpen] = useState(false)
  const [selectedSign, setSelectedSign] = useState(null)
  const { enqueueSnackbar } = useSnackbar()
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery(
    ['trafficSigns', page, rowsPerPage],
    () => trafficSignApi.getAll().then((res) => res.data),
    {
      keepPreviousData: true,
    }
  )

  const signs = Array.isArray(data) ? data : []

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const filteredSigns = filterStatus
    ? signs.filter((sign) => sign.status === filterStatus)
    : signs

  const paginatedSigns = filteredSigns.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  if (error) {
    return <Typography color="error">Error loading traffic signs</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Traffic Signs</Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <FormControl size="small" sx={{ minWidth: 150 }}>
            <InputLabel>Filter Status</InputLabel>
            <Select
              value={filterStatus}
              label="Filter Status"
              onChange={(e) => setFilterStatus(e.target.value)}
            >
              <MenuItem value="">All</MenuItem>
              {STATUS_OPTIONS.TRAFFIC_SIGN.map((status) => (
                <MenuItem key={status} value={status}>
                  {status}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setDialogOpen(true)}>
            Add Sign
          </Button>
        </Box>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Location</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedSigns.map((sign) => (
              <TableRow key={sign.id}>
                <TableCell>{sign.id}</TableCell>
                <TableCell>{sign.type}</TableCell>
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <LocationIcon fontSize="small" />
                    {sign.latitude && sign.longitude
                      ? `${sign.latitude.toFixed(4)}, ${sign.longitude.toFixed(4)}`
                      : '-'}
                  </Box>
                </TableCell>
                <TableCell>
                  <Chip
                    label={sign.status}
                    size="small"
                    color={sign.status === 'Active' ? 'success' : 'default'}
                  />
                </TableCell>
                <TableCell>{new Date(sign.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>
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
        count={filteredSigns.length}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>Add Traffic Sign</DialogTitle>
        <DialogContent>
          <Typography>Traffic sign creation form will be implemented here</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={() => setDialogOpen(false)}>
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default TrafficSigns

