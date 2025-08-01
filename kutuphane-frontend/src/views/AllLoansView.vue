<template>
  <div class="all-loans-page">
    <!-- Header -->
    <div class="page-header">
      <div class="header-content">
        <h1>Tüm Ödünç İşlemleri</h1>
        <p>Kütüphane ödünç geçmişini görüntüleyin ve yönetin</p>
      </div>
      <div class="header-actions">
        <el-button @click="refreshData" :loading="loading">
          <el-icon><RefreshRight /></el-icon>
          Yenile
        </el-button>
        <el-button type="success" @click="exportToExcel">
          <el-icon><Download /></el-icon>
          Excel'e Aktar
        </el-button>
      </div>
    </div>

    <!-- Stats Cards -->
    <el-row :gutter="20" class="stats-section">
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card total">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="32" color="#409EFF"><DataBoard /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ totalLoans }}</div>
              <div class="stat-label">Toplam Ödünç</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card active">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="32" color="#67C23A"><Reading /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ activeLoans }}</div>
              <div class="stat-label">Aktif Ödünç</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card returned">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="32" color="#67C23A"><CircleCheck /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ returnedLoans }}</div>
              <div class="stat-label">İade Edildi</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card overdue">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="32" color="#F56C6C"><Warning /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ overdueLoans }}</div>
              <div class="stat-label">Geciken</div>
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
            placeholder="Kullanıcı, kitap, yazar ara..."
            :prefix-icon="Search"
            clearable
            @input="applyFilters"
          />
        </el-col>
        
        <el-col :xs="24" :md="5">
          <el-select
            v-model="filterStatus"
            placeholder="Durum"
            clearable
            @change="applyFilters"
          >
            <el-option label="Tümü" value="" />
            <el-option label="Aktif" value="active" />
            <el-option label="İade Edildi" value="returned" />
            <el-option label="Geciken" value="overdue" />
            <el-option label="Zamanında" value="ontime" />
          </el-select>
        </el-col>
        
        <el-col :xs="24" :md="4">
          <el-select
            v-model="filterUser"
            placeholder="Kullanıcı"
            clearable
            filterable
            @change="applyFilters"
          >
            <el-option
              v-for="user in uniqueUsers"
              :key="user.id"
              :label="user.name"
              :value="user.id"
            />
          </el-select>
        </el-col>

        <el-col :xs="24" :md="5">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="→"
            start-placeholder="Başlangıç"
            end-placeholder="Bitiş"
            format="DD/MM/YYYY"
            value-format="YYYY-MM-DD"
            @change="applyFilters"
            style="width: 100%"
          />
        </el-col>
        
        <el-col :xs="24" :md="4">
          <el-button @click="clearFilters" style="width: 100%">
            Filtreleri Temizle
          </el-button>
        </el-col>
      </el-row>

      <!-- Advanced Filters Toggle -->
      <el-divider />
      <div class="advanced-filters">
        <el-button 
          type="text" 
          @click="showAdvancedFilters = !showAdvancedFilters"
          :icon="showAdvancedFilters ? ArrowUp : ArrowDown"
        >
          {{ showAdvancedFilters ? 'Basit Filtre' : 'Gelişmiş Filtreler' }}
        </el-button>
        
        <div v-show="showAdvancedFilters" class="advanced-options">
          <el-row :gutter="20" style="margin-top: 16px;">
            <el-col :xs="24" :md="6">
              <el-select
                v-model="sortBy"
                placeholder="Sıralama"
                @change="applySorting"
              >
                <el-option label="En Yeni → En Eski" value="date-desc" />
                <el-option label="En Eski → En Yeni" value="date-asc" />
                <el-option label="Kullanıcı Adı (A-Z)" value="user-asc" />
                <el-option label="Kitap Adı (A-Z)" value="book-asc" />
                <el-option label="İade Tarihi" value="due-date" />
                <el-option label="Gecikme Süresi" value="overdue" />
              </el-select>
            </el-col>
            
            <el-col :xs="24" :md="6">
              <el-input-number
                v-model="minOverdueDays"
                placeholder="Min gecikme günü"
                :min="0"
                :max="365"
                @change="applyFilters"
                style="width: 100%"
              />
            </el-col>
            
            <el-col :xs="24" :md="6">
              <el-input-number
                v-model="maxOverdueDays"
                placeholder="Max gecikme günü"
                :min="0"
                :max="365"
                @change="applyFilters"
                style="width: 100%"
              />
            </el-col>

            <el-col :xs="24" :md="6">
              <el-checkbox-group v-model="selectedCategories" @change="applyFilters">
                <el-checkbox label="Hızlı İstatistikler">
                  Sadece Bu Ay
                </el-checkbox>
              </el-checkbox-group>
            </el-col>
          </el-row>
        </div>
      </div>
    </el-card>

    <!-- Loans Table -->
    <el-card class="table-card">
      <template #header>
        <div class="card-header">
          <span>Ödünç İşlemleri ({{ filteredLoans.length }})</span>
          <div class="view-modes">
            <el-radio-group v-model="viewMode" size="small">
              <el-radio-button label="table">Tablo</el-radio-button>
              <el-radio-button label="timeline">Zaman Çizelgesi</el-radio-button>
            </el-radio-group>
          </div>
        </div>
      </template>

      <!-- Table View -->
      <div v-if="viewMode === 'table'">
        <el-table 
          :data="paginatedLoans" 
          style="width: 100%"
          v-loading="loading"
          @selection-change="handleSelectionChange"
          row-key="id"
        >
          <el-table-column type="selection" width="55" />
          
          <!-- User Info -->
          <el-table-column label="Kullanıcı" min-width="180">
            <template #default="scope">
              <div class="user-info">
                <div class="user-name">{{ scope.row.kullaniciAdSoyad }}</div>
                <div class="user-detail">{{ getUserEmail(scope.row.kullaniciId) }}</div>
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
          <el-table-column prop="oduncAlinmaTarihi" label="Alınma" width="110" align="center">
            <template #default="scope">
              <div class="date-info">
                {{ formatDate(scope.row.oduncAlinmaTarihi) }}
              </div>
            </template>
          </el-table-column>

          <!-- Due Date -->
          <el-table-column prop="geriVerilmesiGerekenTarih" label="İade Tarihi" width="110" align="center">
            <template #default="scope">
              <div class="date-info">
                {{ formatDate(scope.row.geriVerilmesiGerekenTarih) }}
              </div>
            </template>
          </el-table-column>

          <!-- Return Date -->
          <el-table-column label="İade Edilme" width="110" align="center">
            <template #default="scope">
              <div class="date-info">
                {{ scope.row.geriVerilisTarihi ? formatDate(scope.row.geriVerilisTarihi) : '-' }}
              </div>
            </template>
          </el-table-column>

          <!-- Status -->
          <el-table-column label="Durum" width="100" align="center">
            <template #default="scope">
              <el-tag :type="getStatusTagType(scope.row.durumu)">
                {{ scope.row.durumu }}
              </el-tag>
            </template>
          </el-table-column>

          <!-- Days/Fine -->
          <el-table-column label="Gecikme/Ceza" width="120" align="center">
            <template #default="scope">
              <div v-if="scope.row.gecikmeGunSayisi > 0" class="overdue-info">
                <div class="overdue-days">{{ scope.row.gecikmeGunSayisi }} gün</div>
                <div class="fine-amount">{{ scope.row.gecikmeCezasi }}₺</div>
              </div>
              <span v-else class="no-overdue">-</span>
            </template>
          </el-table-column>

          <!-- Actions -->
          <el-table-column label="İşlemler" width="120" fixed="right">
            <template #default="scope">
              <el-button 
                v-if="!scope.row.iadeEdildiMi"
                size="small" 
                type="success" 
                @click="processReturn(scope.row)"
              >
                İade Al
              </el-button>
              <el-button 
                size="small" 
                type="info" 
                @click="viewLoanDetails(scope.row)"
              >
                Detay
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- Timeline View -->
      <div v-else class="timeline-view">
        <el-timeline>
          <el-timeline-item
            v-for="loan in paginatedLoans"
            :key="loan.id"
            :timestamp="formatDate(loan.oduncAlinmaTarihi)"
            :type="getTimelineType(loan.durumu)"
          >
            <el-card class="timeline-card">
              <div class="timeline-content">
                <div class="timeline-header">
                  <h4>{{ loan.kitapBaslik }}</h4>
                  <el-tag :type="getStatusTagType(loan.durumu)" size="small">
                    {{ loan.durumu }}
                  </el-tag>
                </div>
                <div class="timeline-details">
                  <p><strong>Kullanıcı:</strong> {{ loan.kullaniciAdSoyad }}</p>
                  <p><strong>Yazar:</strong> {{ loan.yazarAdSoyad }}</p>
                  <p><strong>İade Tarihi:</strong> {{ formatDate(loan.geriVerilmesiGerekenTarih) }}</p>
                  <p v-if="loan.geriVerilisTarihi">
                    <strong>İade Edildi:</strong> {{ formatDate(loan.geriVerilisTarihi) }}
                  </p>
                  <p v-if="loan.gecikmeGunSayisi > 0" class="overdue-text">
                    <strong>Gecikme:</strong> {{ loan.gecikmeGunSayisi }} gün ({{ loan.gecikmeCezasi }}₺)
                  </p>
                </div>
                <div class="timeline-actions">
                  <el-button 
                    v-if="!loan.iadeEdildiMi"
                    size="small" 
                    type="success" 
                    @click="processReturn(loan)"
                  >
                    İade Al
                  </el-button>
                  <el-button 
                    size="small" 
                    type="info" 
                    @click="viewLoanDetails(loan)"
                  >
                    Detay
                  </el-button>
                </div>
              </div>
            </el-card>
          </el-timeline-item>
        </el-timeline>
      </div>

      <!-- Pagination -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 25, 50, 100, 200]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="filteredLoans.length"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- Loan Details Dialog -->
    <el-dialog
      v-model="detailDialogVisible"
      title="Ödünç Detayları"
      width="500px"
    >
      <div v-if="selectedLoan" class="loan-details">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="Kullanıcı">
            {{ selectedLoan.kullaniciAdSoyad }}
          </el-descriptions-item>
          <el-descriptions-item label="Kitap">
            {{ selectedLoan.kitapBaslik }}
          </el-descriptions-item>
          <el-descriptions-item label="Yazar">
            {{ selectedLoan.yazarAdSoyad }}
          </el-descriptions-item>
          <el-descriptions-item label="Alınma Tarihi">
            {{ formatDate(selectedLoan.oduncAlinmaTarihi) }}
          </el-descriptions-item>
          <el-descriptions-item label="İade Tarihi">
            {{ formatDate(selectedLoan.geriVerilmesiGerekenTarih) }}
          </el-descriptions-item>
          <el-descriptions-item label="İade Edilme Tarihi">
            {{ selectedLoan.geriVerilisTarihi ? formatDate(selectedLoan.geriVerilisTarihi) : 'Henüz iade edilmedi' }}
          </el-descriptions-item>
          <el-descriptions-item label="Durum">
            <el-tag :type="getStatusTagType(selectedLoan.durumu)">
              {{ selectedLoan.durumu }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="Gecikme" v-if="selectedLoan.gecikmeGunSayisi > 0">
            {{ selectedLoan.gecikmeGunSayisi }} gün
          </el-descriptions-item>
          <el-descriptions-item label="Ceza" v-if="selectedLoan.gecikmeCezasi > 0">
            {{ selectedLoan.gecikmeCezasi }}₺
          </el-descriptions-item>
        </el-descriptions>
      </div>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="detailDialogVisible = false">Kapat</el-button>
          <el-button 
            v-if="selectedLoan && !selectedLoan.iadeEdildiMi"
            type="success" 
            @click="processReturn(selectedLoan)"
          >
            İade Al
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { 
  RefreshRight, 
  Download, 
  Search,
  DataBoard,
  Reading,
  CircleCheck,
  Warning,
  ArrowUp,
  ArrowDown
} from '@element-plus/icons-vue'
import { oduncService, kullaniciService } from '@/services/api'

export default {
  name: 'AllLoansView',
  data() {
    return {
      loading: false,
      
      // Data
      allLoans: [],
      filteredLoans: [],
      users: [],
      
      // Filters
      searchText: '',
      filterStatus: '',
      filterUser: null,
      dateRange: null,
      minOverdueDays: null,
      maxOverdueDays: null,
      selectedCategories: [],
      showAdvancedFilters: false,
      
      // Sorting
      sortBy: 'date-desc',
      
      // View
      viewMode: 'table',
      
      // Pagination
      currentPage: 1,
      pageSize: 25,
      
      // Selection
      selectedRows: [],
      
      // Dialogs
      detailDialogVisible: false,
      selectedLoan: null
    }
  },
  computed: {
    totalLoans() {
      return this.allLoans.length
    },
    activeLoans() {
      return this.allLoans.filter(loan => !loan.iadeEdildiMi).length
    },
    returnedLoans() {
      return this.allLoans.filter(loan => loan.iadeEdildiMi).length
    },
    overdueLoans() {
      return this.allLoans.filter(loan => loan.durumu === 'Gecikmiş').length
    },
    uniqueUsers() {
      const userMap = new Map()
      this.allLoans.forEach(loan => {
        if (!userMap.has(loan.kullaniciId)) {
          userMap.set(loan.kullaniciId, {
            id: loan.kullaniciId,
            name: loan.kullaniciAdSoyad
          })
        }
      })
      return Array.from(userMap.values()).sort((a, b) => a.name.localeCompare(b.name, 'tr'))
    },
    paginatedLoans() {
      const start = (this.currentPage - 1) * this.pageSize
      const end = start + this.pageSize
      return this.filteredLoans.slice(start, end)
    }
  },
  async created() {
    await Promise.all([
      this.loadAllLoans(),
      this.loadUsers()
    ])
  },
  methods: {
    async loadAllLoans() {
      this.loading = true
      try {
        this.allLoans = await oduncService.getAll()
        this.filteredLoans = [...this.allLoans]
        this.applySorting()
      } catch (error) {
        console.error('All loans load error:', error)
        this.$message.error('Ödünç verileri yüklenirken hata oluştu')
      } finally {
        this.loading = false
      }
    },

    async loadUsers() {
      try {
        this.users = await kullaniciService.getAll()
      } catch (error) {
        console.error('Users load error:', error)
      }
    },

    async refreshData() {
      await this.loadAllLoans()
      this.$message.success('Veriler güncellendi')
    },

    applyFilters() {
      let filtered = [...this.allLoans]

      // Text search
      if (this.searchText) {
        const search = this.searchText.toLowerCase()
        filtered = filtered.filter(loan => 
          loan.kullaniciAdSoyad.toLowerCase().includes(search) ||
          loan.kitapBaslik.toLowerCase().includes(search) ||
          loan.yazarAdSoyad.toLowerCase().includes(search)
        )
      }

      // Status filter
      if (this.filterStatus) {
        switch (this.filterStatus) {
          case 'active':
            filtered = filtered.filter(loan => !loan.iadeEdildiMi)
            break
          case 'returned':
            filtered = filtered.filter(loan => loan.iadeEdildiMi)
            break
          case 'overdue':
            filtered = filtered.filter(loan => loan.durumu === 'Gecikmiş')
            break
          case 'ontime':
            filtered = filtered.filter(loan => loan.durumu === 'Zamanında')
            break
        }
      }

      // User filter
      if (this.filterUser) {
        filtered = filtered.filter(loan => loan.kullaniciId === this.filterUser)
      }

      // Date range filter
      if (this.dateRange && this.dateRange.length === 2) {
        const startDate = new Date(this.dateRange[0])
        const endDate = new Date(this.dateRange[1])
        filtered = filtered.filter(loan => {
          const loanDate = new Date(loan.oduncAlinmaTarihi)
          return loanDate >= startDate && loanDate <= endDate
        })
      }

      // Overdue days filter
      if (this.minOverdueDays !== null) {
        filtered = filtered.filter(loan => loan.gecikmeGunSayisi >= this.minOverdueDays)
      }
      if (this.maxOverdueDays !== null) {
        filtered = filtered.filter(loan => loan.gecikmeGunSayisi <= this.maxOverdueDays)
      }

      // This month filter
      if (this.selectedCategories.includes('Hızlı İstatistikler')) {
        const now = new Date()
        const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1)
        filtered = filtered.filter(loan => {
          const loanDate = new Date(loan.oduncAlinmaTarihi)
          return loanDate >= startOfMonth
        })
      }

      this.filteredLoans = filtered
      this.currentPage = 1
      this.applySorting()
    },

    applySorting() {
      switch (this.sortBy) {
        case 'date-desc':
          this.filteredLoans.sort((a, b) => new Date(b.oduncAlinmaTarihi) - new Date(a.oduncAlinmaTarihi))
          break
        case 'date-asc':
          this.filteredLoans.sort((a, b) => new Date(a.oduncAlinmaTarihi) - new Date(b.oduncAlinmaTarihi))
          break
        case 'user-asc':
          this.filteredLoans.sort((a, b) => a.kullaniciAdSoyad.localeCompare(b.kullaniciAdSoyad, 'tr'))
          break
        case 'book-asc':
          this.filteredLoans.sort((a, b) => a.kitapBaslik.localeCompare(b.kitapBaslik, 'tr'))
          break
        case 'due-date':
          this.filteredLoans.sort((a, b) => new Date(a.geriVerilmesiGerekenTarih) - new Date(b.geriVerilmesiGerekenTarih))
          break
        case 'overdue':
          this.filteredLoans.sort((a, b) => b.gecikmeGunSayisi - a.gecikmeGunSayisi)
          break
      }
    },

    clearFilters() {
      this.searchText = ''
      this.filterStatus = ''
      this.filterUser = null
      this.dateRange = null
      this.minOverdueDays = null
      this.maxOverdueDays = null
      this.selectedCategories = []
      this.sortBy = 'date-desc'
      this.currentPage = 1
      this.applyFilters()
    },

    handleSelectionChange(selection) {
      this.selectedRows = selection
    },

    async processReturn(loan) {
      try {
        await oduncService.iade(loan.id)
        this.$message.success('İade işlemi başarılı')
        this.detailDialogVisible = false
        await this.loadAllLoans()
      } catch (error) {
        this.$message.error('İade işlemi başarısız')
      }
    },

    viewLoanDetails(loan) {
      this.selectedLoan = loan
      this.detailDialogVisible = true
    },

    exportToExcel() {
      const csvContent = this.generateCSV()
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
      const link = document.createElement('a')
      const url = URL.createObjectURL(blob)
      link.setAttribute('href', url)
      link.setAttribute('download', `tum-odunc-islemleri-${new Date().toISOString().split('T')[0]}.csv`)
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      
      this.$message.success('Excel dosyası indirildi')
    },

    generateCSV() {
      const headers = [
        'Kullanıcı', 'Kitap', 'Yazar', 'Alınma Tarihi', 'İade Tarihi', 
        'İade Edilme Tarihi', 'Durum', 'Gecikme (Gün)', 'Ceza (₺)'
      ]
      const rows = this.filteredLoans.map(loan => [
        loan.kullaniciAdSoyad,
        loan.kitapBaslik,
        loan.yazarAdSoyad,
        this.formatDate(loan.oduncAlinmaTarihi),
        this.formatDate(loan.geriVerilmesiGerekenTarih),
        loan.geriVerilisTarihi ? this.formatDate(loan.geriVerilisTarihi) : '',
        loan.durumu,
        loan.gecikmeGunSayisi,
        loan.gecikmeCezasi
      ])
      
      return [headers, ...rows].map(row => row.join(',')).join('\n')
    },

    getUserEmail(userId) {
      const user = this.users.find(u => u.id === userId)
      return user ? user.email : ''
    },

    getStatusTagType(status) {
      switch (status) {
        case 'İade Edildi': return 'success'
        case 'Gecikmiş': return 'danger'
        case 'Zamanında': return 'primary'
        default: return 'info'
      }
    },

    getTimelineType(status) {
      switch (status) {
        case 'İade Edildi': return 'success'
        case 'Gecikmiş': return 'danger'
        case 'Zamanında': return 'primary'
        default: return 'info'
      }
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
      RefreshRight,
      Download,
      Search,
      DataBoard,
      Reading,
      CircleCheck,
      Warning,
      ArrowUp,
      ArrowDown
    }
  }
}
</script>

<style scoped>
.all-loans-page {
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
  padding: 16px;
}

.stat-icon {
  margin-right: 12px;
}

.stat-info {
  flex: 1;
}

.stat-number {
  font-size: 24px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 4px;
}

.stat-label {
  color: #6b7280;
  font-size: 12px;
  font-weight: 500;
}

/* Filters */
.filters-card {
  margin-bottom: 24px;
}

.advanced-filters {
  text-align: center;
}

.advanced-options {
  margin-top: 16px;
}

/* Table */
.table-card .card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.view-modes {
  display: flex;
  gap: 8px;
}

.user-info, .book-info {
  padding: 4px 0;
}

.user-name, .book-title {
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 4px;
}

.user-detail, .book-author {
  font-size: 12px;
  color: #6b7280;
}

.date-info {
  font-size: 13px;
  color: #374151;
}

.overdue-info {
  text-align: center;
}

.overdue-days {
  font-size: 12px;
  color: #ef4444;
  font-weight: 600;
}

.fine-amount {
  font-size: 11px;
  color: #dc2626;
  font-weight: 500;
}

.no-overdue {
  color: #6b7280;
}

/* Timeline View */
.timeline-view {
  padding: 20px;
}

.timeline-card {
  margin-bottom: 16px;
}

.timeline-content {
  padding: 16px;
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.timeline-header h4 {
  margin: 0;
  color: #1f2937;
  font-size: 16px;
}

.timeline-details p {
  margin: 6px 0;
  color: #6b7280;
  font-size: 14px;
}

.overdue-text {
  color: #ef4444 !important;
  font-weight: 500;
}

.timeline-actions {
  margin-top: 12px;
  display: flex;
  gap: 8px;
}

/* Pagination */
.pagination-container {
  margin-top: 24px;
  text-align: center;
}

/* Dialog */
.loan-details {
  padding: 16px 0;
}

.dialog-footer {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

/* Responsive */
@media (max-width: 768px) {
  .all-loans-page {
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
  }

  .table-card .card-header {
    flex-direction: column;
    gap: 12px;
    align-items: stretch;
  }

  .view-modes {
    justify-content: center;
  }

  .timeline-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
}
</style>