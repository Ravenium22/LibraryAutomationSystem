import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresGuest: true } // Giriş yapmamış kullanıcılar için
    },
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true } // Giriş yapmış kullanıcılar için
    },
    { 
      path: '/kitaplar',
      name: 'kitaplar',
      component: () => import('../views/KitapView.vue'),
      meta: { requiresAuth: true } // Giriş yapmış kullanıcılar için
    },

  ]
})

// Route guard - sayfa geçişlerini kontrol et
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isLoggedIn) {
    // Giriş yapmamış kullanıcıyı login'e yönlendir
    next('/login')
  } else if (to.meta.requiresGuest && authStore.isLoggedIn) {
    // Giriş yapmış kullanıcıyı dashboard'a yönlendir
    next('/')
  } else {
    next()
  }
})

export default router