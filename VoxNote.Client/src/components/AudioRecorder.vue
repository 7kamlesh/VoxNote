<template>
  <div class="recorder">
    <div class="recorder-visual" :class="{ recording: state === 'recording' }">
      <MicIcon :size="40" :stroke-width="1.8" class="mic-icon" />
      <span v-if="state === 'recording'" class="pulse"></span>
    </div>

    <p class="timer" v-if="state === 'recording' || state === 'stopped'">
      {{ formattedDuration }}
    </p>

    <!-- Live transcription preview while recording -->
    <div v-if="state === 'recording' && (transcribedText || interimText)" class="live-text">
      <span>{{ transcribedText }}</span>
      <span class="interim">{{ interimText }}</span>
    </div>

    <div class="recorder-controls">
      <button
        v-if="state === 'idle' || state === 'error'"
        class="btn btn-record"
        @click="startRecording"
      >
        Start Recording
      </button>

      <button
        v-if="state === 'recording'"
        class="btn btn-stop"
        @click="handleStop"
      >
        Stop
      </button>

      <template v-if="state === 'stopped'">
        <button class="btn btn-primary" @click="$emit('done', transcribedText)" :disabled="!transcribedText.trim()">
          Edit &amp; Save
        </button>
        <button class="btn btn-secondary" @click="reset">
          Re-record
        </button>
      </template>
    </div>

    <p v-if="state === 'stopped' && !transcribedText.trim()" class="hint-text">
      No speech was detected. Try recording again.
    </p>

    <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRecorder } from '@/composables/useRecorder'
import MicIcon from '@/components/MicIcon.vue'

const emit = defineEmits<{
  done: [text: string]
}>()

const { state, transcribedText, interimText, errorMessage, duration, startRecording, stopRecording, reset } = useRecorder()

const formattedDuration = computed(() => {
  const mins = Math.floor(duration.value / 60)
  const secs = duration.value % 60
  return `${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')}`
})

function handleStop() {
  stopRecording()
}
</script>

<style scoped>
.recorder {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.recorder-visual {
  position: relative;
  width: 100px;
  height: 100px;
  border-radius: 50%;
  background: #eef2ff;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.3s;
}

.recorder-visual.recording {
  background: #fef2f2;
}

.mic-icon {
  color: #4f46e5;
}

.recorder-visual.recording .mic-icon {
  color: #ef4444;
}

.pulse {
  position: absolute;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  border: 3px solid #ef4444;
  animation: pulse-ring 1.5s ease-out infinite;
}

@keyframes pulse-ring {
  0% { transform: scale(1); opacity: 1; }
  100% { transform: scale(1.4); opacity: 0; }
}

.timer {
  font-size: 1.5rem;
  font-weight: 600;
  font-variant-numeric: tabular-nums;
  color: #374151;
}

.live-text {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  font-size: 0.9375rem;
  color: #374151;
  line-height: 1.5;
  width: 100%;
  max-width: 400px;
  min-height: 60px;
  text-align: center;
}

.live-text .interim {
  color: #9ca3af;
}

.recorder-controls {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
  justify-content: center;
}

.hint-text {
  color: #6b7280;
  font-size: 0.875rem;
  text-align: center;
}

.error-text {
  color: #dc2626;
  font-size: 0.875rem;
  text-align: center;
  max-width: 300px;
}
</style>
