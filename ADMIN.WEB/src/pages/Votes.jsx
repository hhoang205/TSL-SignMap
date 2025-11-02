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
  TextField,
  InputAdornment,
  IconButton,
} from '@mui/material'
import { Search as SearchIcon, Visibility as VisibilityIcon } from '@mui/icons-material'
import { useQuery } from 'react-query'
import { voteApi } from '../services/api'

const Votes = () => {
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [searchTerm, setSearchTerm] = useState('')

  const { data, isLoading, error } = useQuery(
    ['votes', page, rowsPerPage],
    () => voteApi.getAll().then((res) => res.data),
    {
      keepPreviousData: true,
    }
  )

  const votes = Array.isArray(data) ? data : []

  const handleChangePage = (event, newPage) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const filteredVotes = searchTerm
    ? votes.filter(
        (vote) =>
          vote.username?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          vote.contributionId?.toString().includes(searchTerm)
      )
    : votes

  const paginatedVotes = filteredVotes.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)

  if (isLoading) {
    return <Typography>Loading...</Typography>
  }

  if (error) {
    return <Typography color="error">Error loading votes</Typography>
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Votes</Typography>
        <TextField
          placeholder="Search votes..."
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
              <TableCell>User</TableCell>
              <TableCell>Contribution ID</TableCell>
              <TableCell>Vote Type</TableCell>
              <TableCell>Weight</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedVotes.map((vote) => (
              <TableRow key={vote.id}>
                <TableCell>{vote.id}</TableCell>
                <TableCell>{vote.username || vote.userId}</TableCell>
                <TableCell>{vote.contributionId}</TableCell>
                <TableCell>
                  <Chip
                    label={vote.voteType}
                    size="small"
                    color={vote.voteType === 'Upvote' ? 'success' : 'error'}
                  />
                </TableCell>
                <TableCell>{vote.weight || 1}</TableCell>
                <TableCell>{new Date(vote.createdAt).toLocaleDateString()}</TableCell>
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
        count={filteredVotes.length}
        page={page}
        onPageChange={handleChangePage}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />
    </Box>
  )
}

export default Votes

