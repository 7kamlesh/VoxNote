<template>
  <div class="login-view">
    <div class="login-card">
      <div class="login-icon">
        <MicIcon :size="48" :stroke-width="1.5" />
      </div>
      <h1>VoxNote</h1>
      <p class="subtitle">Voice notes, transcribed</p>

      <div class="tab-switcher">
        <button
          :class="['tab', { active: mode === 'login' }]"
          @click="mode = 'login'; error = ''"
        >
          Login
        </button>
        <button
          :class="['tab', { active: mode === 'register' }]"
          @click="mode = 'register'; error = ''"
        >
          Register
        </button>
      </div>

      <form @submit.prevent="handleSubmit" class="login-form">
        <input
          v-model="username"
          type="text"
          placeholder="Username"
          autocomplete="username"
          class="input"
          required
        />
        <input
          v-model="password"
          type="password"
          placeholder="Password"
          autocomplete="current-password"
          class="input"
          required
        />
        <button type="submit" class="btn btn-primary btn-full" :disabled="loading">
          {{ loading ? 'Please wait...' : (mode === 'login' ? 'Sign In' : 'Create Account') }}
        </button>
      </form>

      <p v-if="error" class="error-text">{{ error }}</p>

      <p v-if="mode === 'login'" class="hint">
        Demo admin: <strong>admin</strong> / <strong>admin123</strong>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import MicIcon from '@/components/MicIcon.vue'

const router = useRouter()
const { login, register, isAdmin, error, loading } = useAuth()

const mode = ref<'login' | 'register'>('login')
const username = ref('')
const password = ref('')

async function handleSubmit() {
  const success = mode.value === 'login'
    ? await login(username.value, password.value)
    : await register(username.value, password.value)

  if (success) {
    router.push(isAdmin.value ? '/admin' : '/')
  }
}
</script>

<style scoped>
.login-view {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100dvh;
  padding: 1rem;
  background: #f9fafb;
}

.login-card {
  background: #fff;
  border-radius: 16px;
  padding: 2rem 1.5rem;
  width: 100%;
  max-width: 380px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.08);
  text-align: center;
}

.login-icon {
  color: #4f46e5;
  margin-bottom: 0.5rem;
  display: flex;
  justify-content: center;
}

.login-card h1 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
  margin: 0;
}

.subtitle {
  color: #6b7280;
  font-size: 0.875rem;
  margin: 0.25rem 0 1.5rem;
}

.tab-switcher {
  display: flex;
  background: #f3f4f6;
  border-radius: 8px;
  padding: 3px;
  margin-bottom: 1.25rem;
}

.tab {
  flex: 1;
  padding: 0.5rem;
  border: none;
  background: transparent;
  font-weight: 600;
  font-size: 0.875rem;
  color: #6b7280;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.tab.active {
  background: #fff;
  color: #4f46e5;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.input {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.9375rem;
  outline: none;
  transition: border-color 0.2s;
}

.input:focus {
  border-color: #4f46e5;
}

.btn-full {
  width: 100%;
  margin-top: 0.25rem;
}

.error-text {
  color: #dc2626;
  font-size: 0.875rem;
  margin-top: 0.75rem;
}

.hint {
  color: #9ca3af;
  font-size: 0.75rem;
  margin-top: 1rem;
}

.hint strong {
  color: #6b7280;
}
</style>
