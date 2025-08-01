<template>
  <div class="kitap-page">
    <div v-if="loading" class="loading-container">
      <el-skeleton :rows="5" animated />
    </div>

    <div v-else>
      <div class="page-header">
        <div class="header-left">
          <h1>Kitap Kataloğu</h1>
          <p>Kütüphanedeki kitapları keşfedin</p>
        </div>
        <div class="header-actions">
          <el-button 
            :icon="Grid" 
            :type="viewMode === 'grid' ? 'primary' : ''" 
            @click="viewMode = 'grid'"
          >
            Izgara
          </el-button>
          <el-button 
            :icon="List" 
            :type="viewMode === 'list' ? 'primary' : ''" 
            @click="viewMode = 'list'"
          >
            Liste
          </el-button>
        </div>
      </div>

      <el-card class="filter-card">
        <el-row :gutter="20">
          <el-col :xs="24" :md="8">
            <el-input
              v-model="searchText"
              placeholder="Kitap, yazar veya ISBN ara..."
              :prefix-icon="Search"
              clearable
              @input="filterBooks"
            />
          </el-col>
          <el-col :xs="24" :md="6">
            <el-select 
              v-model="filterCategory" 
              placeholder="Kategori" 
              clearable 
              @change="filterBooks"
            >
              <el-option label="Tüm Kategoriler" value="" />
              <el-option 
                v-for="category in categories" 
                :key="category.id" 
                :label="category.ad" 
                :value="category.id" 
              />
            </el-select>
          </el-col>
          <el-col :xs="24" :md="6">
            <el-select 
              v-model="filterStatus" 
              placeholder="Durum" 
              clearable 
              @change="filterBooks"
            >
              <el-option label="Tümü" value="" />
              <el-option label="Müsait" value="musait" />
              <el-option label="Ödünçte" value="mesgul" />
            </el-select>
          </el-col>
          <el-col :xs="24" :md="4">
            <el-button @click="resetFilters">Temizle</el-button>
          </el-col>
        </el-row>
      </el-card>

      <el-row :gutter="20" class="stats-row">
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number">{{ totalBooks }}</div>
              <div class="stat-label">Toplam Kitap</div>
              <ElIcon class="stat-icon" color="#409EFF" size="50"><Reading />
              </ElIcon>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number available">{{ availableBooks }}</div>
              <div class="stat-label">Müsait Kitap</div>
              <ElIcon class="stat-icon" color="#67C23A" size="50"><User /></ElIcon>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number busy">{{ busyBooks }}</div>
              <div class="stat-label">Ödünçte</div>
              <ElIcon class="stat-icon" color="#E6A23C" size="50"><UserFilled /></ElIcon>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="12" :md="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-number warning">{{ categories.length }}</div>
              <div class="stat-label">Kategori</div>
              <ElIcon class="stat-icon" color="#F56C6C" size="50"><Collection /></ElIcon>
            </div>
          </el-card>
        </el-col>
      </el-row>

      <div v-if="viewMode === 'grid'">
        <el-row :gutter="20">
          <el-col 
            v-for="book in paginatedBooks" 
            :key="book.id"
            :xs="12" 
            :sm="8" 
            :md="6" 
            :lg="4"
            class="book-grid-item"
          >
            <el-card class="book-card" @click="viewBookDetail(book)">
              <div class="book-cover-container">
                <div class="book-cover-wrapper" @click.stop="previewImage(book)">
                  <el-image
                    :src="book.kapakUrl || '/default-book-cover.jpg'"
                    :alt="book.baslik"
                    class="book-cover"
                    fit="cover"
                    :hide-on-click-modal="true"
                    :preview-teleported="true"
                  >
                    <template #error>
                      <div class="image-slot">
                        <el-icon class="image-icon">
                          <Picture />
                        </el-icon>
                        <p>Kapak Yok</p>
                      </div>
                    </template>
                  </el-image>
                </div>
                
                <div class="status-badge">
                  <el-tag 
                    :type="book.musaitMi ? 'success' : 'danger'" 
                    size="small"
                    effect="dark"
                  >
                    {{ book.musaitMi ? 'Müsait' : 'Ödünçte' }}
                  </el-tag>
                </div>
              </div>

              <div class="book-info">
                <h3 class="book-title" :title="book.baslik">{{ book.baslik }}</h3>
                <p class="book-author">{{ book.yazarAdi || 'Bilinmeyen Yazar' }}</p>
                <p class="book-category">{{ book.kategoriAdi || 'Kategori Yok' }}</p>
                <div class="book-meta">
                  <span class="isbn">ISBN: {{ book.isbn }}</span>
                  <span class="location">{{ book.konum }}-{{ book.rafNo }}</span>
                </div>
                <div class="book-stock">
                  <el-progress 
                    :percentage="(book.musaitStok / book.toplamStok) * 100"
                    :stroke-width="6"
                    :show-text="false"
                    :color="getStockColor(book.musaitStok, book.toplamStok)"
                  />
                  <small>{{ book.musaitStok }}/{{ book.toplamStok }} müsait</small>
                </div>
              </div>
            </el-card>
          </el-col>
        </el-row>
      </div>

      <el-card v-else class="table-card">
        <template #header>
          <div class="card-header">
            <span>Kitap Listesi ({{ filteredBooks.length }})</span>
            <div class="header-actions">
              <el-button :icon="Refresh" @click="refreshData">Yenile</el-button>
            </div>
          </div>
        </template>

        <el-table 
          :data="paginatedBooks" 
          style="width: 100%"
          stripe
          @row-click="viewBookDetail"
          v-loading="tableLoading"
        >
          <el-table-column label="Kapak" width="80">
            <template #default="scope">
              <div class="table-image-container" @click.stop="previewImage(scope.row)">
                <el-image
                  :src="scope.row.kapakUrl || '/default-book-cover.jpg'"
                  :alt="scope.row.baslik"
                  style="width: 50px; height: 70px; border-radius: 4px; cursor: pointer;"
                  fit="cover"
                  :hide-on-click-modal="true"
                  :preview-teleported="true"
                >
                  <template #error>
                    <div class="table-image-slot">
                      <el-icon><Picture /></el-icon>
                    </div>
                  </template>
                </el-image>
              </div>
            </template>
          </el-table-column>

          <!-- Kitap Bilgileri -->
          <el-table-column prop="baslik" label="Kitap Bilgileri" min-width="250">
            <template #default="scope">
              <div class="book-details">
                <h4 class="table-book-title">{{ scope.row.baslik }}</h4>
                <p class="table-book-author">{{ scope.row.yazarAdi || 'Bilinmeyen Yazar' }}</p>
                <small class="table-book-isbn">ISBN: {{ scope.row.isbn }}</small>
              </div>
            </template>
          </el-table-column>

          <el-table-column prop="kategoriAdi" label="Kategori" width="120" />

          <el-table-column label="Stok Durumu" width="140" align="center">
            <template #default="scope">
              <div class="stock-info">
                <el-tag :type="getStockTagType(scope.row.stokDurumu)" size="small">
                  {{ scope.row.stokDurumu }}
                </el-tag>
                <small>{{ scope.row.musaitStok }}/{{ scope.row.toplamStok }}</small>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Konum" width="100" align="center">
            <template #default="scope">
              <el-tag type="info" size="small">
                {{ scope.row.konum }}-{{ scope.row.rafNo }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column prop="yayinTarihi" label="Yayın Yılı" width="100" align="center">
            <template #default="scope">
              {{ scope.row.yayinTarihi ? new Date(scope.row.yayinTarihi).getFullYear() : '-' }}
            </template>
          </el-table-column>

          <el-table-column label="İşlemler" width="120" fixed="right">
            <template #default="scope">
              <el-button 
                size="small" 
                :icon="View" 
                @click.stop="viewBookDetail(scope.row)"
                title="Detayları Görüntüle"
              >
                Detay
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- Pagination -->
        <div class="pagination-container">
          <el-pagination
            v-model:current-page="currentPage"
            v-model:page-size="pageSize"
            :page-sizes="[12, 24, 48, 96]"
            layout="total, sizes, prev, pager, next, jumper"
            :total="filteredBooks.length"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </el-card>
    </div>

    <!-- Kitap Detay Dialog -->
    <el-dialog
      v-model="detailDialogVisible"
      :title="selectedBook?.baslik"
      width="600px"
      @close="selectedBook = null"
    >
      <div v-if="selectedBook" class="book-detail">
        <el-row :gutter="20">
          <el-col :span="8">
            <div class="detail-image-container" @click="previewImage(selectedBook)">
              <el-image
                :src="selectedBook.kapakUrl || '/default-book-cover.jpg'"
                :alt="selectedBook.baslik"
                style="width: 100%; border-radius: 8px; cursor: pointer;"
                fit="cover"
                :hide-on-click-modal="true"
                :preview-teleported="true"
              >
                <template #error>
                  <div class="detail-image-slot">
                    <el-icon class="detail-image-icon">
                      <Picture />
                    </el-icon>
                    <p>Kapak Resmi Yok</p>
                  </div>
                </template>
              </el-image>
            </div>
          </el-col>
          <el-col :span="16">
            <el-descriptions :column="1" border>
              <el-descriptions-item label="Kitap Adı">
                {{ selectedBook.baslik }}
              </el-descriptions-item>
              <el-descriptions-item label="Yazar">
                {{ selectedBook.yazarAdi || 'Bilinmeyen' }}
              </el-descriptions-item>
              <el-descriptions-item label="Kategori">
                {{ selectedBook.kategoriAdi || 'Kategori Yok' }}
              </el-descriptions-item>
              <el-descriptions-item label="ISBN">
                {{ selectedBook.isbn }}
              </el-descriptions-item>
              <el-descriptions-item label="Sayfa Sayısı">
                {{ selectedBook.sayfaSayisi || '-' }}
              </el-descriptions-item>
              <el-descriptions-item label="Yayın Tarihi">
                {{ selectedBook.yayinTarihi ? new Date(selectedBook.yayinTarihi).toLocaleDateString('tr-TR') : '-' }}
              </el-descriptions-item>
              <el-descriptions-item label="Konum">
                {{ selectedBook.konum }}-{{ selectedBook.rafNo }}
              </el-descriptions-item>
              <el-descriptions-item label="Stok Durumu">
                <el-tag :type="getStockTagType(selectedBook.stokDurumu)">
                  {{ selectedBook.stokDurumu }}
                </el-tag>
                <span style="margin-left: 8px;">
                  ({{ selectedBook.musaitStok }}/{{ selectedBook.toplamStok }})
                </span>
              </el-descriptions-item>
            </el-descriptions>
          </el-col>
        </el-row>
      </div>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="detailDialogVisible = false">Kapat</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- Image Preview Dialog -->
    <el-dialog
      v-model="imagePreviewVisible"
      title="Kitap Kapağı"
      width="500px"
      :show-close="true"
      center
    >
      <div class="image-preview-container">
        <el-image
          :src="previewImageUrl"
          :alt="previewImageAlt"
          style="width: 100%; max-height: 600px;"
          fit="contain"
        >
          <template #error>
            <div class="preview-error-slot">
              <el-icon class="preview-error-icon">
                <Picture />
              </el-icon>
              <p>Resim yüklenemedi</p>
            </div>
          </template>
        </el-image>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { Search, Refresh, View, Grid, List, Picture } from '@element-plus/icons-vue'
import { yazarService, kategoriService } from '@/services/api'
import apiClient from '@/services/api'
import { ElIcon } from 'element-plus'

export default {
  name: 'KitapView',
  components: {
    Search, Refresh, View, Grid, List, Picture
  },
  data() {
    return {
      
      kitaplar: [],
      yazarlar: [],
      categories: [],
      filteredBooks: [],
      loading: false,
      tableLoading: false,

      
      viewMode: 'grid', 

      
      searchText: '',
      filterCategory: '',
      filterStatus: '',

      currentPage: 1,
      pageSize: 24,

      detailDialogVisible: false,
      selectedBook: null,

      imagePreviewVisible: false,
      previewImageUrl: '',
      previewImageAlt: ''
    }
  },
  computed: {
    totalBooks() {
      return this.kitaplar.length
    },
    availableBooks() {
      return this.kitaplar.filter(k => k.musaitMi).length
    },
    busyBooks() {
      return this.kitaplar.filter(k => !k.musaitMi).length
    },
    paginatedBooks() {
      const start = (this.currentPage - 1) * this.pageSize
      const end = start + this.pageSize
      return this.filteredBooks.slice(start, end)
    }
  },
  async created() {
    this.loading = true
    try {
      // First load authors and categories in parallel
      await Promise.all([
        this.loadYazarlar(),
        this.loadCategories()
      ])
      // Then load books and combine with author data
      await this.loadKitaplar()
      
      // Check for highlight query parameter
      this.checkHighlightBook()
    } finally {
      this.loading = false
    }
  },
  methods: {
    async loadKitaplar() {
      try {
        const response = await apiClient.get('/Kitap')
        this.kitaplar = this.combineBookData(response.data)
        this.filteredBooks = [...this.kitaplar]
      } catch (error) {
        this.$message.error('Kitaplar yüklenirken hata oluştu')
      }
    },

    async loadYazarlar() {
      try {
        this.yazarlar = await yazarService.getAll()
      } catch (error) {
        console.error('Yazarlar yüklenirken hata:', error)
      }
    },

    combineBookData(books) {
      return books.map(book => {
        const yazar = this.yazarlar.find(y => y.id === book.yazarId)
        const kategori = this.categories.find(k => k.id === book.kategoriId)
        return {
          ...book,
          yazarAdi: yazar ? `${yazar.ad} ${yazar.soyad}`.trim() : 'Bilinmeyen Yazar',
          kategoriAdi: kategori ? kategori.ad : 'Kategori Yok'
        }
      })
    },

    async refreshData() {
      this.tableLoading = true
      try {
        await Promise.all([
          this.loadYazarlar(),
          this.loadCategories()
        ])
        await this.loadKitaplar()
      } finally {
        this.tableLoading = false
      }
    },

    async loadCategories() {
      try {
        this.categories = await kategoriService.getAll()
      } catch (error) {
        console.error('Kategoriler yüklenirken hata:', error)
      }
    },

    filterBooks() {
      let filtered = [...this.kitaplar]

      // Metin arama
      if (this.searchText) {
        const search = this.searchText.toLowerCase()
        filtered = filtered.filter(book => 
          book.baslik.toLowerCase().includes(search) ||
          book.isbn.toLowerCase().includes(search) ||
          (book.yazarAdi && book.yazarAdi.toLowerCase().includes(search))
        )
      }

      // Kategori filtresi
      if (this.filterCategory) {
        filtered = filtered.filter(book => book.kategoriId === this.filterCategory)
      }

      // Durum filtresi
      if (this.filterStatus === 'musait') {
        filtered = filtered.filter(book => book.musaitMi)
      } else if (this.filterStatus === 'mesgul') {
        filtered = filtered.filter(book => !book.musaitMi)
      }

      this.filteredBooks = filtered
      this.currentPage = 1
    },

    resetFilters() {
      this.searchText = ''
      this.filterCategory = ''
      this.filterStatus = ''
      this.filteredBooks = [...this.kitaplar]
      this.currentPage = 1
    },

    getStockTagType(stokDurumu) {
      switch (stokDurumu) {
        case 'Müsait': return 'success'
        case 'Tükendi': return 'danger'
        case 'Az Kaldı': return 'warning'
        default: return 'info'
      }
    },

    getStockColor(musaitStok, toplamStok) {
      const percentage = (musaitStok / toplamStok) * 100
      if (percentage > 50) return '#67c23a'
      if (percentage > 20) return '#e6a23c'
      return '#f56c6c'
    },

    checkHighlightBook() {
      const highlightId = this.$route.query.highlight
      if (highlightId) {
        const bookToHighlight = this.kitaplar.find(book => book.id == highlightId)
        if (bookToHighlight) {
          // Open the book detail dialog
          this.viewBookDetail(bookToHighlight)
          
          // Remove the highlight query parameter to clean up URL
          this.$router.replace({ 
            name: 'kitaplar',
            query: { ...this.$route.query, highlight: undefined }
          })
        }
      }
    },

    viewBookDetail(book) {
      this.selectedBook = book
      this.detailDialogVisible = true
    },

    previewImage(book) {
      if (book.kapakUrl) {
        this.previewImageUrl = book.kapakUrl
        this.previewImageAlt = book.baslik
        this.imagePreviewVisible = true
      }
    },

    handleSizeChange(size) {
      this.pageSize = size
    },

    handleCurrentChange(page) {
      this.currentPage = page
    }
  }
}
</script>

<style scoped>
.kitap-page {
  padding: 24px;
  background-color: #f5f7fa;
  min-height: 100vh;
}

.loading-container {
  padding: 40px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 24px;
  background: white;
  padding: 24px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.header-left h1 {
  margin: 0 0 8px 0;
  color: #1f2937;
  font-size: 28px;
  font-weight: 600;
}

.header-left p {
  margin: 0;
  color: #6b7280;
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
}

.stat-content {
  padding: 16px 0;
}

.stat-number {
  font-size: 32px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 8px;
}

.stat-number.available { color: #10b981; }
.stat-number.busy { color: #f59e0b; }
.stat-number.warning { color: #ef4444; }

.stat-label {
  color: #6b7280;
  font-size: 14px;
}

/* Grid Görünümü */
.book-grid-item {
  margin-bottom: 20px;
}

.book-card {
  cursor: pointer;
  transition: all 0.3s ease;
  height: 100%;
}

.book-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
}

.book-cover-container {
  position: relative;
  margin-bottom: 16px;
}

.book-cover-wrapper {
  cursor: pointer;
}

.book-cover {
  width: 100%;
  height: 240px;
  border-radius: 8px;
}

.image-slot {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 240px;
  background-color: #f5f7fa;
  color: #909399;
  border-radius: 8px;
}

.image-icon {
  font-size: 48px;
  margin-bottom: 8px;
}

.status-badge {
  position: absolute;
  top: 8px;
  right: 8px;
}

.book-info {
  padding: 0 8px;
}

.book-title {
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 8px 0;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  line-clamp: 2;
  overflow: hidden;
}

.book-author {
  color: #6b7280;
  font-size: 14px;
  margin: 0 0 4px 0;
}

.book-category {
  color: #9ca3af;
  font-size: 12px;
  margin: 0 0 12px 0;
}

.book-meta {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 11px;
  color: #9ca3af;
  margin-bottom: 12px;
}

.book-meta .isbn,
.book-meta .location {
  word-break: break-all;
  line-height: 1.3;
}

.book-stock {
  margin-top: 12px;
}

.book-stock small {
  display: block;
  text-align: center;
  color: #6b7280;
  font-size: 11px;
  margin-top: 4px;
}

.table-card {
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.table-image-container {
  cursor: pointer;
}

.table-image-slot {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 50px;
  height: 70px;
  background-color: #f5f7fa;
  color: #909399;
  border-radius: 4px;
}

.book-details {
  padding: 4px 0;
}

.table-book-title {
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 4px 0;
  line-height: 1.3;
}

.table-book-author {
  color: #6b7280;
  font-size: 14px;
  margin: 0 0 4px 0;
}

.table-book-isbn {
  color: #9ca3af;
  font-size: 12px;
}

.stock-info {
  text-align: center;
}

.stock-info small {
  display: block;
  margin-top: 4px;
  color: #6b7280;
  font-size: 12px;
}

.pagination-container {
  margin-top: 24px;
  text-align: right;
}

/* Detay Dialog */
.book-detail {
  padding: 16px 0;
}

.detail-image-container {
  cursor: pointer;
}

.detail-image-slot {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 300px;
  background-color: #f5f7fa;
  color: #909399;
  border-radius: 8px;
}

.detail-image-icon {
  font-size: 64px;
  margin-bottom: 16px;
}

.dialog-footer {
  display: flex;
  gap: 12px;
}

.image-preview-container {
  text-align: center;
}

.preview-error-slot {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 300px;
  background-color: #f5f7fa;
  color: #909399;
  border-radius: 8px;
}

.preview-error-icon {
  font-size: 64px;
  margin-bottom: 16px;
}

@media (max-width: 768px) {
  .kitap-page {
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

  .book-cover {
    height: 200px;
  }

  .image-slot {
    height: 200px;
  }
}
</style>