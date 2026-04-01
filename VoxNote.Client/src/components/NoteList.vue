<template>
  <div class="note-list">
    <p v-if="loading" class="status-text">Loading notes...</p>
    <p v-else-if="error" class="error-text">{{ error }}</p>
    <p v-else-if="notes.length === 0" class="status-text">No saved notes yet.</p>

    <div v-else class="notes">
      <div v-for="note in notes" :key="note.id" class="note-card">
        <div class="note-header">
          <h3 class="note-title">{{ note.title || 'Untitled' }}</h3>
          <span class="note-date">{{ formatDate(note.createdAt) }}</span>
        </div>
        <p class="note-content">{{ note.content }}</p>
        <div class="note-actions">
          <button class="btn btn-sm btn-secondary" @click="$emit('edit', note)">Edit</button>
          <button class="btn btn-sm btn-danger" @click="$emit('delete', note.id)">Delete</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { NoteResponse } from '@/services/api'

defineProps<{
  notes: NoteResponse[]
  loading: boolean
  error: string
}>()

defineEmits<{
  edit: [note: NoteResponse]
  delete: [id: number]
}>()

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.note-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.notes {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.note-card {
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  padding: 1rem;
  background: #fff;
  transition: box-shadow 0.2s;
}

.note-card:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
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

.note-date {
  font-size: 0.75rem;
  color: #9ca3af;
  white-space: nowrap;
}

.note-content {
  font-size: 0.875rem;
  color: #4b5563;
  line-height: 1.5;
  margin: 0 0 0.75rem;
  white-space: pre-wrap;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.note-actions {
  display: flex;
  gap: 0.5rem;
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
