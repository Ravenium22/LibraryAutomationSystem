<template>
  <div class="login-container">
    <div class="login-form">
      <h2>Kütüphane Girişi</h2>
      
      <p>Lütfen giriş bilgilerinizi girin.</p>
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label>Email:</label>
          <input 
            type="email" 
            v-model="loginData.email" 
            required
            placeholder="email@example.com"
          >
        </div>
        
        <div class="form-group">
          <label>Şifre:</label>
          <input 
            type="password" 
            v-model="loginData.password" 
            required
            placeholder="Şifrenizi girin"
          >
        </div>
        
        <button type="submit" :disabled="isLoading">
          {{ isLoading ? 'Giriş yapılıyor...' : 'Giriş Yap' }}
        </button>
      </form>
      
      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <!-- Başarılı giriş mesajı -->
      <div v-if="successMessage" class="success-message">
        {{ successMessage }}
      </div>
      <div v-if="!error && !successMessage" class="info-message">
        Henüz hesabınız yok mu? <router-link to="/register">Kayıt Olun</router-link>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/auth'

export default {
  name: 'LoginView',
  data() {
    return {
      loginData: {
        email: '',
        password: ''
      },
      isLoading: false,
      error: null,
      successMessage: null
    }
  },
  setup() {
    const authStore = useAuthStore()
    return { authStore }
  },
  methods: {
    async handleLogin() {
      this.isLoading = true
      this.error = null
      this.successMessage = null
      
      try {
        // Store üzerinden giriş yap
        const result = await this.authStore.login(
          this.loginData.email, 
          this.loginData.password
        )

        if (result.success) {
          this.successMessage = 'Giriş başarılı! Yönlendiriliyorsunuz...'
          setTimeout(() => {
            this.$router.push('/')
          }, 300)
        } else {
          this.error = result.message
        }
      } catch (error) {
        this.error = 'Beklenmeyen bir hata oluştu'
        console.error(error)
      } finally {
        this.isLoading = false
      }
    }
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.login-form {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
}

.form-group {
  margin-bottom: 1rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

button {
  width: 100%;
  padding: 0.75rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
}

button:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.error-message {
  color: red;
  margin-top: 1rem;
  padding: 0.5rem;
  background-color: #ffebee;
  border-radius: 4px;
}

.success-message {
  color: green;
  margin-top: 1rem;
  padding: 0.5rem;
  background-color: #e8f5e8;
  border-radius: 4px;
}

.info-message {
  margin-top: 1rem;
  text-align: center;
}
</style>