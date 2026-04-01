import { ref, computed } from 'vue'
import type { AuthResponse } from '@/services/api'
import { login as apiLogin, register as apiRegister } from '@/services/api'

interface StoredUser {
  username: string
  role: string
}

function loadUser(): StoredUser | null {
  const raw = localStorage.getItem('user')
  if (!raw) return null
  try {
    return JSON.parse(raw) as StoredUser
  } catch {
    return null
  }
}

const currentUser = ref<StoredUser | null>(loadUser())

export function useAuth() {
  const error = ref<string>('')
  const loading = ref(false)

  const isLoggedIn = computed(() => currentUser.value !== null)
  const isAdmin = computed(() => currentUser.value?.role === 'Admin')
  const username = computed(() => currentUser.value?.username ?? '')
  const role = computed(() => currentUser.value?.role ?? '')

  function saveAuth(auth: AuthResponse) {
    localStorage.setItem('token', auth.token)
    localStorage.setItem('user', JSON.stringify({ username: auth.username, role: auth.role }))
    currentUser.value = { username: auth.username, role: auth.role }
  }

  async function login(usernameInput: string, password: string): Promise<boolean> {
    error.value = ''
    loading.value = true
    try {
      const auth = await apiLogin(usernameInput, password)
      saveAuth(auth)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.error ?? 'Login failed.'
      return false
    } finally {
      loading.value = false
    }
  }

  async function register(usernameInput: string, password: string): Promise<boolean> {
    error.value = ''
    loading.value = true
    try {
      const auth = await apiRegister(usernameInput, password)
      saveAuth(auth)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.error ?? 'Registration failed.'
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    currentUser.value = null
  }

  return {
    isLoggedIn,
    isAdmin,
    username,
    role,
    error,
    loading,
    login,
    register,
    logout
  }
}
