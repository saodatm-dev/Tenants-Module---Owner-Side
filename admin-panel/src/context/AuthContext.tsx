import { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import { useNavigate } from 'react-router-dom'
import type { AuthState, LoginRequest } from '../types'
import { authService } from '../services/api'

interface AuthContextType extends AuthState {
  login: (credentials: LoginRequest) => Promise<void>
  logout: () => void
  refreshToken: () => Promise<void>
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({
    isAuthenticated: false,
    user: null,
    isLoading: true,
  })

  const navigate = useNavigate()

  useEffect(() => {
    checkAuth()
  }, [])

  const checkAuth = async () => {
    try {
      const user = await authService.getProfile()
      setState({
        isAuthenticated: true,
        user,
        isLoading: false,
      })
    } catch {
      setState({
        isAuthenticated: false,
        user: null,
        isLoading: false,
      })
    }
  }

  const login = async (credentials: LoginRequest) => {
    setState(prev => ({ ...prev, isLoading: true }))
    try {
      await authService.login(credentials)
      const user = await authService.getProfile()
      setState({
        isAuthenticated: true,
        user,
        isLoading: false,
      })
      navigate('/')
    } catch (error) {
      setState(prev => ({ ...prev, isLoading: false }))
      throw error
    }
  }

  const logout = () => {
    authService.logout()
    setState({
      isAuthenticated: false,
      user: null,
      isLoading: false,
    })
    navigate('/login')
  }

  const refreshToken = async () => {
    try {
      await authService.refreshToken()
    } catch {
      logout()
    }
  }

  return (
    <AuthContext.Provider
      value={{
        ...state,
        login,
        logout,
        refreshToken,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return context
}
