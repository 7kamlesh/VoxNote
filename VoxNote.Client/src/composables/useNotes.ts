import { ref } from 'vue'
import type { NoteResponse } from '@/services/api'
import { getNotes, createNote, updateNote, deleteNote } from '@/services/api'

export function useNotes() {
  const notes = ref<NoteResponse[]>([])
  const loading = ref(false)
  const error = ref<string>('')

  async function fetchNotes() {
    loading.value = true
    error.value = ''
    try {
      notes.value = await getNotes()
    } catch (err: any) {
      error.value = err.response?.data?.error ?? 'Failed to load notes.'
    } finally {
      loading.value = false
    }
  }

  async function saveNote(title: string, content: string): Promise<NoteResponse | null> {
    error.value = ''
    try {
      const note = await createNote(title, content)
      notes.value.unshift(note)
      return note
    } catch (err: any) {
      error.value = err.response?.data?.error ?? 'Failed to save note.'
      return null
    }
  }

  async function editNote(id: number, title: string, content: string): Promise<NoteResponse | null> {
    error.value = ''
    try {
      const updated = await updateNote(id, title, content)
      const idx = notes.value.findIndex(n => n.id === id)
      if (idx !== -1) notes.value[idx] = updated
      return updated
    } catch (err: any) {
      error.value = err.response?.data?.error ?? 'Failed to update note.'
      return null
    }
  }

  async function removeNote(id: number): Promise<boolean> {
    error.value = ''
    try {
      await deleteNote(id)
      notes.value = notes.value.filter(n => n.id !== id)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.error ?? 'Failed to delete note.'
      return false
    }
  }

  return {
    notes,
    loading,
    error,
    fetchNotes,
    saveNote,
    editNote,
    removeNote
  }
}
