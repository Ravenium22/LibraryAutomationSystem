<template>
  <el-container class="layout-container">
    <!-- Header -->
    <el-header class="header">
      <div class="header-left">
        <h3>Kütüphane Yönetim Sistemi</h3>
      </div>
      <div class="header-right">
        <span>Hoş geldin, {{ authStore.user?.email }}</span>
        <el-button type="danger" @click="handleLogout" :icon="SwitchButton">
          Çıkış Yap
        </el-button>
      </div>
    </el-header>

    <el-container>
      <!-- Sidebar -->
      <el-aside width="250px" class="sidebar">
        <el-menu
          :default-active="$route.path"
          class="sidebar-menu"
          router
          background-color="#065535"
          text-color="#fff"
          active-text-color="#d3ffce"
        >
          <div class="logo-section">
            <img src="@/assets/logo.png" alt="Logo" />
            <h4>Kütüphane Yönetim Sistemi</h4>
          </div>

          <el-menu-item index="/">
            <el-icon><DataBoard /></el-icon>
            <span>Dashboard</span>
          </el-menu-item>

          <el-menu-item index="/kitaplar">
            <el-icon><Reading /></el-icon>
            <span>Kitap Yönetimi</span>
          </el-menu-item>

          <el-menu-item index="/kullanicilar">
            <el-icon><User /></el-icon>
            <span>Kullanıcı Yönetimi</span>
          </el-menu-item>

          <el-menu-item index="/odunc">
            <el-icon><RefreshRight /></el-icon>
            <span>Ödünç İşlemleri</span>
          </el-menu-item>

          <el-menu-item index="/kategoriler">
            <el-icon><Collection /></el-icon>
            <span>Kategori Yönetimi</span>
          </el-menu-item>

          <el-menu-item index="/yazarlar">
            <el-icon><EditPen /></el-icon>
            <span>Yazar Yönetimi</span>
          </el-menu-item>
        </el-menu>
      </el-aside>

      <!-- Main Content -->
      <el-main class="main-content">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script>
import { useAuthStore } from '@/stores/auth'
import { 
  DataBoard, 
  Reading, 
  User, 
  RefreshRight, 
  Collection, 
  EditPen,
  SwitchButton 
} from '@element-plus/icons-vue'

export default {
  name: 'AppLayout',
  components: {
    DataBoard,
    Reading, 
    User, 
    RefreshRight, 
    Collection, 
    EditPen,
    SwitchButton
  },
  setup() {
    const authStore = useAuthStore()
    return { 
      authStore,
      DataBoard,
      Reading, 
      User, 
      RefreshRight, 
      Collection, 
      EditPen,
      SwitchButton
    }
  },
  methods: {
    handleLogout() {
      this.authStore.logout()
      this.$router.push('/login')
    }
  }
}
</script>

<style scoped>
.layout-container {
  height: 100vh;
}

.header {
  background: linear-gradient(90deg, #065535 0%, #065535 100%);
  display: flex;
  align-items: center;
  justify-content: space-between;
  color: #d3ffce;
  padding: 0 20px;
}

.header-left h3 {
  margin: 0;
  color: white;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 15px;
  color: white;
}

.sidebar {
  background-color: #001529;
}

.sidebar-menu {
  height: 100%;
  border: none;
}

.logo-section {
  padding: 20px;
  text-align: center;
  border-bottom: 1px solid #333;
  margin-bottom: 10px;
}

.logo-section h4 {
  color: #d3ffce;
  margin: 0;
  font-size: 24px;
}

.logo-section p {
  color: #999;
  margin: 5px 0 0 0;
  font-size: 12px;
  
}
.logo-section img {
  width: 50px;
  height: 50px;
  border-radius: 50%;
}

.main-content {
  background-color: #f0f2f5;
  padding: 20px;
}
</style>