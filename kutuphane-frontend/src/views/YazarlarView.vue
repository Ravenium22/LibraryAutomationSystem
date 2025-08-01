<template>
  <div class="yazarlar-page">
    <div class="header-section">
      <h1>Yazarlar</h1>
      <div class="header-actions">
        <el-input
          v-model="searchQuery"
          placeholder="Yazar ara..."
          style="width: 300px; margin-right: 10px;"
          :prefix-icon="Search"
          @input="handleSearch"
          clearable
        />
        <el-button type="primary" @click="refreshData" :loading="loading">
          <el-icon><RefreshRight /></el-icon>
          Yenile
        </el-button>
      </div>
    </div>

    <!-- Stats Cards -->
    <el-card class="stats-container" style="margin-bottom: 20px;">
      <div class="stats-grid">
        <div class="stat-item">
          <el-icon class="stat-icon" color="#409EFF" size="24"><User /></el-icon>
          <div class="stat-content">
            <div class="stat-number">{{ filteredYazarlar.length }}</div>
            <div class="stat-label">Toplam Yazar</div>
          </div>
        </div>
        <div class="stat-item">
          <el-icon class="stat-icon" color="#67C23A" size="24"><Reading /></el-icon>
          <div class="stat-content">
            <div class="stat-number">{{ totalBooks }}</div>
            <div class="stat-label">Toplam Kitap</div>
          </div>
        </div>
        <div class="stat-item">
          <el-icon class="stat-icon" color="#E6A23C" size="24"><Location /></el-icon>
          <div class="stat-content">
            <div class="stat-number">{{ uniqueCountries.length }}</div>
            <div class="stat-label">Farklı Ülke</div>
          </div>
        </div>
      </div>
    </el-card>

    <!-- Filters -->
    <el-card class="filters-container" style="margin-bottom: 20px;">
      <div class="filters-row">
        <el-select
          v-model="selectedCountry"
          placeholder="Ülkeye göre filtrele"
          style="width: 200px; margin-right: 10px;"
          clearable
          @change="applyFilters"
        >
          <el-option
            v-for="country in uniqueCountries"
            :key="country"
            :label="country"
            :value="country"
          />
        </el-select>
        
        <el-select
          v-model="sortBy"
          placeholder="Sıralama"
          style="width: 150px; margin-right: 10px;"
          @change="applySorting"
        >
          <el-option label="Ad (A-Z)" value="name-asc" />
          <el-option label="Ad (Z-A)" value="name-desc" />
          <el-option label="Ülke" value="country" />
          <el-option label="Doğum Tarihi" value="birth-date" />
        </el-select>

        <el-button @click="clearFilters" type="info" plain>
          Filtreleri Temizle
        </el-button>
      </div>
    </el-card>

    <!-- Yazarlar Grid -->
    <div class="yazarlar-grid" v-loading="loading">
      <el-card 
        v-for="yazar in paginatedYazarlar" 
        :key="yazar.id" 
        class="yazar-card"
        shadow="hover"
      >
        <div class="yazar-header">
          <div class="yazar-avatar">
            <el-icon size="40" color="#409EFF"><User /></el-icon>
          </div>
          <div class="yazar-info">
            <h3 class="yazar-name">{{ yazar.ad }} {{ yazar.soyad }}</h3>
            <p class="yazar-country">
              <el-icon size="14"><Location /></el-icon>
              {{ yazar.ulke }}
            </p>
          </div>
        </div>
        
        <div class="yazar-details">
          <p class="birth-date" v-if="yazar.dogumTarihi">
            <strong>Doğum Tarihi:</strong> {{ formatDate(yazar.dogumTarihi) }}
          </p>
          <p class="book-count">
            <strong>Kitap Sayısı:</strong> {{ yazar.kitaplar?.length || 0 }}
          </p>
        </div>

        <!-- Yazarın Kitapları -->
        <div class="yazar-books" v-if="yazar.kitaplar && yazar.kitaplar.length > 0">
          <h4>Kitapları:</h4>
          <div class="books-list">
            <el-tag 
              v-for="kitap in yazar.kitaplar.slice(0, 3)" 
              :key="kitap.id"
              class="book-tag"
              :type="kitap.musaitMi ? 'success' : 'info'"
            >
              {{ kitap.baslik }}
            </el-tag>
            <span v-if="yazar.kitaplar.length > 3" class="more-books">
              +{{ yazar.kitaplar.length - 3 }} daha...
            </span>
          </div>
        </div>

        <div class="card-actions">
          <el-button size="small" type="primary" @click="viewYazarDetails(yazar)">
            Detayları Gör
          </el-button>
        </div>
      </el-card>
    </div>

    <!-- Empty State -->
    <el-empty 
      v-if="!loading && filteredYazarlar.length === 0" 
      description="Yazar bulunamadı" 
    />

    <!-- Pagination -->
    <div class="pagination-container" v-if="filteredYazarlar.length > pageSize">
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="pageSize"
        :total="filteredYazarlar.length"
        layout="total, prev, pager, next, jumper"
        @current-change="handlePageChange"
      />
    </div>

    <!-- Yazar Detay Dialog -->
    <el-dialog
      v-model="showDetailDialog"
      :title="`${selectedYazar?.ad} ${selectedYazar?.soyad}`"
      width="600px"
    >
      <div v-if="selectedYazar" class="yazar-detail-content">
        <div class="detail-section">
          <h4>Genel Bilgiler</h4>
          <p><strong>Ad Soyad:</strong> {{ selectedYazar.ad }} {{ selectedYazar.soyad }}</p>
          <p><strong>Ülke:</strong> {{ selectedYazar.ulke }}</p>
          <p v-if="selectedYazar.dogumTarihi">
            <strong>Doğum Tarihi:</strong> {{ formatDate(selectedYazar.dogumTarihi) }}
          </p>
        </div>
        
        <div class="detail-section" v-if="selectedYazar.kitaplar?.length > 0">
          <h4>Kitapları ({{ selectedYazar.kitaplar.length }})</h4>
          <div class="books-detail-list">
            <div 
              v-for="kitap in selectedYazar.kitaplar" 
              :key="kitap.id"
              class="book-item"
            >
              <div class="book-info">
                <span class="book-title">{{ kitap.baslik }}</span>
                <span class="book-year">({{ new Date(kitap.yayinTarihi).getFullYear() }})</span>
              </div>
              <el-tag 
                size="small" 
                :type="kitap.musaitMi ? 'success' : 'info'"
              >
                {{ kitap.musaitMi ? 'Müsait' : 'Dolu' }}
              </el-tag>
            </div>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { yazarService } from '@/services/api'
import { 
  User, 
  Reading, 
  Location, 
  Search, 
  RefreshRight 
} from '@element-plus/icons-vue'

export default {
  name: 'YazarlarView',
  data() {
    return {
      yazarlar: [],
      filteredYazarlar: [],
      loading: false,
      searchQuery: '',
      selectedCountry: '',
      sortBy: 'name-asc',
      currentPage: 1,
      pageSize: 12,
      showDetailDialog: false,
      selectedYazar: null,
      searchTimeout: null
    };
  },
  computed: {
    totalBooks() {
      return this.yazarlar.reduce((total, yazar) => {
        return total + (yazar.kitaplar?.length || 0);
      }, 0);
    },
    uniqueCountries() {
      const countries = [...new Set(this.yazarlar.map(yazar => yazar.ulke))];
      return countries.filter(country => country).sort();
    },
    paginatedYazarlar() {
      const start = (this.currentPage - 1) * this.pageSize;
      const end = start + this.pageSize;
      return this.filteredYazarlar.slice(start, end);
    }
  },
  created() {
    this.fetchYazarlar();
  },
  methods: {
    async fetchYazarlar() {
      try {
        this.loading = true;
        const response = await yazarService.getWithBooks();
        this.yazarlar = response || [];
        this.filteredYazarlar = [...this.yazarlar];
        this.applySorting();
      } catch (error) {
        console.error('Yazarlar getirilirken hata:', error);
        this.$message.error('Yazarlar yüklenirken hata oluştu');
      } finally {
        this.loading = false;
      }
    },
    
    async refreshData() {
      await this.fetchYazarlar();
      this.$message.success('Veriler güncellendi');
    },

    handleSearch() {
      if (this.searchTimeout) {
        clearTimeout(this.searchTimeout);
      }
      
      this.searchTimeout = setTimeout(() => {
        this.applyFilters();
      }, 300);
    },

    applyFilters() {
      let filtered = [...this.yazarlar];

      // Search filter
      if (this.searchQuery.trim()) {
        const query = this.searchQuery.toLowerCase().trim();
        filtered = filtered.filter(yazar => 
          yazar.ad.toLowerCase().includes(query) ||
          yazar.soyad.toLowerCase().includes(query) ||
          yazar.ulke.toLowerCase().includes(query)
        );
      }

      // Country filter
      if (this.selectedCountry) {
        filtered = filtered.filter(yazar => yazar.ulke === this.selectedCountry);
      }

      this.filteredYazarlar = filtered;
      this.currentPage = 1;
      this.applySorting();
    },

    applySorting() {
      switch (this.sortBy) {
        case 'name-asc':
          this.filteredYazarlar.sort((a, b) => 
            `${a.ad} ${a.soyad}`.localeCompare(`${b.ad} ${b.soyad}`, 'tr')
          );
          break;
        case 'name-desc':
          this.filteredYazarlar.sort((a, b) => 
            `${b.ad} ${b.soyad}`.localeCompare(`${a.ad} ${a.soyad}`, 'tr')
          );
          break;
        case 'country':
          this.filteredYazarlar.sort((a, b) => 
            a.ulke.localeCompare(b.ulke, 'tr')
          );
          break;
        case 'birth-date':
          this.filteredYazarlar.sort((a, b) => 
            new Date(a.dogumTarihi) - new Date(b.dogumTarihi)
          );
          break;
      }
    },

    clearFilters() {
      this.searchQuery = '';
      this.selectedCountry = '';
      this.sortBy = 'name-asc';
      this.currentPage = 1;
      this.applyFilters();
    },

    handlePageChange(page) {
      this.currentPage = page;
    },

    viewYazarDetails(yazar) {
      this.selectedYazar = yazar;
      this.showDetailDialog = true;
    },

    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString('tr-TR');
    }
  },
  setup() {
    return {
      User,
      Reading,
      Location,
      Search,
      RefreshRight
    }
  }
}
</script>

<style scoped>
.yazarlar-page {
  padding: 20px;
}

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header-actions {
  display: flex;
  align-items: center;
}

.stats-container, .filters-container {
  margin-bottom: 20px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 12px;
}

.stat-icon {
  font-size: 24px;
}

.stat-content {
  flex: 1;
}

.stat-number {
  font-size: 24px;
  font-weight: bold;
  color: #2c3e50;
}

.stat-label {
  font-size: 14px;
  color: #7f8c8d;
  margin-top: 2px;
}

.filters-row {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 10px;
}

.yazarlar-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
  margin-bottom: 20px;
}

.yazar-card {
  transition: transform 0.2s;
}

.yazar-card:hover {
  transform: translateY(-2px);
}

.yazar-header {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
}

.yazar-avatar {
  margin-right: 15px;
}

.yazar-info {
  flex: 1;
}

.yazar-name {
  margin: 0 0 5px 0;
  color: #2c3e50;
  font-size: 18px;
}

.yazar-country {
  margin: 0;
  color: #7f8c8d;
  display: flex;
  align-items: center;
  gap: 5px;
}

.yazar-details {
  margin-bottom: 15px;
}

.yazar-details p {
  margin: 5px 0;
  font-size: 14px;
  color: #606266;
}

.yazar-books {
  margin-bottom: 15px;
}

.yazar-books h4 {
  margin: 0 0 10px 0;
  font-size: 14px;
  color: #2c3e50;
}

.books-list {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
  align-items: center;
}

.book-tag {
  font-size: 12px;
}

.more-books {
  font-size: 12px;
  color: #909399;
}

.card-actions {
  display: flex;
  justify-content: flex-end;
}

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: 20px;
}

.yazar-detail-content {
  max-height: 500px;
  overflow-y: auto;
}

.detail-section {
  margin-bottom: 20px;
}

.detail-section h4 {
  margin: 0 0 10px 0;
  color: #2c3e50;
  border-bottom: 1px solid #ebeef5;
  padding-bottom: 5px;
}

.detail-section p {
  margin: 8px 0;
  color: #606266;
}

.books-detail-list {
  max-height: 200px;
  overflow-y: auto;
}

.book-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px solid #f5f7fa;
}

.book-item:last-child {
  border-bottom: none;
}

.book-info {
  flex: 1;
}

.book-title {
  font-weight: 500;
  color: #2c3e50;
}

.book-year {
  color: #909399;
  font-size: 12px;
  margin-left: 8px;
}

@media (max-width: 768px) {
  .header-section {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .header-actions {
    justify-content: center;
  }
  
  .yazarlar-grid {
    grid-template-columns: 1fr;
  }
  
  .filters-row {
    justify-content: center;
  }
}
</style>