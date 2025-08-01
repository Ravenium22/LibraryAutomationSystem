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
      meta: { requiresGuest: true }
    },
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true }
    },
    { 
      path: '/kitaplar',
      name: 'kitaplar',
      component: () => import('../views/KitapView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/kategoriler',
      name: 'kategoriler',
      component: () => import('../views/KategoriView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/odunc',
      name: 'odunc',
      component: () => import('../views/OduncView.vue'),
      meta: { requiresAuth: true }
    
    },
    {
      path: '/yazarlar',
      name: 'yazarlar',
      component: () => import('../views/YazarlarView.vue'),
      meta: { requiresAuth: true }
    },

    {
      path: '/admin',
      name: 'admin',
      component: () => import('../views/AdminView.vue'),
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    {
    path: '/admin/overdue-books',
    name: 'admin-overdue-books',
    component: () => import('@/views/OverdueView.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
{
  path: '/admin/all-loans',
  name: 'admin-all-loans', 
  component: () => import('@/views/AllLoansView.vue'),
  meta: { requiresAuth: true, requiresAdmin: true }
}
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isLoggedIn) {
    next('/login')
  } else if (to.meta.requiresGuest && authStore.isLoggedIn) {
    next('/')
  } else {
    next()
  }
})

export default router