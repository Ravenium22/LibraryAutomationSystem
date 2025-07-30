<template>
  <div class="dashboard">
    <el-row :gutter="20">
      <el-col :span="24">
        <h1>Dashboard</h1>
        <p>Kütüphane sistemi genel görünümü</p>
      </el-col>
    </el-row>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-container">
      <el-loading-spinner />
      <p>İstatistikler yükleniyor...</p>
    </div>

    <!-- Error -->
    <el-alert
      v-else-if="error"
      :title="error"
      type="error"
      show-icon
      @close="loadDashboardData"
    />

    <!-- Stats Cards -->
    <el-row v-else :gutter="20" class="stats-row">
      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <el-icon class="stat-icon" color="#409EFF"><Reading /></el-icon>
            <div class="stat-info">
              <h3>{{ stats.totalBooks || 0 }}</h3>
              <p>Toplam Kitap</p>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <el-icon class="stat-icon" color="#67C23A"><User /></el-icon>
            <div class="stat-info">
              <h3>{{ stats.totalCategories || 0 }}</h3>
              <p>Toplam Kategori</p>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <el-icon class="stat-icon" color="#E6A23C"><RefreshRight /></el-icon>
            <div class="stat-info">
              <h3>{{ stats.availableBooks || 0 }}</h3>
              <p>Müsait Kitap</p>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <el-icon class="stat-icon" color="#F56C6C"><Warning /></el-icon>
            <div class="stat-info">
              <h3>{{ stats.totalAuthors || 0 }}</h3>
              <p>Toplam Yazar</p>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- Content Cards -->
    <el-row :gutter="20" style="margin-top: 20px;">
      <!-- Popüler Kitaplar -->
      <el-col :xs="24" :md="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>Popüler Kitaplar</span>
              <el-button type="text">Tümünü Gör</el-button>
            </div>
          </template>
          
          <el-empty v-if="popularBooks.length === 0" description="Henüz popüler kitap verisi yok" />
          
          <div v-else>
            <div 
              v-for="(book, index) in popularBooks.slice(0, 5)" 
              :key="book.id"
              class="book-item"
            >
              <el-tag>{{ index + 1 }}</el-tag>
              <div class="book-info">
                <strong>{{ book.title }}</strong>
                <p>{{ book.author }}</p>
              </div>
              <el-tag type="success">{{ book.loanCount }} ödünç</el-tag>
            </div>
          </div>
        </el-card>
      </el-col>
      <!-- Kullanıcı Geri Bildirimleri -->
      <el-col :xs="24" :md="8">
        <el-card class="testimonial-card">
          <template #header>
            <div class="card-header">
              <span>Kullanıcı Geri Bildirimleri</span>
              <el-icon color="#409EFF"><ChatDotSquare /></el-icon>
            </div>
          </template>

          <div class="testimonial-content">
            <div class="testimonial-item">
              <div class="testimonial-text">
                <el-icon class="quote-icon"><ChatLineRound /></el-icon>
                <p>"Kütüphane sistemi, kitaplarımı yönetmemi çok kolaylaştırdı! Artık tüm koleksiyonuma kolayca erişebiliyorum."</p>
              </div>
              <div class="testimonial-author">
                <el-avatar :size="32" class="author-avatar">A</el-avatar>
                <div class="author-info">
                  <strong>Ahmet P.</strong>
                  <small>Öğretmen</small>
                </div>
                <div class="rating">
                  <el-rate v-model="testimonialRatings.user1" disabled show-score size="small" />
                </div>
              </div>
            </div>

            <el-divider />

            <div class="testimonial-item">
              <div class="testimonial-text">
                <el-icon class="quote-icon"><ChatLineRound /></el-icon>
                <p>"Kitap arama ve filtreleme özellikleri gerçekten harika. İstediğim kitabı saniyeler içinde bulabiliyorum."</p>
              </div>
              <div class="testimonial-author">
                <el-avatar :size="32" class="author-avatar">B</el-avatar>
                <div class="author-info">
                  <strong>Büşra K.</strong>
                  <small>Kütüphane Görevlisi</small>
                </div>
                <div class="rating">
                  <el-rate v-model="testimonialRatings.user2" disabled show-score size="small" />
                </div>
              </div>
            </div>

            <el-divider />

            <div class="testimonial-item">
              <div class="testimonial-text">
                <el-icon class="quote-icon"><ChatLineRound /></el-icon>
                <p>"Ödünç alma ve iade işlemleri çok pratik. Sistem sayesinde hiç kitap kaybetmedim!"</p>
              </div>
              <div class="testimonial-author">
                <el-avatar :size="32" class="author-avatar">C</el-avatar>
                <div class="author-info">
                  <strong>Cem Y.</strong>
                  <small>Araştırmacı</small>
                </div>
                <div class="rating">
                  <el-rate v-model="testimonialRatings.user3" disabled show-score size="small" />
                </div>
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>


    <el-row :gutter="20" style="margin-top: 20px;" v-if="trends.length > 0">
      <el-col :span="24">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>Kategori Trendleri</span>
              <el-button type="text">Detayları Gör</el-button>
            </div>
          </template>
          
          <div class="trends-container">
            <div 
              v-for="trend in trends.slice(0, 6)" 
              :key="trend.kategoriAdi"
              class="trend-item"
            >
              <div class="trend-header">
                <strong>{{ trend.kategoriAdi }}</strong>
                <small>{{ trend.ay }}</small>
              </div>
              <div class="trend-stats">
                <span class="book-count">{{ trend.kitapSayisi }} kitap</span>
                <span class="loan-count">{{ trend.oduncSayisi }} ödünç</span>
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { Reading, User, RefreshRight, Warning, Plus, UserFilled, ChatDotSquare, ChatLineRound } from '@element-plus/icons-vue'
import { dashboardService } from '@/services/api'

export default {
  name: 'DashboardView',
  components: {
    Reading,
    User,
    RefreshRight,
    Warning,
    Plus,
    UserFilled,
    ChatDotSquare,
    ChatLineRound
  },
  data() {
    return {
      stats: {},
      popularBooks: [],
      trends: [],
      isLoading: false,
      error: null,
      testimonialRatings: {
        user1: 5,
        user2: 5,
        user3: 4
      }
    }
  },
  async created() {
    await this.loadDashboardData()
  },
  methods: {
    async loadDashboardData() {
      this.isLoading = true
      this.error = null

      try {
        // Public dashboard stats almak
        const statsResponse = await dashboardService.getPublicStats()
        this.stats = {
          totalBooks: statsResponse.toplamKitapSayisi,
          totalCategories: statsResponse.toplamKategoriSayisi,
          totalAuthors: statsResponse.toplamYazarSayisi,
          availableBooks: statsResponse.musaitKitapSayisi,
          activeLoans: statsResponse.toplamKitapSayisi - statsResponse.musaitKitapSayisi, // Aktif ödünç = toplam - müsait
          overdueBooks: 0 // Bu veri API'de yok, varsayılan 0
        }

        // Popüler kitapları al
        const popularBooksResponse = await dashboardService.getPopularBooks(5)
        this.popularBooks = popularBooksResponse.map(book => ({
          id: book.kitapAdi, // Unique identifier olarak kullanıyoruz
          title: book.kitapAdi,
          author: book.yazarAdi,
          loanCount: book.oduncSayisi,
          category: book.kategoriAdi,
          isAvailable: book.isMusait,
          coverUrl: book.kapakUrl
        }))

        // Trend verilerini al
        const trendsResponse = await dashboardService.getTrends()
        this.trends = trendsResponse

      } catch (error) {
        console.error('Dashboard data error:', error)
        this.error = 'Veriler yüklenirken bir hata oluştu'
      } finally {
        this.isLoading = false
      }
    }
  }
}
</script>

<style scoped>
.dashboard h1 {
  margin: 0 0 10px 0;
  color: #303133;
}

.stats-row {
  margin-top: 20px;
}

.stat-card {
  margin-bottom: 20px;
}

.stat-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.stat-icon {
  font-size: 40px;
}

.stat-info h3 {
  margin: 0;
  font-size: 28px;
  color: #303133;
}

.stat-info p {
  margin: 5px 0 0 0;
  color: #909399;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.book-item {
  display: flex;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #f4f4f5;
  gap: 15px;
}

.book-item:last-child {
  border-bottom: none;
}

.book-info {
  flex: 1;
}

.book-info strong {
  color: #303133;
}

.book-info p {
  margin: 5px 0 0 0;
  color: #909399;
  font-size: 14px;
}

.loading-container {
  text-align: center;
  padding: 50px;
}

.trends-container {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.trend-item {
  background: #f8f9fa;
  padding: 16px;
  border-radius: 8px;
  border-left: 4px solid #409EFF;
}

.trend-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.trend-header strong {
  color: #303133;
  font-size: 14px;
}

.trend-header small {
  color: #909399;
  font-size: 12px;
}

.trend-stats {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
}

.book-count {
  color: #67C23A;
}

.loan-count {
  color: #E6A23C;
}

/* Testimonial Styles */
.testimonial-card {
  height: 100%;
}

.testimonial-content {
  max-height: 500px;
  overflow-y: auto;
}

.testimonial-item {
  margin: 16px 0;
}

.testimonial-text {
  position: relative;
  margin-bottom: 16px;
}

.quote-icon {
  position: absolute;
  top: -2px;
  left: -8px;
  color: #409EFF;
  font-size: 16px;
  opacity: 0.7;
}

.testimonial-text p {
  margin: 0;
  padding-left: 16px;
  font-style: italic;
  color: #606266;
  line-height: 1.6;
  font-size: 14px;
}

.testimonial-author {
  display: flex;
  align-items: center;
  gap: 12px;
  padding-left: 16px;
}

.author-avatar {
  background: linear-gradient(135deg, #409EFF, #67C23A);
  color: white;
  font-weight: bold;
}

.author-info {
  flex: 1;
}

.author-info strong {
  display: block;
  color: #303133;
  font-size: 14px;
  margin-bottom: 2px;
}

.author-info small {
  color: #909399;
  font-size: 12px;
}

.rating {
  margin-left: auto;
}

.testimonial-content .el-divider {
  margin: 16px 0;
  opacity: 0.3;
}
</style>