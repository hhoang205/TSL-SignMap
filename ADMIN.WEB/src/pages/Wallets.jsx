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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
} from '@mui/material'
import { Add as AddIcon, AccountBalanceWallet as WalletIcon } from '@mui/icons-material'
import { useQuery, useMutation, useQueryClient } from 'react-query'
import { walletApi } from '../services/api'
import { useSnackbar } from 'notistack'

const Wallets = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [adjustDialogOpen, setAdjustDialogOpen] = useState(false)
  const [selectedUserId, setSelectedUserId] = useState(null)
  const [adjustmentType, setAdjustmentType] = useState('Credit')
  const [amount, setAmount] = useState('')
  const [reason, setReason] = useState('')
  const { enqueueSnackbar } = useSnackbar()
  const queryClient = useQueryClient()

  const { data: walletsData, isLoading } = useQuery(
    ['wallets', page, rowsPerPage],
    () => walletApi.filter({ pageNumber: page + 1, pageSize: rowsPerPage }).then((res) => res.data),
    {
      keepPreviousData: true,
    }
  )

  const { data: summaryData } = useQuery('walletSummary', () =>
    walletApi.getSummary().then((res) => res.data)
  )

  const wallets = Array.isArray(walletsData) ? walletsData : []

  const adjustMutation = useMutation(
    (data) => walletApi.adjust(selectedUserId, data),
    {
      onSuccess: () => {
        enqueueSnackbar('Coin adjustment successful', { variant: 'success' })
        queryClient.invalidateQueries('wallets')
        queryClient.invalidateQueries('walletSummary')
        setAdjustDialogOpen(false)
        setAmount('')
        setReason('')
      },
      onError: (error) => {
        enqueueSnackbar(
          error.response?.data?.message || 'Failed to adjust coins',
          { variant: 'error' }
        )
      },
    }
  )

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const handleOpenAdjustDialog = (userId) => {
    setSelectedUserId(userId)
    setAdjustDialogOpen(true)
  }

  const handleSubmitAdjust = () => {
    if (!amount || parseFloat(amount) <= 0) {
      enqueueSnackbar('Please enter a valid amount', { variant: 'error' })
      return
    }

    adjustMutation.mutate({
      amount: parseFloat(amount),
      adjustmentType,
      reason: reason || 'Admin adjustment',
    })
  }

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Coin Wallets</Typography>
        {summaryData && (
          <Box sx={{ display: 'flex', gap: 2 }}>
            <Chip label={`Total: ${summaryData.totalBalance || 0} coins`} color="primary" />
            <Chip label={`Wallets: ${summaryData.totalWallets || 0}`} color="secondary" />
          </Box>
        )}
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>User ID</TableCell>
              <TableCell>Username</TableCell>
              <TableCell>Balance</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {wallets.map((wallet) => (
              <TableRow key={wallet.id}>
                <TableCell>{wallet.id}</TableCell>
                <TableCell>{wallet.userId}</TableCell>
                <TableCell>{wallet.username || '-'}</TableCell>
                <TableCell>
                  <Chip
                    label={`${wallet.balance || 0} coins`}
                    color={wallet.balance > 100 ? 'success' : wallet.balance > 10 ? 'info' : 'default'}
                  />
                </TableCell>
                <TableCell>{new Date(wallet.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>
                  <Button
                    size="small"
                    variant="outlined"
                    startIcon={<WalletIcon />}
                    onClick={() => handleOpenAdjustDialog(wallet.userId)}
                  >
                    Adjust
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={wallets.length}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />

      <Dialog
        open={adjustDialogOpen}
        onClose={() => setAdjustDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Adjust Coins</DialogTitle>
        <DialogContent>
          <Box sx={{ mt: 2 }}>
            <FormControl fullWidth sx={{ mb: 2 }}>
              <InputLabel>Adjustment Type</InputLabel>
              <Select
                value={adjustmentType}
                label="Adjustment Type"
                onChange={(e) => setAdjustmentType(e.target.value)}
              >
                <MenuItem value="Credit">Credit (Add)</MenuItem>
                <MenuItem value="Debit">Debit (Subtract)</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Amount"
              type="number"
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              sx={{ mb: 2 }}
              inputProps={{ min: 0.01, step: 0.01 }}
            />
            <TextField
              fullWidth
              multiline
              rows={3}
              label="Reason"
              value={reason}
              onChange={(e) => setReason(e.target.value)}
              placeholder="Optional reason for this adjustment..."
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAdjustDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleSubmitAdjust}
            disabled={adjustMutation.isLoading}
            color={adjustmentType === 'Credit' ? 'success' : 'error'}
          >
            {adjustMutation.isLoading ? 'Processing...' : adjustmentType}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default Wallets

