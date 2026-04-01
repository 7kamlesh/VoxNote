<template>
  <div class="home-view">
    <h1 class="page-title">Record a Voice Note</h1>

    <!-- Step 1: Record (transcription happens live in browser) -->
    <AudioRecorder
      v-if="step === 'record'"
      @done="handleTranscriptionDone"
    />

    <!-- Step 2: Edit & Save -->
    <NoteEditor
      v-if="step === 'edit'"
      :initial-content="transcribedText"
      @save="handleSave"
      @cancel="step = 'record'"
    />

    <!-- Step 3: Saved confirmation -->
    <div v-if="step === 'saved'" class="status-section">
      <p class="success-text">? Note saved successfully!</p>
      <div class="saved-actions">
        <button class="btn btn-primary" @click="step = 'record'">Record Another</button>
        <router-link to="/notes" class="btn btn-secondary">View Notes</router-link>
      </div>
    </div>

    <p v-if="saveError" class="error-text">{{ saveError }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import AudioRecorder from '@/components/AudioRecorder.vue'
import NoteEditor from '@/components/NoteEditor.vue'
import { useNotes } from '@/composables/useNotes'

type Step = 'record' | 'edit' | 'saved'

const step = ref<Step>('record')
const transcribedText = ref('')
const saveError = ref('')
const { saveNote } = useNotes()

function handleTranscriptionDone(text: string) {
  transcribedText.value = text
  step.value = 'edit'
}

async function handleSave(title: string, content: string) {
  saveError.value = ''
  const note = await saveNote(title, content)
  if (note) {
    step.value = 'saved'
  } else {
    saveError.value = 'Failed to save note. Please try again.'
  }
}
</script>

<style scoped>
.home-view {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
}

.page-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
}

.status-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 2rem 0;
}

.success-text {
  color: #16a34a;
  font-weight: 600;
  font-size: 1.125rem;
}

.saved-actions {
  display: flex;
  gap: 0.75rem;
}

.error-text {
  color: #dc2626;
  text-align: center;
}
</style>
