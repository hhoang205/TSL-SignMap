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
} from '@mui/material'
import { Visibility as VisibilityIcon } from '@mui/icons-material'
import { useQuery } from 'react-query'
import { paymentApi } from '../services/api'
import { STATUS_OPTIONS } from '../utils/constants'

const Payments = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [filterStatus, setFilterStatus] = useState('')

  const { data, isLoading, error } = useQuery(
    ['payments', page, rowsPerPage, filterStatus],
    () => {
      if (filterStatus) {
        return paymentApi.getByStatus(filterStatus).then((res) => res.data)
      }
      return paymentApi.getAll().then((res) => res.data)
    },
    {
      keepPreviousData: true,
    }
  )

  const payments = Array.isArray(data) ? data : []

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const filteredPayments = filterStatus
    ? payments.filter((p) => p.status === filterStatus)
    : payments

  const paginatedPayments = filteredPayments.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  )

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  if (error) {
    return <Typography color="error">Error loading payments</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Payments</Typography>
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
            {STATUS_OPTIONS.PAYMENT.map((status) => (
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
              <TableCell>Amount</TableCell>
              <TableCell>Coins</TableCell>
              <TableCell>Payment Method</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedPayments.map((payment) => (
              <TableRow key={payment.id}>
                <TableCell>{payment.id}</TableCell>
                <TableCell>{payment.username || payment.userId}</TableCell>
                <TableCell>${payment.amount || 0}</TableCell>
                <TableCell>{payment.coinAmount || 0} coins</TableCell>
                <TableCell>{payment.paymentMethod || '-'}</TableCell>
                <TableCell>
                  <Chip
                    label={payment.status}
                    size="small"
                    color={
                      payment.status === 'Completed'
                        ? 'success'
                        : payment.status === 'Failed'
                        ? 'error'
                        : 'warning'
                    }
                  />
                </TableCell>
                <TableCell>{new Date(payment.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>
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
        count={filteredPayments.length}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />
    </Box>
  )
}

export default Payments

