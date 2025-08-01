import axios from 'axios'

const API_BASE_URL = 'http://localhost:5164/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

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
  async getAll() {
    const response = await apiClient.get('/Kitap')
    return response.data
  },

  async getById(id) {
    const response = await apiClient.get(`/Kitap/${id}`)
    return response.data
  },

  async create(kitapData) {
    const response = await apiClient.post('/Kitap/admin/create-kitap', kitapData)
    return response.data
  },

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
  },

  // Stok biten kitaplar (Admin)
  async getStokBiten() {
    const response = await apiClient.get('/Kitap/admin/stok-biten')
    return response.data
  },

  // Stok yakın biten kitaplar (Admin)
  async getStokYakinBiten() {
    const response = await apiClient.get('/Kitap/admin/stok-yakin-biten')
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
  },

  // Yazarları kitaplarıyla birlikte getir
  async getWithBooks() {
    const response = await apiClient.get('/Yazar/with-kitaplar')
    return response.data
  },

  // Ülkeye göre yazar getir
  async getByCountry(ulke) {
    const response = await apiClient.get(`/Yazar/ulke/${ulke}`)
    return response.data
  },

  // Yazar ara
  async search(searchParams) {
    const response = await apiClient.get('/Yazar/search', {
      params: searchParams
    })
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
  },

  // Yeni kategori oluştur (Admin)
  async create(kategoriData) {
    const response = await apiClient.post('/Kategori/admin/create-kategori', kategoriData)
    return response.data
  },

  // Kategori güncelle (Admin)
  async update(id, kategoriData) {
    const response = await apiClient.put(`/Kategori/admin/update-kategori/${id}`, kategoriData)
    return response.data
  },

  // Kategori sil (Admin)
  async delete(id) {
    const response = await apiClient.delete(`/Kategori/admin/delete-kategori/${id}`)
    return response.data
  },

  // Kategorileri kitaplarıyla birlikte getir
  async getWithBooks() {
    const response = await apiClient.get('/Kategori/with-kitaplar')
    return response.data
  },

  // İsme göre kategori getir
  async getByName(name) {
    const response = await apiClient.get(`/Kategori/name/${name}`)
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

// Kullanici servisleri
export const kullaniciService = {
  // Tüm kullanıcıları getir
  async getAll() {
    const response = await apiClient.get('/Kullanici')
    return response.data
  },

  // ID'ye göre kullanıcı getir
  async getById(id) {
    const response = await apiClient.get(`/Kullanici/${id}`)
    return response.data
  },

  // Yeni kullanıcı oluştur (Admin)
  async create(kullaniciData) {
    const response = await apiClient.post('/Kullanici/admin/create-kullanici', kullaniciData)
    return response.data
  },

  // Kullanıcı güncelle (Admin)
  async update(id, kullaniciData) {
    const response = await apiClient.put(`/Kullanici/admin/update-kullanici/${id}`, kullaniciData)
    return response.data
  },

  // Kullanıcı sil (Admin)
  async delete(id) {
    const response = await apiClient.delete(`/Kullanici/admin/delete-kullanici/${id}`)
    return response.data
  },

  // Kullanıcıları ödünçleriyle birlikte getir
  async getWithOduncler() {
    const response = await apiClient.get('/Kullanici/with-oduncler')
    return response.data
  },

  // Aktif kullanıcıları getir
  async getAktif() {
    const response = await apiClient.get('/Kullanici/aktif')
    return response.data
  }
}

// Odunc servisleri
export const oduncService = {
  // Tüm ödünçleri getir
  async getAll() {
    const response = await apiClient.get('/Odunc')
    return response.data
  },

  // ID'ye göre ödünç getir
  async getById(id) {
    const response = await apiClient.get(`/Odunc/${id}`)
    return response.data
  },

  // Yeni ödünç oluştur (Admin)
  async create(oduncData) {
    const response = await apiClient.post('/Odunc/admin/create-odunc', oduncData)
    return response.data
  },

  // Ödünç güncelle (Admin)
  async update(id, oduncData) {
    const response = await apiClient.put(`/Odunc/admin/update-odunc/${id}`, oduncData)
    return response.data
  },

  // Ödünç sil (Admin)
  async delete(id) {
    const response = await apiClient.delete(`/Odunc/admin/delete-odunc/${id}`)
    return response.data
  },

  // Aktif ödünçleri getir
  async getAktif() {
    const response = await apiClient.get('/Odunc/aktif')
    return response.data
  },

  // Süresi dolan ödünçleri getir
  async getSuresiDolan() {
    const response = await apiClient.get('/Odunc/suresi-dolan')
    return response.data
  },

  // Kullanıcının ödünçlerini getir
  async getByKullanici(kullaniciId) {
    const response = await apiClient.get(`/Odunc/kullanici/${kullaniciId}`)
    return response.data
  },

  // Kitabın ödünç geçmişini getir
  async getKitapGecmis(kitapId) {
    const response = await apiClient.get(`/Odunc/kitap/${kitapId}/gecmis`)
    return response.data
  },

  // Borç raporu getir (Admin)
  async getBorcRaporu() {
    const response = await apiClient.get('/Odunc/admin/borç-raporu')
    return response.data
  },

  // Kitap iade et (Admin)
  async iade(id) {
    const response = await apiClient.post(`/Odunc/admin/${id}/iade`)
    return response.data
  },

  // Ödünç ver (Admin)
  async oduncVer(oduncData) {
    const response = await apiClient.post('/Odunc/admin/odunc-ver', oduncData)
    return response.data
  }
}

export default apiClient