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
        
        // Decode token to get user ID
        const payload = JSON.parse(atob(response.token.split('.')[1]))
        this.user = {
          id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid || payload.sub,
          email: response.email,
          ad: response.ad,
          soyad: response.soyad,
          role: response.role
        }
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
        
        return { 
          success: response.success,
          message: response.message
        }
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
        
        // Decode JWT token to get user info (without server verification)
        try {
          const payload = JSON.parse(atob(token.split('.')[1]))
          this.user = {
            id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid || payload.sub,
            email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.email,
            role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role
          }
        } catch (error) {
          console.error('Invalid token format:', error)
          this.logout()
        }
      }
    }
  }
})