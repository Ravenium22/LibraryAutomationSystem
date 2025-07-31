import { defineStore } from 'pinia'
import { authService } from '@/services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,           
    token: localStorage.getItem('token') || null,
    isAuthenticated: false
  }),

  
  getters: {
    isLoggedIn: (state) => !!state.token && !!state.user
  },

  actions: {
    async login(email, password) {
      try {
        const response = await authService.login(email, password)
        
        this.token = response.token
        this.user = response.user || { email } 
        this.isAuthenticated = true
        
        localStorage.setItem('token', response.token)
        
        return { success: true }
      } catch (error) {
        console.error('Login error:', error)
        return { 
          success: false, 
          message: error.response?.data?.message || 'Giriş yapılırken hata oluştu' 
        }
      }
    },

    // Kayıt ol
    async register(ad, soyad, email, password, dogumTarihi, telefon) {
      try {
        const response = await authService.register({
          ad,
          soyad,
          email,
          password,
          dogumTarihi,
          telefon
        })
        
        if (response.token) {
          this.token = response.token
          this.user = response.user || { email, ad, soyad }
          this.isAuthenticated = true
          localStorage.setItem('token', response.token)
        }
        
        return { success: true }
      } catch (error) {
        console.error('Register error:', error)
        return { 
          success: false, 
          message: error.response?.data?.message || 'Kayıt olurken hata oluştu' 
        }
      }
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      localStorage.removeItem('token')
    },

    checkAuth() {
      const token = localStorage.getItem('token')
      if (token) {
        this.token = token
        this.isAuthenticated = true
        
        authService.verifyToken(token)
          .then(response => {
            this.user = response.user || { email: '' } 
          })
          .catch(() => {
            this.logout() 
          }) 
      }
    }
  }
})