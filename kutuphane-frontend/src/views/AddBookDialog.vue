<template>
  <el-dialog
    v-model="visible"
    title="Yeni Kitap Ekle"
    width="600px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="bookFormRef"
      :model="bookForm"
      :rules="bookRules"
      label-width="140px"
      @submit.prevent="submitForm"
    >
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="Kitap Başlığı" prop="baslik">
            <el-input
              v-model="bookForm.baslik"
              placeholder="Kitap başlığını girin"
              maxlength="200"
              show-word-limit
            />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="ISBN" prop="isbn">
            <el-input
              v-model="bookForm.isbn"
              placeholder="ISBN numarası"
              maxlength="17"
            />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Sayfa Sayısı" prop="sayfaSayisi">
            <el-input-number
              v-model="bookForm.sayfaSayisi"
              :min="1"
              :max="9999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Yazar" prop="yazarId">
            <el-select
              v-model="bookForm.yazarId"
              placeholder="Yazar seçin"
              filterable
              style="width: 100%"
              @change="handleAuthorChange"
            >
              <el-option
                v-for="author in authors"
                :key="author.id"
                :label="`${author.ad} ${author.soyad}`"
                :value="author.id"
              />
            </el-select>
            <div class="form-helper">
              <el-button 
                size="small" 
                type="text" 
                @click="showAddAuthorDialog"
              >
                + Yeni Yazar Ekle
              </el-button>
            </div>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Kategori" prop="kategoriId">
            <el-select
              v-model="bookForm.kategoriId"
              placeholder="Kategori seçin"
              filterable
              style="width: 100%"
            >
              <el-option
                v-for="category in categories"
                :key="category.id"
                :label="category.ad"
                :value="category.id"
              />
            </el-select>
            <div class="form-helper">
              <el-button 
                size="small" 
                type="text" 
                @click="showAddCategoryDialog"
              >
                + Yeni Kategori Ekle
              </el-button>
            </div>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Yayın Tarihi" prop="yayinTarihi">
            <el-date-picker
              v-model="bookForm.yayinTarihi"
              type="date"
              placeholder="Yayın tarihini seçin"
              format="DD/MM/YYYY"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Toplam Stok" prop="toplamStok">
            <el-input-number
              v-model="bookForm.toplamStok"
              :min="1"
              :max="999"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Konum" prop="konum">
            <el-input
              v-model="bookForm.konum"
              placeholder="Ör: Ana Salon, 1. Kat"
              maxlength="100"
            />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="Raf No" prop="rafNo">
            <el-input
              v-model="bookForm.rafNo"
              placeholder="Ör: A-15, B-03"
              maxlength="20"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- Preview Section -->
      <el-divider>Kitap Önizlemesi</el-divider>
      <div class="book-preview">
        <div class="preview-cover">
          <el-image
            :src="previewCoverUrl"
            alt="Kitap Kapağı"
            style="width: 120px; height: 160px; border-radius: 8px;"
            fit="cover"
          >
            <template #error>
              <div class="preview-placeholder">
                <el-icon size="40"><Picture /></el-icon>
                <p>Kapak Önizlemesi</p>
              </div>
            </template>
          </el-image>
        </div>
        <div class="preview-info">
          <h3>{{ bookForm.baslik || 'Kitap Başlığı' }}</h3>
          <p><strong>Yazar:</strong> {{ selectedAuthorName || 'Yazar seçiniz' }}</p>
          <p><strong>Kategori:</strong> {{ selectedCategoryName || 'Kategori seçiniz' }}</p>
          <p><strong>ISBN:</strong> {{ bookForm.isbn || 'ISBN giriniz' }}</p>
          <p><strong>Stok:</strong> {{ bookForm.toplamStok || 0 }} adet</p>
          <p><strong>Konum:</strong> {{ bookForm.konum || 'Konum giriniz' }}-{{ bookForm.rafNo || 'Raf No' }}</p>
        </div>
      </div>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">İptal</el-button>
        <el-button 
          type="primary" 
          @click="submitForm"
          :loading="submitting"
        >
          Kitabı Ekle
        </el-button>
      </div>
    </template>

    <!-- Add Author Dialog -->
    <el-dialog
      v-model="addAuthorVisible"
      title="Yeni Yazar Ekle"
      width="400px"
      append-to-body
    >
      <el-form :model="authorForm" label-width="120px">
        <el-form-item label="Ad" required>
          <el-input v-model="authorForm.ad" placeholder="Yazar adı" />
        </el-form-item>
        <el-form-item label="Soyad" required>
          <el-input v-model="authorForm.soyad" placeholder="Yazar soyadı" />
        </el-form-item>
        <el-form-item label="Ülke" required>
          <el-input v-model="authorForm.ulke" placeholder="Ülke" />
        </el-form-item>
        <el-form-item label="Doğum Tarihi">
          <el-date-picker
            v-model="authorForm.dogumTarihi"
            type="date"
            placeholder="Doğum tarihi"
            format="DD/MM/YYYY"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="addAuthorVisible = false">İptal</el-button>
        <el-button type="primary" @click="addAuthor" :loading="addingAuthor">
          Yazar Ekle
        </el-button>
      </template>
    </el-dialog>

    <!-- Add Category Dialog -->
    <el-dialog
      v-model="addCategoryVisible"
      title="Yeni Kategori Ekle"
      width="400px"
      append-to-body
    >
      <el-form :model="categoryForm" label-width="120px">
        <el-form-item label="Kategori Adı" required>
          <el-input v-model="categoryForm.ad" placeholder="Kategori adı" />
        </el-form-item>
        <el-form-item label="Açıklama" required>
          <el-input
            v-model="categoryForm.aciklama"
            type="textarea"
            placeholder="Kategori açıklaması"
            :rows="3"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="addCategoryVisible = false">İptal</el-button>
        <el-button type="primary" @click="addCategory" :loading="addingCategory">
          Kategori Ekle
        </el-button>
      </template>
    </el-dialog>
  </el-dialog>
</template>

<script>
import { Picture } from '@element-plus/icons-vue'
import { kitapService, yazarService, kategoriService } from '@/services/api'

export default {
  name: 'AddBookDialog',
  props: {
    modelValue: {
      type: Boolean,
      default: false
    }
  },
  emits: ['update:modelValue', 'book-added'],
  data() {
    return {
      visible: this.modelValue,
      submitting: false,
      
      // Main form
      bookForm: {
        baslik: '',
        isbn: '',
        yayinTarihi: '',
        sayfaSayisi: null,
        toplamStok: 1,
        yazarId: null,
        kategoriId: null,
        konum: '',
        rafNo: ''
      },
      
      bookRules: {
        baslik: [
          { required: true, message: 'Kitap başlığı gereklidir', trigger: 'blur' }
        ],
        isbn: [
          { required: true, message: 'ISBN gereklidir', trigger: 'blur' },
          { pattern: /^[0-9-]+$/, message: 'Geçerli bir ISBN girin', trigger: 'blur' }
        ],
        yazarId: [
          { required: true, message: 'Yazar seçimi gereklidir', trigger: 'change' }
        ],
        kategoriId: [
          { required: true, message: 'Kategori seçimi gereklidir', trigger: 'change' }
        ],
        yayinTarihi: [
          { required: true, message: 'Yayın tarihi gereklidir', trigger: 'change' }
        ],
        sayfaSayisi: [
          { required: true, message: 'Sayfa sayısı gereklidir', trigger: 'blur' }
        ],
        toplamStok: [
          { required: true, message: 'Stok miktarı gereklidir', trigger: 'blur' }
        ],
        konum: [
          { required: true, message: 'Konum gereklidir', trigger: 'blur' }
        ],
        rafNo: [
          { required: true, message: 'Raf numarası gereklidir', trigger: 'blur' }
        ]
      },
      
      // Data
      authors: [],
      categories: [],
      
      // Add author dialog
      addAuthorVisible: false,
      addingAuthor: false,
      authorForm: {
        ad: '',
        soyad: '',
        ulke: '',
        dogumTarihi: ''
      },
      
      // Add category dialog
      addCategoryVisible: false,
      addingCategory: false,
      categoryForm: {
        ad: '',
        aciklama: ''
      }
    }
  },
  computed: {
    previewCoverUrl() {
      if (this.bookForm.isbn) {
        return `https://covers.openlibrary.org/b/isbn/${this.bookForm.isbn}-M.jpg`
      }
      return null
    },
    selectedAuthorName() {
      const author = this.authors.find(a => a.id === this.bookForm.yazarId)
      return author ? `${author.ad} ${author.soyad}` : ''
    },
    selectedCategoryName() {
      const category = this.categories.find(c => c.id === this.bookForm.kategoriId)
      return category ? category.ad : ''
    }
  },
  watch: {
    modelValue(val) {
      this.visible = val
      if (val) {
        this.loadData()
      }
    },
    visible(val) {
      this.$emit('update:modelValue', val)
    }
  },
  methods: {
    async loadData() {
      try {
        const [authorsResponse, categoriesResponse] = await Promise.all([
          yazarService.getAll(),
          kategoriService.getAll()
        ])
        this.authors = authorsResponse
        this.categories = categoriesResponse
      } catch (error) {
        this.$message.error('Veriler yüklenirken hata oluştu')
      }
    },

    handleAuthorChange() {
      // Author selection changed
    },

    async submitForm() {
      try {
        const valid = await this.$refs.bookFormRef.validate()
        if (!valid) return

        this.submitting = true
        
        await kitapService.create(this.bookForm)
        
        this.$message.success('Kitap başarıyla eklendi!')
        this.$emit('book-added')
        this.handleClose()
        
      } catch (error) {
        console.error('Book creation error:', error)
        this.$message.error('Kitap eklenirken hata oluştu')
      } finally {
        this.submitting = false
      }
    },

    handleClose() {
      this.visible = false
      this.resetForm()
    },

    resetForm() {
      this.bookForm = {
        baslik: '',
        isbn: '',
        yayinTarihi: '',
        sayfaSayisi: null,
        toplamStok: 1,
        yazarId: null,
        kategoriId: null,
        konum: '',
        rafNo: ''
      }
      this.$refs.bookFormRef?.resetFields()
    },

    showAddAuthorDialog() {
      this.addAuthorVisible = true
      this.authorForm = {
        ad: '',
        soyad: '',
        ulke: '',
        dogumTarihi: ''
      }
    },

    async addAuthor() {
      if (!this.authorForm.ad || !this.authorForm.soyad || !this.authorForm.ulke) {
        this.$message.error('Lütfen gerekli alanları doldurun')
        return
      }

      this.addingAuthor = true
      try {
        const newAuthor = await yazarService.create(this.authorForm)
        this.authors.push(newAuthor)
        this.bookForm.yazarId = newAuthor.id
        this.addAuthorVisible = false
        this.$message.success('Yazar başarıyla eklendi!')
      } catch (error) {
        this.$message.error('Yazar eklenirken hata oluştu')
      } finally {
        this.addingAuthor = false
      }
    },

    showAddCategoryDialog() {
      this.addCategoryVisible = true
      this.categoryForm = {
        ad: '',
        aciklama: ''
      }
    },

    async addCategory() {
      if (!this.categoryForm.ad || !this.categoryForm.aciklama) {
        this.$message.error('Lütfen gerekli alanları doldurun')
        return
      }

      this.addingCategory = true
      try {
        const newCategory = await kategoriService.create(this.categoryForm)
        this.categories.push(newCategory)
        this.bookForm.kategoriId = newCategory.id
        this.addCategoryVisible = false
        this.$message.success('Kategori başarıyla eklendi!')
      } catch (error) {
        this.$message.error('Kategori eklenirken hata oluştu')
      } finally {
        this.addingCategory = false
      }
    }
  },
  setup() {
    return {
      Picture
    }
  }
}
</script>

<style scoped>
.form-helper {
  margin-top: 4px;
}

.book-preview {
  display: flex;
  gap: 20px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #e9ecef;
}

.preview-cover {
  flex-shrink: 0;
}

.preview-placeholder {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 120px;
  height: 160px;
  background-color: #f5f7fa;
  color: #909399;
  border-radius: 8px;
  border: 1px dashed #d9d9d9;
}

.preview-placeholder p {
  margin: 8px 0 0 0;
  font-size: 12px;
}

.preview-info {
  flex: 1;
}

.preview-info h3 {
  margin: 0 0 16px 0;
  color: #1f2937;
  font-size: 18px;
  font-weight: 600;
}

.preview-info p {
  margin: 8px 0;
  color: #6b7280;
  font-size: 14px;
}

.dialog-footer {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

@media (max-width: 768px) {
  .book-preview {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }
  
  .preview-info {
    margin-top: 16px;
  }
}
</style>