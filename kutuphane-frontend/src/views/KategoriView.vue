<template>
  <div class="kategori-page">
    <div v-if="loading" class="loading-container">
      <el-skeleton :rows="5" animated />
    </div>

    <div v-else>
      <div class="page-header">
        <div class="header-left">
          <h1>Kategori Kataloğu</h1>
          <p>Kütüphanedeki kategorileri keşfedin ve kitapları inceleyin</p>
        </div>
      </div>

      <!-- Filtreler -->
      <el-card class="filter-card">
        <el-row :gutter="20">
          <el-col :xs="24" :md="8">
            <el-input
              v-model="searchText"
              placeholder="Kategori adı ara..."
              :prefix-icon="Search"
              clearable
              @input="filterKategoriler"
            />
          </el-col>
          <el-col :xs="24" :md="6">
            <el-select 
              v-model="sortBy" 
              placeholder="Sıralama" 
              @change="sortKategoriler"
            >
              <el-option label="Alfabetik (A-Z)" value="name-asc" />
              <el-option label="Alfabetik (Z-A)" value="name-desc" />
              <el-option label="En Çok Kitap" value="books-desc" />
              <el-option label="En Az Kitap" value="books-asc" />
            </el-select>
          </el-col>
          <el-col :xs="24" :md="4">
            <el-button @click="resetFilters">Temizle</el-button>
          </el-col>
        </el-row>
      </el-card>

      <!-- İstatistikler -->
      <el-row :gutter="20" class="stats-row">
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number">{{ totalKategoriler }}</div>
              <div class="stat-label">Toplam Kategori</div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number available">{{ kategorilerWithBooks }}</div>
              <div class="stat-label">Kitaplı Kategori</div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number busy">{{ emptyKategoriler }}</div>
              <div class="stat-label">Boş Kategori</div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number info">{{ totalBooks }}</div>
              <div class="stat-label">Toplam Kitap</div>
            </div>
          </el-card>
        </el-col>
      </el-row>

      <!-- Kategori Liderligi -->
      <el-card class="leaderboard-card" v-if="trends.length > 0 || trendsLoading">
        <template #header>
          <div class="card-header">
            <h3><el-icon><Trophy /></el-icon> Kategori Liderliği</h3>
            <p>Bu yılın en popüler kategorileri</p>
          </div>
        </template>
        
        <div v-if="trendsLoading" class="leaderboard-loading">
          <el-skeleton :rows="3" animated />
        </div>
        
        <div v-else class="leaderboard">
          <div 
            v-for="(trend, index) in trends.slice(0, 5)" 
            :key="trend.kategoriAdi"
            class="leaderboard-item"
            :class="{ 'leader': index < 3 }"
          >
            <div class="rank">
              <el-icon v-if="index === 0" color="#FFD700"><Trophy /></el-icon>
              <el-icon v-else-if="index === 1" color="#C0C0C0"><Medal /></el-icon>
              <el-icon v-else-if="index === 2" color="#CD7F32"><Medal /></el-icon>
              <span v-else class="rank-number">{{ index + 1 }}</span>
            </div>
            <div class="category-info">
              <span class="category-name">{{ trend.kategoriAdi }}</span>
              <span class="category-stats">{{ trend.oduncSayisi }} ödünç, {{ trend.kitapSayisi }} kitap</span>
            </div>
            <div class="progress-bar">
              <el-progress 
                :percentage="(trend.oduncSayisi / maxOduncSayisi) * 100" 
                :show-text="false"
                :color="getProgressColor(index)"
              />
            </div>
          </div>
        </div>
      </el-card>

      <!-- Kategori Listesi -->
      <div class="kategoriler-container">
        <el-row :gutter="20">
          <el-col 
            v-for="kategori in filteredKategoriler" 
            :key="kategori.id"
            :xs="24" 
            :sm="12" 
            :md="8" 
            :lg="6"
          >
            <el-card 
              class="kategori-card" 
              shadow="hover"
              @click="viewKategoriBooks(kategori)"
            >
              <template #header>
                <div class="card-header">
                  <span class="kategori-title">{{ kategori.ad }}</span>
                  <el-icon class="view-icon"><ArrowRight /></el-icon>
                </div>
              </template>
              
              <div class="kategori-content">
                <p class="kategori-description">{{ kategori.aciklama }}</p>
                
                <div class="kategori-stats">
                  <el-tag 
                    :type="kategori.kitaplar && kategori.kitaplar.length > 0 ? 'success' : 'info'"
                  >
                    {{ kategori.kitaplar ? kategori.kitaplar.length : 0 }} Kitap
                  </el-tag>
                  
                  <el-tag 
                    v-if="kategori.kitaplar && kategori.kitaplar.length > 0"
                    type="primary"
                    class="available-books-tag"
                  >
                    {{ getAvailableBooks(kategori) }} Müsait
                  </el-tag>
                </div>

                <!-- Önizleme Kitaplar -->
                <div 
                  v-if="kategori.kitaplar && kategori.kitaplar.length > 0" 
                  class="preview-books"
                >
                  <div class="preview-title">Örnek Kitaplar:</div>
                  <div class="book-previews">
                    <div 
                      v-for="kitap in kategori.kitaplar.slice(0, 3)" 
                      :key="kitap.id" 
                      class="book-preview"
                      @click.stop="navigateToBookDetail(kitap.id)"
                    >
                      <div class="book-title">{{ kitap.baslik }}</div>
                      <div class="book-author">{{ kitap.yazarAdi || 'Bilinmeyen Yazar' }}</div>
                      <div class="book-preview-hint">
                        <el-icon><ArrowRight /></el-icon>
                      </div>
                    </div>
                    <div 
                      v-if="kategori.kitaplar.length > 3" 
                      class="more-books"
                    >
                      +{{ kategori.kitaplar.length - 3 }} diğer
                    </div>
                  </div>
                </div>

                <div v-else class="empty-category">
                  <el-icon><Document /></el-icon>
                  <span>Bu kategoride henüz kitap bulunmuyor</span>
                </div>
              </div>
            </el-card>
          </el-col>
        </el-row>

        <!-- Kategori bulunamadı durumu -->
        <el-empty 
          v-if="filteredKategoriler.length === 0 && !loading"
          description="Kategori bulunamadı"
        />
      </div>
    </div>

    <!-- Kategori Detay Dialog -->
    <el-dialog 
      v-model="detailDialogVisible" 
      :title="selectedKategori?.ad || 'Kategori Detayları'"
      width="80%"
      top="5vh"
    >
      <div v-if="selectedKategori" class="kategori-detail">
        <div class="detail-header">
          <h3>{{ selectedKategori.ad }}</h3>
          <p>{{ selectedKategori.aciklama }}</p>
          <div class="detail-stats">
            <el-tag type="success">{{ selectedKategori.kitaplar?.length || 0 }} Kitap</el-tag>
            <el-tag type="primary">{{ getAvailableBooks(selectedKategori) }} Müsait</el-tag>
          </div>
        </div>

        <div v-if="selectedKategori.kitaplar && selectedKategori.kitaplar.length > 0">
          <h4>Bu Kategorideki Kitaplar:</h4>
          <el-row :gutter="16" class="books-grid">
            <el-col 
              v-for="kitap in selectedKategori.kitaplar" 
              :key="kitap.id"
              :xs="24" 
              :sm="12" 
              :md="8"
            >
              <el-card 
                class="book-card" 
                shadow="hover"
                @click="navigateToBookDetail(kitap.id)"
              >
                <div class="book-info">
                  <h5 class="book-title">{{ kitap.baslik }}</h5>
                  <p class="book-author">{{ kitap.yazarAdi || 'Bilinmeyen Yazar' }}</p>
                  <div class="book-details">
                    <span class="book-isbn">ISBN: {{ kitap.isbn || 'Belirtilmemiş' }}</span>
                    <span class="book-pages">{{ kitap.sayfaSayisi }} sayfa</span>
                  </div>
                  <div class="book-status">
                    <el-tag 
                      :type="kitap.musaitMi ? 'success' : 'danger'"
                      size="small"
                    >
                      {{ kitap.musaitMi ? 'Müsait' : 'Ödünçte' }}
                    </el-tag>
                    <span class="stock-info">
                      Stok: {{ kitap.musaitStok }}/{{ kitap.toplamStok }}
                    </span>
                  </div>
                  <div v-if="kitap.konum" class="book-location">
                    <el-icon><Location /></el-icon>
                    {{ kitap.konum }} - Raf: {{ kitap.rafNo }}
                  </div>
                  <div class="click-hint">
                    <el-icon><ArrowRight /></el-icon>
                    <span>Detayları görmek için tıklayın</span>
                  </div>
                </div>
              </el-card>
            </el-col>
          </el-row>
        </div>

        <el-empty 
          v-else
          description="Bu kategoride henüz kitap bulunmuyor"
          :image-size="100"
        />
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { 
  Search, 
  ArrowRight, 
  Document, 
  Location, 
  Trophy, 
  Medal 
} from '@element-plus/icons-vue'
import { kategoriService, dashboardService } from '@/services/api'

const router = useRouter()

// State
const loading = ref(false)
const trendsLoading = ref(false)
const kategoriler = ref([])
const filteredKategoriler = ref([])
const trends = ref([])
const searchText = ref('')
const sortBy = ref('name-asc')
const detailDialogVisible = ref(false)
const selectedKategori = ref(null)

// Computed
const totalKategoriler = computed(() => kategoriler.value.length)

const kategorilerWithBooks = computed(() => {
  return kategoriler.value.filter(k => k.kitaplar && k.kitaplar.length > 0).length
})

const emptyKategoriler = computed(() => {
  return kategoriler.value.filter(k => !k.kitaplar || k.kitaplar.length === 0).length
})

const totalBooks = computed(() => {
  return kategoriler.value.reduce((total, kategori) => {
    return total + (kategori.kitaplar ? kategori.kitaplar.length : 0)
  }, 0)
})

const maxOduncSayisi = computed(() => {
  return Math.max(...trends.value.map(t => t.oduncSayisi), 1)
})

// Methods
const loadKategoriler = async () => {
  loading.value = true
  try {
    const response = await kategoriService.getWithBooks()
    kategoriler.value = response
    filteredKategoriler.value = response
    sortKategoriler()
  } catch (error) {
    console.error('Kategoriler yüklenirken hata:', error)
    ElMessage.error('Kategoriler yüklenemedi')
  } finally {
    loading.value = false
  }
}

const loadTrends = async () => {
  trendsLoading.value = true
  try {
    const response = await dashboardService.getTrends()
    
    // Aggregate trends by category (sum up all months for each category)
    const aggregatedTrends = {}
    response.forEach(trend => {
      if (aggregatedTrends[trend.kategoriAdi]) {
        aggregatedTrends[trend.kategoriAdi].oduncSayisi += trend.oduncSayisi
        // Take the max kitapSayisi across months for each category
        if (trend.kitapSayisi > 0) {
          aggregatedTrends[trend.kategoriAdi].kitapSayisi = Math.max(
            aggregatedTrends[trend.kategoriAdi].kitapSayisi, 
            trend.kitapSayisi
          )
        }
      } else {
        aggregatedTrends[trend.kategoriAdi] = {
          kategoriAdi: trend.kategoriAdi,
          oduncSayisi: trend.oduncSayisi,
          kitapSayisi: trend.kitapSayisi || 0
        }
      }
    })
    
    // If kitapSayisi is still 0, try to get it from kategoriler data
    Object.values(aggregatedTrends).forEach(trend => {
      if (trend.kitapSayisi === 0) {
        const kategori = kategoriler.value.find(k => k.ad === trend.kategoriAdi)
        if (kategori && kategori.kitaplar) {
          trend.kitapSayisi = kategori.kitaplar.length
        }
      }
    })
    
    // Convert to array and sort by oduncSayisi
    trends.value = Object.values(aggregatedTrends)
      .filter(trend => trend.oduncSayisi > 0) // Only show categories with actual borrows
      .sort((a, b) => b.oduncSayisi - a.oduncSayisi)
  } catch (error) {
    console.error('Trendler yüklenirken hata:', error)
    // Don't show error message for trends as it's not critical
  } finally {
    trendsLoading.value = false
  }
}

const filterKategoriler = () => {
  if (!searchText.value.trim()) {
    filteredKategoriler.value = kategoriler.value
  } else {
    const search = searchText.value.toLowerCase().trim()
    filteredKategoriler.value = kategoriler.value.filter(kategori => 
      kategori.ad.toLowerCase().includes(search) ||
      kategori.aciklama.toLowerCase().includes(search)
    )
  }
  sortKategoriler()
}

const sortKategoriler = () => {
  const sorted = [...filteredKategoriler.value]
  
  switch (sortBy.value) {
    case 'name-asc':
      sorted.sort((a, b) => a.ad.localeCompare(b.ad, 'tr'))
      break
    case 'name-desc':
      sorted.sort((a, b) => b.ad.localeCompare(a.ad, 'tr'))
      break
    case 'books-desc':
      sorted.sort((a, b) => (b.kitaplar?.length || 0) - (a.kitaplar?.length || 0))
      break
    case 'books-asc':
      sorted.sort((a, b) => (a.kitaplar?.length || 0) - (b.kitaplar?.length || 0))
      break
  }
  
  filteredKategoriler.value = sorted
}

const resetFilters = () => {
  searchText.value = ''
  sortBy.value = 'name-asc'
  filteredKategoriler.value = kategoriler.value
  sortKategoriler()
}

const viewKategoriBooks = (kategori) => {
  selectedKategori.value = kategori
  detailDialogVisible.value = true
}

const navigateToBookDetail = (kitapId) => {
  // Navigate to book page with book ID highlighted
  router.push({
    name: 'kitaplar',
    query: { highlight: kitapId }
  })
}

const getAvailableBooks = (kategori) => {
  if (!kategori.kitaplar) return 0
  return kategori.kitaplar.filter(kitap => kitap.musaitMi).length
}

const getProgressColor = (index) => {
  const colors = ['#FFD700', '#C0C0C0', '#CD7F32', '#409EFF', '#67C23A']
  return colors[index] || '#409EFF'
}

// Lifecycle
onMounted(async () => {
  await loadKategoriler()
  await loadTrends() // Load trends after kategoriler to get book counts
})
</script>

<style scoped>
.kategori-page {
  padding: 20px;
  background-color: #f5f7fa;
  min-height: 100vh;
}

.loading-container {
  padding: 40px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  background: white;
  padding: 24px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.header-left h1 {
  margin: 0 0 8px 0;
  color: #2c3e50;
  font-size: 28px;
  font-weight: 600;
}

.header-left p {
  margin: 0;
  color: #7f8c8d;
  font-size: 16px;
}

.filter-card {
  margin-bottom: 24px;
}

.stats-row {
  margin-bottom: 24px;
}

.stat-card {
  text-align: center;
  transition: transform 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
}

.stat-content .stat-number {
  font-size: 32px;
  font-weight: bold;
  color: #409EFF;
  margin-bottom: 8px;
}

.stat-content .stat-number.available {
  color: #67C23A;
}

.stat-content .stat-number.busy {
  color: #F56C6C;
}

.stat-content .stat-number.info {
  color: #909399;
}

.stat-content .stat-label {
  color: #909399;
  font-size: 14px;
}

.leaderboard-card {
  margin-bottom: 24px;
}

.leaderboard-card .card-header h3 {
  margin: 0 0 8px 0;
  color: #2c3e50;
  display: flex;
  align-items: center;
  gap: 8px;
}

.leaderboard-card .card-header p {
  margin: 0;
  color: #7f8c8d;
  font-size: 14px;
}

.leaderboard-loading {
  padding: 16px;
}

.leaderboard {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.leaderboard-item {
  display: flex;
  align-items: center;
  padding: 12px;
  background: #f8f9fa;
  border-radius: 8px;
  transition: background-color 0.2s;
}

.leaderboard-item.leader {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.leaderboard-item.leader .category-stats {
  color: rgba(255, 255, 255, 0.8);
}

.rank {
  width: 40px;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 18px;
  font-weight: bold;
}

.rank-number {
  color: #606266;
}

.category-info {
  flex: 1;
  margin-left: 16px;
  display: flex;
  flex-direction: column;
}

.category-name {
  font-weight: 600;
  margin-bottom: 4px;
}

.category-stats {
  font-size: 12px;
  color: #909399;
}

.progress-bar {
  width: 100px;
  margin-left: 16px;
}

.kategoriler-container {
  margin-top: 24px;
}

.kategori-card {
  margin-bottom: 20px;
  transition: transform 0.2s, box-shadow 0.2s;
  cursor: pointer;
}

.kategori-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.kategori-title {
  font-weight: 600;
  font-size: 18px;
  color: #2c3e50;
}

.view-icon {
  color: #409EFF;
  transition: transform 0.2s;
}

.kategori-card:hover .view-icon {
  transform: translateX(4px);
}

.kategori-content {
  padding-top: 10px;
}

.kategori-description {
  color: #7f8c8d;
  margin-bottom: 16px;
  line-height: 1.5;
}

.kategori-stats {
  margin-bottom: 16px;
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.available-books-tag {
  margin-left: 8px;
}

.preview-books {
  margin-top: 12px;
}

.preview-title {
  font-size: 14px;
  font-weight: 600;
  color: #606266;
  margin-bottom: 8px;
}

.book-previews {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.book-preview {
  padding: 8px;
  background: #f8f9fa;
  border-radius: 4px;
  border-left: 3px solid #409EFF;
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.book-preview:hover {
  background: #ecf5ff;
  border-left-color: #337ecc;
  transform: translateX(4px);
}

.book-preview-hint {
  position: absolute;
  top: 8px;
  right: 8px;
  opacity: 0;
  transition: opacity 0.2s;
  color: #409EFF;
  font-size: 12px;
}

.book-preview:hover .book-preview-hint {
  opacity: 1;
}

.book-title {
  font-size: 13px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 2px;
}

.book-author {
  font-size: 12px;
  color: #7f8c8d;
}

.more-books {
  text-align: center;
  padding: 8px;
  color: #409EFF;
  font-size: 12px;
  font-weight: 600;
  background: #ecf5ff;
  border-radius: 4px;
}

.empty-category {
  text-align: center;
  padding: 20px;
  color: #909399;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
}

.kategori-detail {
  max-height: 70vh;
  overflow-y: auto;
}

.detail-header {
  margin-bottom: 24px;
  text-align: center;
}

.detail-header h3 {
  margin: 0 0 8px 0;
  color: #2c3e50;
}

.detail-header p {
  margin: 0 0 16px 0;
  color: #7f8c8d;
}

.detail-stats {
  display: flex;
  justify-content: center;
  gap: 12px;
}

.books-grid {
  margin-top: 16px;
}

.book-card {
  margin-bottom: 16px;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.book-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
}

.book-info {
  padding: 8px;
}

.book-title {
  margin: 0 0 8px 0;
  color: #2c3e50;
  font-size: 16px;
  font-weight: 600;
}

.book-author {
  margin: 0 0 12px 0;
  color: #7f8c8d;
  font-size: 14px;
}

.book-details {
  margin-bottom: 12px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.book-isbn, .book-pages {
  font-size: 12px;
  color: #909399;
}

.book-status {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.stock-info {
  font-size: 12px;
  color: #909399;
}

.book-location {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  color: #409EFF;
}

.click-hint {
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid #ebeef5;
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 11px;
  color: #909399;
  transition: color 0.2s;
}

.book-card:hover .click-hint {
  color: #409EFF;
}

@media (max-width: 768px) {
  .kategori-page {
    padding: 16px;
  }
  
  .page-header {
    flex-direction: column;
    gap: 16px;
    align-items: flex-start;
  }
  
  .stats-row .el-col {
    margin-bottom: 12px;
  }

  .leaderboard-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }

  .progress-bar {
    width: 100%;
    margin-left: 0;
  }
}

@media (max-width: 480px) {
  .kategori-page {
    padding: 12px;
  }
  
  .header-left h1 {
    font-size: 24px;
  }
  
  .card-header {
    flex-direction: column;
    gap: 12px;
    align-items: flex-start;
  }
  
  .kategori-stats {
    flex-direction: column;
    gap: 8px;
  }
}
</style>