<template>
  <div id="app" class="dark">
    <router-view v-if="!authStore.isLoggedIn" />
    
    <AppLayout v-else />
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/auth'
import AppLayout from '@/components/AppLayout.vue'

export default {
  name: 'App',
  components: {
    AppLayout
  },
  setup() {
    const authStore = useAuthStore()
    authStore.checkAuth()
    return { authStore }
  }
}
</script>

<style>
html.dark {
  color-scheme: dark;
}
</style>