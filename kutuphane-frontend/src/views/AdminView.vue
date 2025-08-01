<template>
  <div class="admin-dashboard">
    <!-- Header -->
    <div class="dashboard-header">
      <div class="header-content">
        <h1>Admin Yönetim Paneli</h1>
        <p>Kütüphane yönetimi ve istatistikleri</p>
      </div>
      <div class="header-actions">
        <el-button type="primary" @click="refreshAllData" :loading="loading">
          <el-icon><RefreshRight /></el-icon>
          Verileri Yenile
        </el-button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <el-skeleton :rows="8" animated />
    </div>

    <!-- Error State -->
    <el-alert
      v-else-if="error"
      :title="error"
      type="error"
      show-icon
      @close="refreshAllData"
      style="margin-bottom: 20px;"
    />

    <!-- Dashboard Content -->
    <div v-else>
      <!-- Stats Cards -->
      <el-row :gutter="20" class="stats-section">
        <el-col :xs="24" :sm="12" :md="6">
          <el-card class="stat-card books">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon size="40" color="#409EFF"><Reading /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ adminStats.toplamKitapSayisi || 0 }}</div>
                <div class="stat-label">Toplam Kitap</div>
              </div>
            </div>
          </el-card>
        </el-col>

        <el-col :xs="24" :sm="12" :md="6">
          <el-card class="stat-card users">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon size="40" color="#67C23A"><User /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ adminStats.toplamKullaniciSayisi || 0 }}</div>
                <div class="stat-label">Toplam Kullanıcı</div>
              </div>
            </div>
          </el-card>
        </el-col>

        <el-col :xs="24" :sm="12" :md="6">
          <el-card class="stat-card loans">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon size="40" color="#E6A23C"><SwitchButton /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ adminStats.aktifOduncSayisi || 0 }}</div>
                <div class="stat-label">Aktif Ödünç</div>
              </div>
            </div>
          </el-card>
        </el-col>

        <el-col :xs="24" :sm="12" :md="6">
          <el-card class="stat-card overdue">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon size="40" color="#F56C6C"><Warning /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-number">{{ adminStats.toplamGecikenIadeSayisi || 0 }}</div>
                <div class="stat-label">Geciken İade</div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>

      <!-- Financial Stats -->
      <el-row :gutter="20" style="margin-top: 20px;">
        <el-col :xs="24" :sm="12" :md="8">
          <el-card class="financial-card">
            <div class="financial-content">
              <div class="financial-icon">
                <el-icon size="30" color="#F56C6C"><Money /></el-icon>
              </div>
              <div class="financial-info">
                <div class="financial-number">{{ adminStats.toplamCezaGeliri || 0 }}₺</div>
                <div class="financial-label">Toplam Ceza Geliri</div>
              </div>
            </div>
          </el-card>
        </el-col>
        
        <el-col :xs="24" :sm="12" :md="8">
          <el-card class="financial-card">
            <div class="financial-content">
              <div class="financial-icon">
                <el-icon size="30" color="#67C23A"><TrendCharts /></el-icon>
              </div>
              <div class="financial-info">
                <div class="financial-number">{{ adminStats.buAyOduncSayisi || 0 }}</div>
                <div class="financial-label">Bu Ay Ödünç</div>
              </div>
            </div>
          </el-card>
        </el-col>

        <el-col :xs="24" :sm="12" :md="8">
          <el-card class="financial-card">
            <div class="financial-content">
              <div class="financial-icon">
                <el-icon size="30" color="#409EFF"><DataAnalysis /></el-icon>
              </div>
              <div class="financial-info">
                <div class="financial-number">{{ adminStats.musaitKitapSayisi || 0 }}</div>
                <div class="financial-label">Müsait Kitap</div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>

      <!-- Quick Actions -->
      <el-card class="quick-actions-card" style="margin-top: 20px;">
        <template #header>
          <div class="card-header">
            <span>Hızlı İşlemler</span>
            <el-icon color="#409EFF"><Operation /></el-icon>
          </div>
        </template>

        <el-row :gutter="20">
          <el-col :xs="24" :sm="12" :md="6">
            <el-button class="action-button" type="primary" @click="showAddBookDialog">
              <el-icon><Plus /></el-icon>
              Yeni Kitap Ekle
            </el-button>
          </el-col>
          <el-col :xs="24" :sm="12" :md="6">
            <el-button class="action-button" type="success" @click="showLoanDialog">
              <el-icon><SwitchButton /></el-icon>
              Ödünç Ver
            </el-button>
          </el-col>
          <el-col :xs="24" :sm="12" :md="6">
            <el-button class="action-button" type="warning" @click="viewOverdueBooks">
              <el-icon><Warning /></el-icon>
              Geciken Kitaplar
            </el-button>
          </el-col>
          <el-col :xs="24" :sm="12" :md="6">
            <el-button class="action-button" type="info" @click="viewAllLoans">
              <el-icon><DataAnalysis /></el-icon>
              Tüm Ödünçler
            </el-button>
          </el-col>
        </el-row>
      </el-card>

      <!-- Content Tabs -->
      <el-card style="margin-top: 20px;">
        <el-tabs v-model="activeTab" type="card" @tab-change="handleTabChange">
          <!-- Recent Loans Tab -->
          <el-tab-pane label="Son Ödünçler" name="loans">
            <div class="tab-content">
              <div class="table-header">
                <h3>Son Ödünç İşlemleri</h3>
                <el-button size="small" @click="refreshLoans">
                  <el-icon><RefreshRight /></el-icon>
                  Yenile
                </el-button>
              </div>
              
              <el-table :data="recentLoans" style="width: 100%" v-loading="loansLoading">
                <el-table-column prop="kullaniciAdSoyad" label="Kullanıcı" />
                <el-table-column prop="kitapBaslik" label="Kitap" />
                <el-table-column prop="oduncAlinmaTarihi" label="Alınma Tarihi">
                  <template #default="scope">
                    {{ formatDate(scope.row.oduncAlinmaTarihi) }}
                  </template>
                </el-table-column>
                <el-table-column prop="geriVerilmesiGerekenTarih" label="İade Tarihi">
                  <template #default="scope">
                    {{ formatDate(scope.row.geriVerilmesiGerekenTarih) }}
                  </template>
                </el-table-column>
                <el-table-column prop="durumu" label="Durum">
                  <template #default="scope">
                    <el-tag 
                      :type="scope.row.durumu === 'İade Edildi' ? 'success' : 
                             scope.row.durumu === 'Gecikmiş' ? 'danger' : 'warning'"
                    >
                      {{ scope.row.durumu }}
                    </el-tag>
                  </template>
                </el-table-column>
                <el-table-column label="İşlemler" width="120">
                  <template #default="scope">
                    <el-button 
                      v-if="!scope.row.iadeEdildiMi"
                      size="small" 
                      type="success" 
                      @click="processReturn(scope.row)"
                    >
                      İade Al
                    </el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </el-tab-pane>

          <!-- Low Stock Tab -->
          <el-tab-pane label="Stok Uyarıları" name="stock">
            <div class="tab-content">
              <div class="table-header">
                <h3>Stok Durumu</h3>
                <el-button size="small" @click="refreshStock">
                  <el-icon><RefreshRight /></el-icon>
                  Yenile
                </el-button>
              </div>

              <el-alert
                title="Stok Uyarısı"
                :description="`${lowStockBooks.length} kitapta stok azlığı tespit edildi`"
                type="warning"
                show-icon
                style="margin-bottom: 20px;"
                v-if="lowStockBooks.length > 0"
              />

              <el-table :data="lowStockBooks" style="width: 100%" v-loading="stockLoading">
                <el-table-column prop="baslik" label="Kitap Adı" />
                <el-table-column prop="yazarAdSoyad" label="Yazar" />
                <el-table-column prop="kategoriAdi" label="Kategori" />
                <el-table-column label="Stok Durumu" align="center">
                  <template #default="scope">
                    <el-tag :type="scope.row.musaitStok === 0 ? 'danger' : 'warning'">
                      {{ scope.row.musaitStok }}/{{ scope.row.toplamStok }}
                    </el-tag>
                  </template>
                </el-table-column>
                <el-table-column prop="konum" label="Konum" />
                <el-table-column label="İşlemler" width="120">
                  <template #default="scope">
                    <el-button size="small" type="primary" @click="editBook(scope.row)">
                      Düzenle
                    </el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </el-tab-pane>

          <!-- Popular Books Tab -->
          <el-tab-pane label="Popüler Kitaplar" name="popular">
            <div class="tab-content">
              <div class="table-header">
                <h3>En Popüler Kitaplar</h3>
              </div>

              <div class="popular-books-grid">
                <div 
                  v-for="(book, index) in popularBooks" 
                  :key="book.kitapAdi"
                  class="popular-book-item"
                >
                  <div class="book-rank">{{ index + 1 }}</div>
                  <div class="book-cover">
                    <el-image
                      :src="book.kapakUrl || '/default-book-cover.jpg'"
                      :alt="book.kitapAdi"
                      style="width: 60px; height: 80px; border-radius: 4px;"
                      fit="cover"
                    >
                      <template #error>
                        <div class="image-slot">
                          <el-icon><Picture /></el-icon>
                        </div>
                      </template>
                    </el-image>
                  </div>
                  <div class="book-info">
                    <h4>{{ book.kitapAdi }}</h4>
                    <p>{{ book.yazarAdi }}</p>
                    <small>{{ book.kategoriAdi }}</small>
                  </div>
                  <div class="book-stats">
                    <el-tag type="success">{{ book.oduncSayisi }} ödünç</el-tag>
                    <el-tag :type="book.isMusait ? 'success' : 'danger'" size="small">
                      {{ book.isMusait ? 'Müsait' : 'Dolu' }}
                    </el-tag>
                  </div>
                </div>
              </div>
            </div>
          </el-tab-pane>
        </el-tabs>
      </el-card>
    </div>

    <!-- Loan Dialog -->
    <el-dialog v-model="loanDialogVisible" title="Ödünç Ver" width="500px">
      <el-form :model="loanForm" label-width="120px">
        <el-form-item label="Kullanıcı">
          <el-select v-model="loanForm.kullaniciId" placeholder="Kullanıcı seçin" filterable>
            <el-option
              v-for="user in users"
              :key="user.id"
              :label="`${user.ad} ${user.soyad} (${user.email})`"
              :value="user.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="Kitap">
          <el-select v-model="loanForm.kitapId" placeholder="Kitap seçin" filterable>
            <el-option
              v-for="book in availableBooks"
              :key="book.id"
              :label="`${book.baslik} - ${book.yazarAdi}`"
              :value="book.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="loanDialogVisible = false">İptal</el-button>
          <el-button type="primary" @click="createLoan" :loading="loanProcessing">
            Ödünç Ver
          </el-button>
        </div>
      </template>
    </el-dialog>

    <!-- Add Book Dialog -->
    <AddBookDialog 
      v-model="addBookDialogVisible" 
      @book-added="refreshAllData" 
    />
  </div>
</template>

<script>
import { 
  Reading, 
  User, 
  SwitchButton, 
  Warning, 
  RefreshRight,
  Plus,
  Operation,
  Money,
  TrendCharts,
  DataAnalysis,
  Picture
} from '@element-plus/icons-vue'
import { 
  dashboardService, 
  oduncService, 
  kitapService, 
  kullaniciService 
} from '@/services/api'
import AddBookDialog from '@/views/AddBookDialog.vue'

export default {
  name: 'AdminDashboard',
  components: {
    AddBookDialog
  },
  data() {
    return {
      loading: false,
      error: null,
      activeTab: 'loans',
      
      // Stats data
      adminStats: {},
      popularBooks: [],
      
      // Table data
      recentLoans: [],
      lowStockBooks: [],
      loansLoading: false,
      stockLoading: false,
      
      // Loan dialog
      loanDialogVisible: false,
      loanForm: {
        kullaniciId: null,
        kitapId: null
      },
      loanProcessing: false,
      users: [],
      availableBooks: [],
      
      // Add book dialog
      addBookDialogVisible: false
    }
  },
  async created() {
    await this.loadInitialData()
  },
  methods: {
    async loadInitialData() {
      this.loading = true
      this.error = null
      
      try {
        // Load admin dashboard data
        await Promise.all([
          this.loadAdminStats(),
          this.loadPopularBooks(),
          this.loadRecentLoans()
        ])
      } catch (error) {
        console.error('Dashboard load error:', error)
        this.error = 'Dashboard verileri yüklenirken hata oluştu'
      } finally {
        this.loading = false
      }
    },

    async loadAdminStats() {
      try {
        const [fullDashboard, financialData] = await Promise.all([
          dashboardService.getFullDashboard(),
          dashboardService.getFinancialData()
        ])
        
        this.adminStats = {
          ...fullDashboard,
          ...financialData
        }
      } catch (error) {
        console.error('Admin stats error:', error)
        throw error
      }
    },

    async loadPopularBooks() {
      try {
        this.popularBooks = await dashboardService.getPopularBooks(8)
      } catch (error) {
        console.error('Popular books error:', error)
      }
    },

    async loadRecentLoans() {
      this.loansLoading = true
      try {
        const loans = await oduncService.getAll()
        this.recentLoans = loans.slice(0, 10) // Show last 10
      } catch (error) {
        console.error('Recent loans error:', error)
      } finally {
        this.loansLoading = false
      }
    },

    async loadLowStockBooks() {
      this.stockLoading = true
      try {
        const [stockOut, lowStock] = await Promise.all([
          kitapService.getStokBiten(),
          kitapService.getStokYakinBiten()
        ])
        this.lowStockBooks = [...stockOut, ...lowStock]
      } catch (error) {
        console.error('Low stock books error:', error)
      } finally {
        this.stockLoading = false
      }
    },

    async refreshAllData() {
      await this.loadInitialData()
      this.$message.success('Tüm veriler güncellendi')
    },

    async refreshLoans() {
      await this.loadRecentLoans()
      this.$message.success('Ödünç verileri güncellendi')
    },

    async refreshStock() {
      await this.loadLowStockBooks()
      this.$message.success('Stok verileri güncellendi')
    },

    async handleTabChange(tabName) {
      if (tabName === 'stock' && this.lowStockBooks.length === 0) {
        await this.loadLowStockBooks()
      }
    },

    async showLoanDialog() {
      this.loanDialogVisible = true
      
      // Load users and available books
      try {
        const [usersResponse, booksResponse] = await Promise.all([
          kullaniciService.getAll(),
          kitapService.getMusait()
        ])
        this.users = usersResponse
        this.availableBooks = booksResponse
      } catch (error) {
        this.$message.error('Veri yükleme hatası')
      }
    },

    async createLoan() {
      if (!this.loanForm.kullaniciId || !this.loanForm.kitapId) {
        this.$message.error('Lütfen kullanıcı ve kitap seçin')
        return
      }

      this.loanProcessing = true
      try {
        await oduncService.oduncVer({
          kullaniciId: this.loanForm.kullaniciId,
          kitapId: this.loanForm.kitapId
        })
        
        this.$message.success('Ödünç işlemi başarılı')
        this.loanDialogVisible = false
        this.loanForm = { kullaniciId: null, kitapId: null }
        
        // Refresh data
        await this.loadRecentLoans()
        await this.loadAdminStats()
      } catch (error) {
        this.$message.error('Ödünç işlemi başarısız')
      } finally {
        this.loanProcessing = false
      }
    },

    async processReturn(loan) {
      try {
        await oduncService.iade(loan.id)
        this.$message.success('İade işlemi başarılı')
        await this.loadRecentLoans()
        await this.loadAdminStats()
      } catch (error) {
        this.$message.error('İade işlemi başarısız')
      }
    },

    showAddBookDialog() {
      this.addBookDialogVisible = true
    },

    viewOverdueBooks() {
      this.$router.push('/admin/overdue-books')
    },

    viewAllLoans() {
      this.$router.push('/admin/all-loans')
    },

    editBook(book) {
      this.$message.info('Kitap düzenleme özelliği geliştirilecek')
    },

    formatDate(dateString) {
      if (!dateString) return ''
      return new Date(dateString).toLocaleDateString('tr-TR')
    }
  },
  setup() {
    return {
      Reading,
      User,
      SwitchButton,
      Warning,
      RefreshRight,
      Plus,
      Operation,
      Money,
      TrendCharts,
      DataAnalysis,
      Picture
    }
  }
}
</script>

<style scoped>
.admin-dashboard {
  padding: 24px;
  background-color: #f5f7fa;
  min-height: 100vh;
}

.dashboard-header {
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

.loading-container {
  padding: 40px;
  background: white;
  border-radius: 12px;
}

/* Stats Cards */
.stats-section {
  margin-bottom: 20px;
}

.stat-card {
  height: 120px;
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
  height: 100%;
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

/* Financial Cards */
.financial-card {
  height: 100px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.financial-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
}

.financial-content {
  display: flex;
  align-items: center;
  height: 100%;
  padding: 16px;
}

.financial-icon {
  margin-right: 12px;
}

.financial-number {
  font-size: 24px;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 4px;
}

.financial-label {
  color: #6b7280;
  font-size: 12px;
}

/* Quick Actions */
.quick-actions-card .card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.action-button {
  width: 100%;
  height: 60px;
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 10px;
}

/* Tab Content */
.tab-content {
  padding: 20px 0;
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.table-header h3 {
  margin: 0;
  color: #1f2937;
  font-size: 18px;
  font-weight: 600;
}

/* Popular Books Grid */
.popular-books-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 16px;
}

.popular-book-item {
  display: flex;
  align-items: center;
  background: white;
  padding: 16px;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
  transition: all 0.3s ease;
}

.popular-book-item:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}

.book-rank {
  font-size: 24px;
  font-weight: 700;
  color: #f59e0b;
  margin-right: 16px;
  min-width: 30px;
}

.book-cover {
  margin-right: 16px;
}

.book-info {
  flex: 1;
  margin-right: 16px;
}

.book-info h4 {
  margin: 0 0 4px 0;
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
}

.book-info p {
  margin: 0 0 4px 0;
  font-size: 14px;
  color: #6b7280;
}

.book-info small {
  font-size: 12px;
  color: #9ca3af;
}

.book-stats {
  display: flex;
  flex-direction: column;
  gap: 8px;
  align-items: flex-end;
}

.image-slot {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 60px;
  height: 80px;
  background-color: #f5f7fa;
  color: #909399;
  border-radius: 4px;
}

/* Dialog */
.dialog-footer {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

/* Responsive */
@media (max-width: 768px) {
  .admin-dashboard {
    padding: 16px;
  }

  .dashboard-header {
    flex-direction: column;
    gap: 16px;
    text-align: center;
  }

  .header-actions {
    width: 100%;
  }

  .popular-books-grid {
    grid-template-columns: 1fr;
  }

  .popular-book-item {
    flex-direction: column;
    text-align: center;
  }

  .book-rank {
    margin-right: 0;
    margin-bottom: 8px;
  }

  .book-cover {
    margin-right: 0;
    margin-bottom: 12px;
  }

  .book-info {
    margin-right: 0;
    margin-bottom: 12px;
  }
}
</style>