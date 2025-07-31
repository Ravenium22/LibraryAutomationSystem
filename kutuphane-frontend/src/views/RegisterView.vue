<template>
<div class="register-container">            
<div class="register-form">
  <h2>Kayıt Ol</h2>
  
  <p>Lütfen kayıt bilgilerinizi girin.</p>
  <form @submit.prevent="handleRegister">
    <div class="form-group">
      <label>Ad:</label>
      <input 
        type="text" 
        v-model="registerData.ad" 
        required
        placeholder="Adınızı girin"
      >
    </div>

    <div class="form-group">
      <label>Soyad:</label>
      <input 
        type="text" 
        v-model="registerData.soyad" 
        required
        placeholder="Soyadınızı girin"
      >
    </div>

    <div class="form-group">
      <label>Email:</label>
      <input 
        type="email" 
        v-model="registerData.email" 
        required
        placeholder="email@example.com"
      >
    </div>

    <div class="form-group">
      <label>Şifre:</label>
      <input 
        type="password" 
        v-model="registerData.password" 
        required
        placeholder="Şifrenizi girin"
      >
    </div>

    <div class="form-group">
      <label>Doğum Tarihi:</label>
      <input 
        type="date" 
        v-model="registerData.dogumTarihi" 
        required
      >
    </div>

    <div class="form-group">
      <label>Telefon:</label>
      <input 
        type="tel" 
        v-model="registerData.telefon" 
        required
        placeholder="Telefon numaranızı girin"
      >
    </div>

    <button type="submit" :disabled="isLoading">
      {{ isLoading ? 'Kayıt yapılıyor...' : 'Kayıt Ol' }}
    </button>
  </form>

  <div v-if="error" class="error-message">
    {{ error }}
  </div>

  <!-- Başarılı kayıt mesajı -->
  <div v-if="successMessage" class="success-message">
    {{ successMessage }}
  </div>
  <div v-if="!error && !successMessage" class="info-message">
    Zaten bir hesabınız var mı? <router-link to="/login">Giriş Yapın</router-link>
  </div>
</div>
</div>

</template>

<script>
import { useAuthStore } from '@/stores/auth'

export default {
  name: 'RegisterView',
  data() {
    return {
      registerData: {
        ad: '',
        soyad: '',
        email: '',
        password: '',
        dogumTarihi: '',
        telefon: ''
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
    async handleRegister() {
      this.isLoading = true
      this.error = null
      this.successMessage = null

      try {
        // Store üzerinden kayıt ol
        const result = await this.authStore.register(
          this.registerData.ad,
          this.registerData.soyad,
          this.registerData.email,
          this.registerData.password,
          this.registerData.dogumTarihi,
          this.registerData.telefon
        )

        if (result.success) {
          this.successMessage = 'Kayıt başarılı! Yönlendiriliyorsunuz...'

          setTimeout(() => {
            this.$router.push('/login')
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
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.register-form {
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