<template>
  <div class="note-editor">
    <input
      v-model="title"
      class="input-title"
      type="text"
      placeholder="Note title (optional)"
      maxlength="200"
    />
    <textarea
      v-model="content"
      class="input-content"
      rows="6"
      placeholder="Transcribed text will appear here..."
    ></textarea>
    <div class="editor-actions">
      <button class="btn btn-primary" @click="handleSave" :disabled="!content.trim() || saving">
        {{ saving ? 'Saving...' : 'Save Note' }}
      </button>
      <button class="btn btn-secondary" @click="$emit('cancel')">
        Cancel
      </button>
    </div>
    <p v-if="errorMsg" class="error-text">{{ errorMsg }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  initialContent?: string
  initialTitle?: string
}>()

const emit = defineEmits<{
  save: [title: string, content: string]
  cancel: []
}>()

const title = ref(props.initialTitle ?? '')
const content = ref(props.initialContent ?? '')
const saving = ref(false)
const errorMsg = ref('')

watch(() => props.initialContent, (val) => {
  if (val !== undefined) content.value = val
})

function handleSave() {
  if (!content.value.trim()) {
    errorMsg.value = 'Content cannot be empty.'
    return
  }
  errorMsg.value = ''
  emit('save', title.value.trim(), content.value.trim())
}

defineExpose({ saving })
</script>

<style scoped>
.note-editor {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.input-title {
  padding: 0.625rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
  outline: none;
  transition: border-color 0.2s;
}

.input-title:focus {
  border-color: #4f46e5;
}

.input-content {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.9375rem;
  resize: vertical;
  min-height: 120px;
  font-family: inherit;
  outline: none;
  transition: border-color 0.2s;
}

.input-content:focus {
  border-color: #4f46e5;
}

.editor-actions {
  display: flex;
  gap: 0.75rem;
}

.error-text {
  color: #dc2626;
  font-size: 0.875rem;
}
</style>
