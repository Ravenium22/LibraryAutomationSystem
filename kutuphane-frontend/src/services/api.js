import axios from 'axios'

const API_BASE_URL = 'http://localhost:5164/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Token varsa her istekte otomatik ekle
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Auth servisleri
export const authService = {
  // Giriş yap
  async login(email, password) {
    const response = await apiClient.post('/Auth/login', {
      email,
      password
    })
    return response.data
  },

  // Kayıt ol
  async register(userData) {
    const response = await apiClient.post('/Auth/register', userData)
    return response.data
  }
}

// Kitap servisleri
export const kitapService = {
  // Tüm kitapları getir
  async getAll() {
    const response = await apiClient.get('/Kitap')
    return response.data
  },

  // ID'ye göre kitap getir
  async getById(id) {
    const response = await apiClient.get(`/Kitap/${id}`)
    return response.data
  },

  // Yeni kitap oluştur (Admin)
  async create(kitapData) {
    const response = await apiClient.post('/Kitap/admin/create-kitap', kitapData)
    return response.data
  },

  // Kitap güncelle (Admin)
  async update(id, kitapData) {
    const response = await apiClient.put(`/Kitap/admin/update-kitap/${id}`, kitapData)
    return response.data
  },

  // Kitap sil (Admin)
  async delete(id) {
    const response = await apiClient.delete(`/Kitap/admin/delete-kitap/${id}`)
    return response.data
  },

  // Müsait kitapları getir
  async getMusait() {
    const response = await apiClient.get('/Kitap/musait')
    return response.data
  },

  // Meşgul kitapları getir
  async getMesgul() {
    const response = await apiClient.get('/Kitap/mesgul')
    return response.data
  },

  // Kitap ara
  async search(searchParams) {
    const response = await apiClient.get('/Kitap/kitap/search', {
      params: searchParams
    })
    return response.data
  }

}

// Yazar servisleri
export const yazarService = {
  // Tüm yazarları getir
  async getAll() {
    const response = await apiClient.get('/Yazar')
    return response.data
  },

  // ID'ye göre yazar getir
  async getById(id) {
    const response = await apiClient.get(`/Yazar/${id}`)
    return response.data
  },

  // Yeni yazar oluştur (Admin)
  async create(yazarData) {
    const response = await apiClient.post('/Yazar/admin/create-yazar', yazarData)
    return response.data
  },

  // Yazar güncelle (Admin)
  async update(id, yazarData) {
    const response = await apiClient.put(`/Yazar/admin/update-yazar/${id}`, yazarData)
    return response.data
  },

  // Yazar sil (Admin)
  async delete(id) {
    const response = await apiClient.delete(`/Yazar/admin/delete-yazar/${id}`)
    return response.data
  }
}

// Kategori servisleri
export const kategoriService = {
  // Tüm kategorileri getir
  async getAll() {
    const response = await apiClient.get('/Kategori')
    return response.data
  },

  // ID'ye göre kategori getir
  async getById(id) {
    const response = await apiClient.get(`/Kategori/${id}`)
    return response.data
  }
}

// Dashboard servisleri
export const dashboardService = {
  // Public dashboard istatistiklerini getir
  async getPublicStats() {
    const response = await apiClient.get('/Dashboard/public/stats')
    return response.data
  },

  // Popüler kitapları getir
  async getPopularBooks(limit = 10) {
    const response = await apiClient.get('/Dashboard/public/popular-books', {
      params: { limit }
    })
    return response.data
  },

  // Trend verilerini getir
  async getTrends() {
    const response = await apiClient.get('/Dashboard/public/trends')
    return response.data
  },

  // Admin - tam dashboard verilerini getir (requires authentication)
  async getFullDashboard() {
    const response = await apiClient.get('/Dashboard/admin/full')
    return response.data
  },

  // Admin - finansal verileri getir (requires authentication)
  async getFinancialData() {
    const response = await apiClient.get('/Dashboard/admin/financial')
    return response.data
  }
}

export default apiClient