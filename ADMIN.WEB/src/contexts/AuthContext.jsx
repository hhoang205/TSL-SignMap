import { createContext, useContext, useState, useEffect } from 'react'
import api from '../utils/api'
import { STORAGE_KEYS } from '../utils/constants'

const AuthContext = createContext(null)

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return context
}

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const storedUser = localStorage.getItem(STORAGE_KEYS.USER)
    const storedToken = localStorage.getItem(STORAGE_KEYS.TOKEN)
    
    if (storedUser && storedToken) {
      try {
        setUser(JSON.parse(storedUser))
      } catch (error) {
        console.error('Error parsing stored user:', error)
        localStorage.removeItem(STORAGE_KEYS.USER)
        localStorage.removeItem(STORAGE_KEYS.TOKEN)
      }
    }
    setLoading(false)
  }, [])

  const login = async (username, password) => {
    try {
      const response = await api.post('/users/login', {
        email: username, // Backend expects 'email' not 'username'
        password,
      })
      
      // Response format: { User: UserDto, Token: string }
      const { User, Token } = response.data.data || response.data
      
      if (Token && User) {
        localStorage.setItem(STORAGE_KEYS.TOKEN, Token)
        localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(User))
        setUser(User)
        return { success: true }
      }
      
      throw new Error('Invalid response from server')
    } catch (error) {
      const message = error.response?.data?.message || error.message || 'Login failed'
      return { success: false, error: message }
    }
  }

  const logout = () => {
    localStorage.removeItem(STORAGE_KEYS.TOKEN)
    localStorage.removeItem(STORAGE_KEYS.USER)
    setUser(null)
  }

  const value = {
    user,
    login,
    logout,
    isAuthenticated: !!user,
    loading,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

