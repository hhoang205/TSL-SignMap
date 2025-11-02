import api from '../utils/api'

// User API
export const userApi = {
  getAll: (params) => api.get('/users', { params }),
  getById: (id) => api.get(`/users/${id}`),
  update: (id, data) => api.put(`/users/${id}`, data),
  delete: (id) => api.delete(`/users/${id}`),
  filter: (data) => api.post('/users/filter', data),
}

// Traffic Sign API
export const trafficSignApi = {
  getAll: () => api.get('/signs'),
  getById: (id) => api.get(`/signs/${id}`),
  create: (data) => api.post('/signs', data),
  update: (id, data) => api.put(`/signs/${id}`, data),
  delete: (id) => api.delete(`/signs/${id}`),
  search: (data) => api.post('/signs/search', data),
}

// Contribution API
export const contributionApi = {
  getAll: () => api.get('/contributions'),
  getById: (id) => api.get(`/contributions/${id}`),
  getByStatus: (status) => api.get(`/contributions/status/${status}`),
  getByUserId: (userId) => api.get(`/contributions/user/${userId}`),
  approve: (id, data) => api.post(`/contributions/${id}/approve`, data),
  reject: (id, data) => api.post(`/contributions/${id}/reject`, data),
  filter: (data) => api.post('/contributions/filter', data),
}

// Vote API
export const voteApi = {
  getAll: () => api.get('/votes'),
  getById: (id) => api.get(`/votes/${id}`),
  getByContributionId: (contributionId) => api.get(`/votes/contribution/${contributionId}`),
  override: (id, data) => api.post(`/votes/${id}/override`, data),
  filter: (data) => api.post('/votes/filter', data),
}

// Wallet API
export const walletApi = {
  getAll: () => api.get('/wallets'),
  getById: (id) => api.get(`/wallets/${id}`),
  getByUserId: (userId) => api.get(`/wallets/user/${userId}`),
  getBalance: (userId) => api.get(`/wallets/user/${userId}/balance`),
  getSummary: () => api.get('/wallets/summary'),
  create: (data) => api.post('/wallets', data),
  update: (id, data) => api.put(`/wallets/${id}`, data),
  adjust: (userId, data) => api.post(`/wallets/user/${userId}/adjust`, data),
  filter: (data) => api.post('/wallets/filter', data),
}

// Payment API
export const paymentApi = {
  getAll: () => api.get('/payments'),
  getById: (id) => api.get(`/payments/${id}`),
  getByUserId: (userId) => api.get(`/payments/user/${userId}`),
  getByStatus: (status) => api.get(`/payments/status/${status}`),
  update: (id, data) => api.put(`/payments/${id}`, data),
  filter: (data) => api.post('/payments/filter', data),
  getSummary: () => api.get('/payments/summary'),
}

// Notification API
export const notificationApi = {
  getAll: () => api.get('/notifications'),
  getById: (id) => api.get(`/notifications/${id}`),
  getByUserId: (userId) => api.get(`/notifications/user/${userId}`),
  create: (data) => api.post('/notifications', data),
  update: (id, data) => api.put(`/notifications/${id}`, data),
  delete: (id) => api.delete(`/notifications/${id}`),
  filter: (data) => api.post('/notifications/filter', data),
}

// Feedback API
export const feedbackApi = {
  getAll: () => api.get('/feedbacks'),
  getById: (id) => api.get(`/feedbacks/${id}`),
  getByUserId: (userId) => api.get(`/feedbacks/user/${userId}`),
  update: (id, data) => api.put(`/feedbacks/${id}`, data),
  filter: (data) => api.post('/feedbacks/filter', data),
}

