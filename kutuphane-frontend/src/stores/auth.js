// src/stores/auth.js
import { defineStore } from 'pinia'
import { authService } from '@/services/api'

export const useAuthStore = defineStore('auth', {
  // State (veriler)
  state: () => ({
    user: null,           // Giriş yapan kullanıcı bilgileri
    token: localStorage.getItem('token') || null,
    isAuthenticated: false
  }),

  // Getters (hesaplanmış değerler)
  getters: {
    // Kullanıcı giriş yapmış mı?
    isLoggedIn: (state) => !!state.token && !!state.user
  },

  // Actions (fonksiyonlar)
  actions: {
    // Giriş yap
    async login(email, password) {
      try {
        const response = await authService.login(email, password)
        
        // Token'ı kaydet
        this.token = response.token
        this.user = response.user || { email } // API'den user bilgisi gelirse kullan
        this.isAuthenticated = true
        
        // Local storage'a kaydet
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

    // Çıkış yap
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
        // Burada token'ın geçerli olup olmadığını da kontrol edebiliriz
      }
    }
  }
})