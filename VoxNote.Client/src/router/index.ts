import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/views/LoginView.vue'
import HomeView from '@/views/HomeView.vue'
import NotesView from '@/views/NotesView.vue'
import AdminView from '@/views/AdminView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', name: 'login', component: LoginView, meta: { guest: true } },
    { path: '/', name: 'home', component: HomeView, meta: { requiresAuth: true } },
    { path: '/notes', name: 'notes', component: NotesView, meta: { requiresAuth: true } },
    { path: '/admin', name: 'admin', component: AdminView, meta: { requiresAuth: true, requiresAdmin: true } }
  ]
})

router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  const user = localStorage.getItem('user')

  if (to.meta.requiresAuth && !token) {
    return { name: 'login' }
  }

  if (to.meta.requiresAdmin) {
    try {
      const parsed = user ? JSON.parse(user) : null
      if (parsed?.role !== 'Admin') {
        return { name: 'home' }
      }
    } catch {
      return { name: 'login' }
    }
  }

  if (to.meta.guest && token) {
    return { name: 'home' }
  }
})

export default router
