<template>
  <div class="overdue-books-page">
    <!-- Header -->
    <div class="page-header">
      <div class="header-content">
        <h1>Geciken Kitaplar</h1>
        <p>Süresi geçen ödünç kitapları yönetin</p>
      </div>
      <div class="header-actions">
        <el-button @click="refreshData" :loading="loading">
          <el-icon><RefreshRight /></el-icon>
          Yenile
        </el-button>
        <el-button type="primary" @click="sendReminders" :loading="sendingReminders">
          <el-icon><Message /></el-icon>
          Toplu Hatırlatma Gönder
        </el-button>
      </div>
    </div>

    <!-- Stats Cards -->
    <el-row :gutter="20" class="stats-section">
      <el-col :xs="24" :sm="8">
        <el-card class="stat-card overdue">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="40" color="#F56C6C"><Warning /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ overdueBooks.length }}</div>
              <div class="stat-label">Toplam Geciken</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="24" :sm="8">
        <el-card class="stat-card fine">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="40" color="#E6A23C"><Money /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ totalFines }}₺</div>
              <div class="stat-label">Toplam Ceza</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="24" :sm="8">
        <el-card class="stat-card critical">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="40" color="#F56C6C"><Timer /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ criticalOverdue }}</div>
              <div class="stat-label">Kritik Gecikme (30+ gün)</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- Filters -->
    <el-card class="filters-card">
      <el-row :gutter="20">
        <el-col :xs="24" :md="6">
          <el-input
            v-model="searchText"
            placeholder="Kullanıcı, kitap ara..."
            :prefix-icon="Search"
            clearable
            @input="applyFilters"
          />
        </el-col>
        <el-col :xs="24" :md="5">
          <el-select
            v-model="filterSeverity"
            placeholder="Gecikme Seviyesi"
            clearable
            @change="applyFilters"
          >
            <el-option label="Tümü" value="" />
            <el-option label="Hafif (1-7 gün)" value="light" />
            <el-option label="Orta (8-15 gün)" value="medium" />
            <el-option label="Ağır (16-30 gün)" value="heavy" />
            <el-option label="Kritik (30+ gün)" value="critical" />
          </el-select>
        </el-col>
        <el-col :xs="24" :md="5">
          <el-select
            v-model="sortBy"
            placeholder="Sıralama"
            @change="applySorting"
          >
            <el-option label="Gecikme Süresi (Çok → Az)" value="overdue-desc" />
            <el-option label="Gecikme Süresi (Az → Çok)" value="overdue-asc" />
            <el-option label="Ceza Miktarı (Yüksek → Düşük)" value="fine-desc" />
            <el-option label="Kullanıcı Adı (A-Z)" value="user-asc" />
          </el-select>
        </el-col>
        <el-col :xs="24" :md="4">
          <el-button @click="clearFilters">Temizle</el-button>
        </el-col>
        <el-col :xs="24" :md="4">
          <el-button type="success" @click="exportToExcel">
            <el-icon><Download /></el-icon>
            Excel'e Aktar
          </el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- Overdue Books Table -->
    <el-card class="table-card">
      <template #header>
        <div class="card-header">
          <span>Geciken Kitaplar ({{ filteredBooks.length }})</span>
          <div class="header-actions">
            <el-button-group>
              <el-button 
                size="small" 
                :type="selectedRows.length > 0 ? 'primary' : ''" 
                @click="processSelectedReturns"
                :disabled="selectedRows.length === 0"
              >
                Seçilenleri İade Al ({{ selectedRows.length }})
              </el-button>
              <el-button 
                size="small" 
                :type="selectedRows.length > 0 ? 'warning' : ''" 
                @click="sendSelectedReminders"
                :disabled="selectedRows.length === 0"
              >
                Seçilenlere Hatırlatma Gönder
              </el-button>
            </el-button-group>
          </div>
        </div>
      </template>

      <el-table 
        :data="paginatedBooks" 
        style="width: 100%"
        v-loading="loading"
        @selection-change="handleSelectionChange"
        row-key="id"
      >
        <el-table-column type="selection" width="55" />
        
        <!-- User Info -->
        <el-table-column label="Kullanıcı Bilgileri" min-width="200">
          <template #default="scope">
            <div class="user-info">
              <div class="user-name">{{ scope.row.kullaniciAdSoyad }}</div>
              <div class="user-email">{{ scope.row.kullaniciEmail || 'Email yok' }}</div>
            </div>
          </template>
        </el-table-column>

        <!-- Book Info -->
        <el-table-column label="Kitap Bilgileri" min-width="250">
          <template #default="scope">
            <div class="book-info">
              <div class="book-title">{{ scope.row.kitapBaslik }}</div>
              <div class="book-author">{{ scope.row.yazarAdSoyad }}</div>
            </div>
          </template>
        </el-table-column>

        <!-- Loan Date -->
        <el-table-column prop="oduncAlinmaTarihi" label="Alınma Tarihi" width="130">
          <template #default="scope">
            {{ formatDate(scope.row.oduncAlinmaTarihi) }}
          </template>
        </el-table-column>

        <!-- Due Date -->
        <el-table-column prop="geriVerilmesiGerekenTarih" label="İade Tarihi" width="130">
          <template #default="scope">
            {{ formatDate(scope.row.geriVerilmesiGerekenTarih) }}
          </template>
        </el-table-column>

        <!-- Days Overdue -->
        <el-table-column label="Gecikme" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getOverdueTagType(scope.row.gecikmeGunSayisi)">
              {{ scope.row.gecikmeGunSayisi }} gün
            </el-tag>
          </template>
        </el-table-column>

        <!-- Fine Amount -->
        <el-table-column label="Ceza" width="100" align="center">
          <template #default="scope">
            <div class="fine-amount">
              <strong>{{ scope.row.gecikmeCezasi }}₺</strong>
            </div>
          </template>
        </el-table-column>

        <!-- Severity -->
        <el-table-column label="Seviye" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getSeverityTagType(scope.row.gecikmeGunSayisi)" effect="dark">
              {{ getSeverityText(scope.row.gecikmeGunSayisi) }}
            </el-tag>
          </template>
        </el-table-column>

        <!-- Actions -->
        <el-table-column label="İşlemler" width="180" fixed="right">
          <template #default="scope">
            <el-button-group>
              <el-button 
                size="small" 
                type="success" 
                @click="processReturn(scope.row)"
                :loading="scope.row.processing"
              >
                İade Al
              </el-button>
              <el-button 
                size="small" 
                type="warning" 
                @click="sendReminder(scope.row)"
              >
                Hatırlat
              </el-button>
            </el-button-group>
          </template>
        </el-table-column>
      </el-table>

      <!-- Pagination -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 25, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="filteredBooks.length"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- Return Confirmation Dialog -->
    <el-dialog
      v-model="returnDialogVisible"
      title="İade Onayı"
      width="400px"
    >
      <div v-if="selectedReturnBook">
        <p>Bu kitabın iadesi işlensin mi?</p>
        <div class="return-info">
          <p><strong>Kitap:</strong> {{ selectedReturnBook.kitapBaslik }}</p>
          <p><strong>Kullanıcı:</strong> {{ selectedReturnBook.kullaniciAdSoyad }}</p>
          <p><strong>Gecikme:</strong> {{ selectedReturnBook.gecikmeGunSayisi }} gün</p>
          <p><strong>Ceza:</strong> {{ selectedReturnBook.gecikmeCezasi }}₺</p>
        </div>
      </div>
      
      <template #footer>
        <el-button @click="returnDialogVisible = false">İptal</el-button>
        <el-button type="primary" @click="confirmReturn" :loading="processingReturn">
          İadeyi Onayla
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { 
  Warning, 
  Money, 
  Timer, 
  RefreshRight, 
  Message, 
  Search, 
  Download 
} from '@element-plus/icons-vue'
import { oduncService } from '@/services/api'

export default {
  name: 'OverdueBooksView',
  data() {
    return {
      loading: false,
      sendingReminders: false,
      processingReturn: false,
      
      // Data
      overdueBooks: [],
      filteredBooks: [],
      
      // Filters
      searchText: '',
      filterSeverity: '',
      sortBy: 'overdue-desc',
      
      // Pagination
      currentPage: 1,
      pageSize: 25,
      
      // Selection
      selectedRows: [],
      
      // Dialogs
      returnDialogVisible: false,
      selectedReturnBook: null
    }
  },
  computed: {
    totalFines() {
      return this.overdueBooks.reduce((total, book) => total + book.gecikmeCezasi, 0)
    },
    criticalOverdue() {
      return this.overdueBooks.filter(book => book.gecikmeGunSayisi >= 30).length
    },
    paginatedBooks() {
      const start = (this.currentPage - 1) * this.pageSize
      const end = start + this.pageSize
      return this.filteredBooks.slice(start, end)
    }
  },
  async created() {
    await this.loadOverdueBooks()
  },
  methods: {
    async loadOverdueBooks() {
      this.loading = true
      try {
        this.overdueBooks = await oduncService.getSuresiDolan()
        this.filteredBooks = [...this.overdueBooks]
        this.applySorting()
      } catch (error) {
        console.error('Overdue books load error:', error)
        this.$message.error('Geciken kitaplar yüklenirken hata oluştu')
      } finally {
        this.loading = false
      }
    },

    async refreshData() {
      await this.loadOverdueBooks()
      this.$message.success('Veriler güncellendi')
    },

    applyFilters() {
      let filtered = [...this.overdueBooks]

      // Text search
      if (this.searchText) {
        const search = this.searchText.toLowerCase()
        filtered = filtered.filter(book => 
          book.kullaniciAdSoyad.toLowerCase().includes(search) ||
          book.kitapBaslik.toLowerCase().includes(search) ||
          book.yazarAdSoyad.toLowerCase().includes(search)
        )
      }

      // Severity filter
      if (this.filterSeverity) {
        filtered = filtered.filter(book => {
          const days = book.gecikmeGunSayisi
          switch (this.filterSeverity) {
            case 'light': return days >= 1 && days <= 7
            case 'medium': return days >= 8 && days <= 15
            case 'heavy': return days >= 16 && days <= 30
            case 'critical': return days > 30
            default: return true
          }
        })
      }

      this.filteredBooks = filtered
      this.currentPage = 1
      this.applySorting()
    },

    applySorting() {
      switch (this.sortBy) {
        case 'overdue-desc':
          this.filteredBooks.sort((a, b) => b.gecikmeGunSayisi - a.gecikmeGunSayisi)
          break
        case 'overdue-asc':
          this.filteredBooks.sort((a, b) => a.gecikmeGunSayisi - b.gecikmeGunSayisi)
          break
        case 'fine-desc':
          this.filteredBooks.sort((a, b) => b.gecikmeCezasi - a.gecikmeCezasi)
          break
        case 'user-asc':
          this.filteredBooks.sort((a, b) => a.kullaniciAdSoyad.localeCompare(b.kullaniciAdSoyad, 'tr'))
          break
      }
    },

    clearFilters() {
      this.searchText = ''
      this.filterSeverity = ''
      this.sortBy = 'overdue-desc'
      this.currentPage = 1
      this.applyFilters()
    },

    handleSelectionChange(selection) {
      this.selectedRows = selection
    },

    async processReturn(book) {
      this.selectedReturnBook = book
      this.returnDialogVisible = true
    },

    async confirmReturn() {
      if (!this.selectedReturnBook) return

      this.processingReturn = true
      try {
        await oduncService.iade(this.selectedReturnBook.id)
        this.$message.success('İade işlemi başarılı')
        this.returnDialogVisible = false
        await this.loadOverdueBooks()
      } catch (error) {
        this.$message.error('İade işlemi başarısız')
      } finally {
        this.processingReturn = false
      }
    },

    async processSelectedReturns() {
      if (this.selectedRows.length === 0) return

      try {
        const confirmResult = await this.$confirm(
          `${this.selectedRows.length} kitabın iadesi işlensin mi?`,
          'Toplu İade Onayı',
          {
            confirmButtonText: 'Evet',
            cancelButtonText: 'Hayır',
            type: 'warning'
          }
        )

        if (confirmResult) {
          const promises = this.selectedRows.map(book => oduncService.iade(book.id))
          await Promise.all(promises)
          
          this.$message.success(`${this.selectedRows.length} kitap başarıyla iade edildi`)
          await this.loadOverdueBooks()
        }
      } catch (error) {
        if (error !== 'cancel') {
          this.$message.error('Toplu iade işlemi başarısız')
        }
      }
    },

    async sendReminder(book) {
      this.$message.success(`${book.kullaniciAdSoyad} kullanıcısına hatırlatma gönderildi`)
      // In real implementation, this would call an API to send email/SMS
    },

    async sendSelectedReminders() {
      if (this.selectedRows.length === 0) return
      
      this.$message.success(`${this.selectedRows.length} kullanıcıya hatırlatma gönderildi`)
      // In real implementation, this would call an API for bulk reminders
    },

    async sendReminders() {
      this.sendingReminders = true
      try {
        this.$message.success(`${this.overdueBooks.length} kullanıcıya hatırlatma gönderildi`)
        // In real implementation, this would call an API for all overdue users
      } catch (error) {
        this.$message.error('Hatırlatma gönderimi başarısız')
      } finally {
        this.sendingReminders = false
      }
    },

    exportToExcel() {
      // Simple CSV export
      const csvContent = this.generateCSV()
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
      const link = document.createElement('a')
      const url = URL.createObjectURL(blob)
      link.setAttribute('href', url)
      link.setAttribute('download', `geciken-kitaplar-${new Date().toISOString().split('T')[0]}.csv`)
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      
      this.$message.success('Excel dosyası indirildi')
    },

    generateCSV() {
      const headers = ['Kullanıcı', 'Email', 'Kitap', 'Yazar', 'Alınma Tarihi', 'İade Tarihi', 'Gecikme (Gün)', 'Ceza (₺)']
      const rows = this.filteredBooks.map(book => [
        book.kullaniciAdSoyad,
        book.kullaniciEmail || '',
        book.kitapBaslik,
        book.yazarAdSoyad,
        this.formatDate(book.oduncAlinmaTarihi),
        this.formatDate(book.geriVerilmesiGerekenTarih),
        book.gecikmeGunSayisi,
        book.gecikmeCezasi
      ])
      
      return [headers, ...rows].map(row => row.join(',')).join('\n')
    },

    getOverdueTagType(days) {
      if (days <= 7) return 'warning'
      if (days <= 15) return 'danger'
      if (days <= 30) return 'danger'
      return 'danger'
    },

    getSeverityTagType(days) {
      if (days <= 7) return 'warning'
      if (days <= 15) return 'danger'
      if (days <= 30) return 'danger'
      return 'danger'
    },

    getSeverityText(days) {
      if (days <= 7) return 'Hafif'
      if (days <= 15) return 'Orta'
      if (days <= 30) return 'Ağır'
      return 'Kritik'
    },

    handleSizeChange(size) {
      this.pageSize = size
    },

    handleCurrentChange(page) {
      this.currentPage = page
    },

    formatDate(dateString) {
      if (!dateString) return ''
      return new Date(dateString).toLocaleDateString('tr-TR')
    }
  },
  setup() {
    return {
      Warning,
      Money,
      Timer,
      RefreshRight,
      Message,
      Search,
      Download
    }
  }
}
</script>

<style scoped>
.overdue-books-page {
  padding: 24px;
  background-color: #f5f7fa;
  min-height: 100vh;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 24px;
  background: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.header-content h1 {
  margin: 0 0 8px 0;
  color: #1f2937;
  font-size: 28px;
  font-weight: 600;
}

.header-content p {
  margin: 0;
  color: #6b7280;
  font-size: 16px;
}

.header-actions {
  display: flex;
  gap: 12px;
}

/* Stats Section */
.stats-section {
  margin-bottom: 24px;
}

.stat-card {
  cursor: pointer;
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
}

.stat-content {
  display: flex;
  align-items: center;
  padding: 20px;
}

.stat-icon {
  margin-right: 16px;
}

.stat-info {
  flex: 1;
}

.stat-number {
  font-size: 32px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 4px;
}

.stat-label {
  color: #6b7280;
  font-size: 14px;
  font-weight: 500;
}

/* Filters */
.filters-card {
  margin-bottom: 24px;
}

/* Table */
.table-card .card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.user-info {
  padding: 4px 0;
}

.user-name {
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 4px;
}

.user-email {
  font-size: 12px;
  color: #6b7280;
}

.book-info {
  padding: 4px 0;
}

.book-title {
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 4px;
}

.book-author {
  font-size: 14px;
  color: #6b7280;
}

.fine-amount {
  color: #ef4444;
  font-weight: 600;
}

.pagination-container {
  margin-top: 24px;
  text-align: center;
}

/* Return Dialog */
.return-info {
  background: #f8f9fa;
  padding: 16px;
  border-radius: 8px;
  margin: 16px 0;
}

.return-info p {
  margin: 8px 0;
  color: #6b7280;
}

/* Responsive */
@media (max-width: 768px) {
  .overdue-books-page {
    padding: 16px;
  }

  .page-header {
    flex-direction: column;
    gap: 16px;
    text-align: center;
  }

  .header-actions {
    width: 100%;
    justify-content: center;
    flex-wrap: wrap;
  }

  .table-card .card-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    justify-content: center;
  }
}
</style>