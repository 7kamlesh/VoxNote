<template>
  <div class="notes-view">
    <h1 class="page-title">My Notes</h1>

    <!-- Edit modal -->
    <div v-if="editingNote" class="edit-overlay">
      <div class="edit-modal">
        <h2>Edit Note</h2>
        <NoteEditor
          :initial-title="editingNote.title"
          :initial-content="editingNote.content"
          @save="handleUpdate"
          @cancel="editingNote = null"
        />
      </div>
    </div>

    <NoteList
      :notes="notes"
      :loading="loading"
      :error="error"
      @edit="startEdit"
      @delete="handleDelete"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import NoteList from '@/components/NoteList.vue'
import NoteEditor from '@/components/NoteEditor.vue'
import { useNotes } from '@/composables/useNotes'
import type { NoteResponse } from '@/services/api'

const { notes, loading, error, fetchNotes, editNote, removeNote } = useNotes()
const editingNote = ref<NoteResponse | null>(null)

onMounted(() => fetchNotes())

function startEdit(note: NoteResponse) {
  editingNote.value = { ...note }
}

async function handleUpdate(title: string, content: string) {
  if (!editingNote.value) return
  const updated = await editNote(editingNote.value.id, title, content)
  if (updated) {
    editingNote.value = null
  }
}

async function handleDelete(id: number) {
  if (confirm('Delete this note?')) {
    await removeNote(id)
  }
}
</script>

<style scoped>
.notes-view {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.page-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
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

.edit-modal h2 {
  margin: 0 0 1rem;
  font-size: 1.125rem;
  font-weight: 600;
}
</style>
