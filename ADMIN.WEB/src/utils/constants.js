export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api'

export const ROUTES = {
  LOGIN: '/login',
  DASHBOARD: '/',
  USERS: '/users',
  TRAFFIC_SIGNS: '/traffic-signs',
  CONTRIBUTIONS: '/contributions',
  VOTES: '/votes',
  WALLETS: '/wallets',
  PAYMENTS: '/payments',
  NOTIFICATIONS: '/notifications',
  FEEDBACK: '/feedback',
}

export const STATUS_OPTIONS = {
  CONTRIBUTION: ['Pending', 'Approved', 'Rejected'],
  PAYMENT: ['Pending', 'Completed', 'Failed'],
  TRAFFIC_SIGN: ['Active', 'Inactive', 'Removed'],
  FEEDBACK: ['Pending', 'Resolved', 'Dismissed'],
}

export const USER_ROLES = {
  ADMIN: 'Admin',
  STAFF: 'Staff',
  USER: 'User',
}

export const STORAGE_KEYS = {
  TOKEN: 'admin_token',
  USER: 'admin_user',
}

