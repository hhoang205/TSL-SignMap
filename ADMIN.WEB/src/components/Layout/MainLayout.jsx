import { Box } from '@mui/material'
import Sidebar from './Sidebar'
import Header from './Header'

const MainLayout = ({ children }) => {
  return (
    <Box sx={{ display: 'flex' }}>
      <Header />
      <Sidebar />
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          mt: 8,
          width: { sm: `calc(100% - 240px)` },
        }}
      >
        {children}
      </Box>
    </Box>
  )
}

export default MainLayout

