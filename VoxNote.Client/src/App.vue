<template>
  <div id="app-root">
    <header v-if="isLoggedIn" class="app-header">
      <router-link to="/" class="logo">
        <MicIcon :size="20" :stroke-width="2.5" />
        VoxNote
      </router-link>
      <nav>
        <router-link v-if="!isAdmin" to="/">Record</router-link>
        <router-link v-if="!isAdmin" to="/notes">My Notes</router-link>
        <router-link v-if="isAdmin" to="/admin">Admin</router-link>
        <button class="nav-btn" @click="handleLogout">Logout</button>
      </nav>
    </header>
    <main :class="{ 'app-main': isLoggedIn }">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import MicIcon from '@/components/MicIcon.vue'

const router = useRouter()
const { isLoggedIn, isAdmin, logout } = useAuth()

function handleLogout() {
  logout()
  router.push('/login')
}
</script>

<style scoped>
.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  background: #4f46e5;
  color: #fff;
}

.app-header .logo {
  font-size: 1.25rem;
  font-weight: 700;
  color: #fff;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
}

.app-header nav {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.app-header nav a {
  color: rgba(255, 255, 255, 0.85);
  text-decoration: none;
  font-weight: 500;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  transition: background 0.2s;
}

.app-header nav a:hover,
.app-header nav a.router-link-exact-active {
  background: rgba(255, 255, 255, 0.15);
  color: #fff;
}

.nav-btn {
  background: rgba(255, 255, 255, 0.15);
  color: #fff;
  border: none;
  padding: 0.25rem 0.75rem;
  border-radius: 6px;
  font-weight: 500;
  font-size: 0.9375rem;
  cursor: pointer;
  transition: background 0.2s;
}

.nav-btn:hover {
  background: rgba(255, 255, 255, 0.25);
}

.app-main {
  max-width: 600px;
  margin: 0 auto;
  padding: 1.5rem 1rem;
}
</style>
