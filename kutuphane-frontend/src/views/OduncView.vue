<template>
  <div class="odunc-page">
    <h1>{{ authStore.user?.email }}'ın Ödünçleri</h1>
    
    <el-card class="stats-container" style="margin-bottom: 20px;">
      <div class="stats-grid">
        <div class="stat-item">
          <el-icon class="stat-icon" color="#409EFF" size="24"><Reading /></el-icon>
          <div class="stat-content">
            <div class="stat-number">{{ oduncKitaplar.length }}</div>
            <div class="stat-label">Toplam Ödünç</div>
          </div>
        </div>
        <div class="stat-item">
          <el-icon class="stat-icon" color="#67C23A" size="24"><SwitchButton /></el-icon>
          <div class="stat-content">
            <div class="stat-number">{{ activeBooks }}</div>
            <div class="stat-label">Aktif Ödünç</div>
          </div>
        </div>
        <div class="stat-item" v-if="overdueBooks > 0">
          <el-icon class="stat-icon" color="#F56C6C" size="24"><RefreshRight /></el-icon>
          <div class="stat-content">
            <div class="stat-number">{{ overdueBooks }}</div>
            <div class="stat-label">Geciken Kitap</div>
          </div>
        </div>
        <div class="stat-item" v-if="totalFine > 0">
          <el-icon class="stat-icon" color="#E6A23C" size="24"><Collection /></el-icon>
          <div class="stat-content">
            <div class="stat-number">{{ totalFine }}₺</div>
            <div class="stat-label">Toplam Ceza</div>
          </div>
        </div>
      </div>
    </el-card>

    <el-table :data="oduncKitaplar" style="width: 100%" v-loading="loading">
      <el-table-column prop="kitapAdi" label="Kitap Adı" />
      <el-table-column prop="yazarAdi" label="Yazar" />
      <el-table-column prop="verilenTarih" label="Verilme Tarihi">
        <template #default="scope">
          {{ formatDate(scope.row.verilenTarih) }}
        </template>
      </el-table-column>
      <el-table-column prop="geriVerilmesiGerekenTarih" label="Son Teslim">
        <template #default="scope">
          {{ formatDate(scope.row.geriVerilmesiGerekenTarih) }}
        </template>
      </el-table-column>
      <el-table-column prop="iadeTarihi" label="İade Tarihi">
        <template #default="scope">
          {{ scope.row.iadeTarihi ? formatDate(scope.row.iadeTarihi) : '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="durum" label="Durum">
        <template #default="scope">
          <el-tag 
            :type="scope.row.durum === 'İade Edildi' ? 'success' : 
                   scope.row.durum === 'Gecikmiş' ? 'danger' : 'warning'">
            {{ scope.row.durum }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="Gecikme/Ceza" v-if="hasOverdueBooks">
        <template #default="scope">
          <span v-if="scope.row.gecikmeGunSayisi > 0" class="overdue-info">
            {{ scope.row.gecikmeGunSayisi }} gün - {{ scope.row.gecikmeCezasi }}₺
          </span>
          <span v-else>-</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/auth'
import { oduncService } from '@/services/api'
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
  data() {
    return {
      oduncKitaplar: [],
      loading: false
    };
  },
  computed: {
    activeBooks() {
      return this.oduncKitaplar.filter(book => !book.iadeEdildiMi).length;
    },
    overdueBooks() {
      return this.oduncKitaplar.filter(book => book.durum === 'Gecikmiş').length;
    },
    totalFine() {
      return this.oduncKitaplar
        .filter(book => book.gecikmeGunSayisi > 0)
        .reduce((total, book) => total + book.gecikmeCezasi, 0);
    },
    hasOverdueBooks() {
      return this.oduncKitaplar.some(book => book.gecikmeGunSayisi > 0);
    }
  },
  watch: {
    'authStore.user.id': {
      handler(newId) {
        if (newId) {
          this.fetchOduncKitaplar();
        }
      },
      immediate: false
    }
  },
  created() {
    // Try to fetch data immediately, but watcher will handle if user ID isn't ready yet
    if (this.authStore.user?.id) {
      this.fetchOduncKitaplar();
    }
  },
  methods: {
    async fetchOduncKitaplar() {
      try {
        this.loading = true;
        
        // Ensure user is authenticated and has an ID
        if (!this.authStore.user) {
          console.error('User not authenticated');
          this.$message.error('Kullanıcı kimlik doğrulaması yapılmamış');
          return;
        }
        
        if (!this.authStore.user.id) {
          console.error('User ID not available');
          this.$message.error('Kullanıcı ID bilgisi alınamadı');
          return;
        }
        
        const response = await oduncService.getByKullanici(this.authStore.user.id);
        
        // Map the backend response to frontend format
        this.oduncKitaplar = response.map(odunc => ({
          id: odunc.id,
          kitapAdi: odunc.kitapBaslik,
          yazarAdi: odunc.yazarAdSoyad,
          verilenTarih: odunc.oduncAlinmaTarihi,
          iadeTarihi: odunc.geriVerilisTarihi,
          durum: odunc.durumu,
          iadeEdildiMi: odunc.iadeEdildiMi,
          gecikmeGunSayisi: odunc.gecikmeGunSayisi,
          gecikmeCezasi: odunc.gecikmeCezasi,
          geriVerilmesiGerekenTarih: odunc.geriVerilmesiGerekenTarih
        }));
        
      } catch (error) {
        console.error('Ödünç kitaplar getirilirken hata:', error);
        this.$message.error('Ödünç kitaplar yüklenirken hata oluştu');
      } finally {
        this.loading = false;
      }
    },
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString('tr-TR');
    }
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
  }
}
</script>

<style scoped>
.odunc-page {
  padding: 20px;
}

.stats-container {
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

.overdue-info {
  color: #F56C6C;
  font-weight: 500;
}
</style>