<template>
  <div class="admin-view">
    <h1 class="page-title">Admin Dashboard</h1>

    <div class="admin-tabs">
      <button :class="['tab', { active: tab === 'users' }]" @click="tab = 'users'">Users</button>
      <button :class="['tab', { active: tab === 'notes' }]" @click="tab = 'notes'; loadAllNotes()">All Notes</button>
    </div>

    <!-- Users Tab -->
    <div v-if="tab === 'users'">
      <p v-if="usersLoading" class="status-text">Loading users...</p>
      <p v-else-if="usersError" class="error-text">{{ usersError }}</p>

      <div v-else class="users-list">
        <div v-for="user in users" :key="user.id" class="user-card">
          <div class="user-info">
            <span class="user-name">{{ user.username }}</span>
            <span :class="['role-badge', user.role.toLowerCase()]">{{ user.role }}</span>
          </div>
          <div class="user-meta">
            <span>{{ user.noteCount }} notes</span>
            <span>Joined {{ formatDate(user.createdAt) }}</span>
          </div>
          <button
            v-if="user.noteCount > 0"
            class="btn btn-sm btn-secondary"
            @click="viewUserNotes(user)"
          >
            View Notes
          </button>
        </div>
      </div>
    </div>

    <!-- All Notes Tab -->
    <div v-if="tab === 'notes'">
      <p v-if="notesLoading" class="status-text">Loading notes...</p>
      <p v-else-if="notesError" class="error-text">{{ notesError }}</p>
      <p v-else-if="allNotes.length === 0" class="status-text">No notes found.</p>

      <div v-else class="notes-list">
        <div v-for="note in allNotes" :key="note.id" class="note-card">
          <div class="note-header">
            <h3 class="note-title">{{ note.title || 'Untitled' }}</h3>
            <span class="note-owner">by {{ note.username }}</span>
          </div>
          <p class="note-content">{{ note.content }}</p>
          <span class="note-date">{{ formatDate(note.createdAt) }}</span>
        </div>
      </div>
    </div>

    <!-- User Notes Modal -->
    <div v-if="selectedUser" class="edit-overlay" @click.self="selectedUser = null">
      <div class="edit-modal">
        <div class="modal-header">
          <h2>Notes by {{ selectedUser.username }}</h2>
          <button class="btn btn-sm btn-secondary" @click="selectedUser = null">Close</button>
        </div>

        <p v-if="userNotesLoading" class="status-text">Loading...</p>
        <p v-else-if="userNotes.length === 0" class="status-text">No notes.</p>

        <div v-else class="notes-list">
          <div v-for="note in userNotes" :key="note.id" class="note-card">
            <div class="note-header">
              <h3 class="note-title">{{ note.title || 'Untitled' }}</h3>
              <span class="note-date">{{ formatDate(note.createdAt) }}</span>
            </div>
            <p class="note-content-full">{{ note.content }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { UserSummary, NoteResponse } from '@/services/api'
import { getAdminUsers, getAdminUserNotes, getAdminAllNotes } from '@/services/api'

const tab = ref<'users' | 'notes'>('users')

const users = ref<UserSummary[]>([])
const usersLoading = ref(false)
const usersError = ref('')

const allNotes = ref<NoteResponse[]>([])
const notesLoading = ref(false)
const notesError = ref('')

const selectedUser = ref<UserSummary | null>(null)
const userNotes = ref<NoteResponse[]>([])
const userNotesLoading = ref(false)

onMounted(() => loadUsers())

async function loadUsers() {
  usersLoading.value = true
  usersError.value = ''
  try {
    users.value = await getAdminUsers()
  } catch (err: any) {
    usersError.value = err.response?.data?.error ?? 'Failed to load users.'
  } finally {
    usersLoading.value = false
  }
}

async function loadAllNotes() {
  if (allNotes.value.length > 0) return
  notesLoading.value = true
  notesError.value = ''
  try {
    allNotes.value = await getAdminAllNotes()
  } catch (err: any) {
    notesError.value = err.response?.data?.error ?? 'Failed to load notes.'
  } finally {
    notesLoading.value = false
  }
}

async function viewUserNotes(user: UserSummary) {
  selectedUser.value = user
  userNotesLoading.value = true
  userNotes.value = []
  try {
    userNotes.value = await getAdminUserNotes(user.id)
  } finally {
    userNotesLoading.value = false
  }
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}
</script>

<style scoped>
.admin-view {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.page-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
}

.admin-tabs {
  display: flex;
  background: #f3f4f6;
  border-radius: 8px;
  padding: 3px;
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

.users-list, .notes-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.user-card {
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  padding: 1rem;
  background: #fff;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.375rem;
}

.user-name {
  font-weight: 600;
  color: #111827;
}

.role-badge {
  font-size: 0.6875rem;
  font-weight: 600;
  padding: 0.125rem 0.5rem;
  border-radius: 12px;
  text-transform: uppercase;
}

.role-badge.admin {
  background: #fef3c7;
  color: #92400e;
}

.role-badge.user {
  background: #dbeafe;
  color: #1e40af;
}

.user-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.75rem;
  color: #9ca3af;
  margin-bottom: 0.5rem;
}

.note-card {
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  padding: 1rem;
  background: #fff;
}

.note-header {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  margin-bottom: 0.5rem;
}

.note-title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.note-owner {
  font-size: 0.75rem;
  color: #4f46e5;
  font-weight: 500;
  white-space: nowrap;
}

.note-content {
  font-size: 0.875rem;
  color: #4b5563;
  line-height: 1.5;
  margin: 0 0 0.5rem;
  white-space: pre-wrap;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.note-content-full {
  font-size: 0.875rem;
  color: #4b5563;
  line-height: 1.5;
  margin: 0;
  white-space: pre-wrap;
}

.note-date {
  font-size: 0.75rem;
  color: #9ca3af;
}

.edit-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
  padding: 1rem;
}

.edit-modal {
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
}

.status-text {
  text-align: center;
  color: #6b7280;
  padding: 2rem 0;
}

.error-text {
  text-align: center;
  color: #dc2626;
  padding: 2rem 0;
}
</style>
