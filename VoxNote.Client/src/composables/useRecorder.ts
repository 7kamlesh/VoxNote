import { ref, onUnmounted } from 'vue'

export type RecordingState = 'idle' | 'recording' | 'stopped' | 'error'

export function useRecorder() {
  const state = ref<RecordingState>('idle')
  const transcribedText = ref<string>('')
  const errorMessage = ref<string>('')
  const duration = ref(0)
  const interimText = ref<string>('')

  let recognition: any = null
  let timerInterval: ReturnType<typeof setInterval> | null = null
  let finalTranscript = ''

  function startTimer() {
    duration.value = 0
    timerInterval = setInterval(() => {
      duration.value++
    }, 1000)
  }

  function stopTimer() {
    if (timerInterval) {
      clearInterval(timerInterval)
      timerInterval = null
    }
  }

  function getSpeechRecognition(): any {
    const w = window as any
    const SpeechRecognition = w.SpeechRecognition || w.webkitSpeechRecognition
    if (!SpeechRecognition) {
      return null
    }
    return new SpeechRecognition()
  }

  async function startRecording() {
    try {
      errorMessage.value = ''
      transcribedText.value = ''
      interimText.value = ''
      finalTranscript = ''

      // Request mic permission first (so we get proper error if denied)
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true })
      stream.getTracks().forEach(track => track.stop())

      recognition = getSpeechRecognition()
      if (!recognition) {
        errorMessage.value = 'Speech recognition is not supported in this browser. Please use Chrome or Edge.'
        state.value = 'error'
        return
      }

      recognition.continuous = true
      recognition.interimResults = true
      recognition.lang = 'en-US'

      recognition.onresult = (event: any) => {
        let interim = ''
        for (let i = event.resultIndex; i < event.results.length; i++) {
          const transcript = event.results[i][0].transcript
          if (event.results[i].isFinal) {
            finalTranscript += transcript + ' '
          } else {
            interim += transcript
          }
        }
        transcribedText.value = finalTranscript.trim()
        interimText.value = interim
      }

      recognition.onerror = (event: any) => {
        if (event.error === 'not-allowed') {
          errorMessage.value = 'Microphone permission was denied. Please allow access and try again.'
        } else if (event.error === 'no-speech') {
          // Ignore no-speech errors, recognition will continue
          return
        } else if (event.error === 'network') {
          errorMessage.value = 'Network error during speech recognition. Please check your connection.'
        } else {
          errorMessage.value = `Speech recognition error: ${event.error}`
        }
        stopTimer()
        state.value = 'error'
      }

      recognition.onend = () => {
        // Only update state if we didn't manually stop
        if (state.value === 'recording') {
          // Recognition auto-stopped (e.g. silence), restart it
          try {
            recognition.start()
          } catch {
            stopTimer()
            transcribedText.value = finalTranscript.trim()
            state.value = 'stopped'
          }
        }
      }

      recognition.start()
      startTimer()
      state.value = 'recording'
    } catch (err: any) {
      stopTimer()
      state.value = 'error'

      if (err.name === 'NotAllowedError') {
        errorMessage.value = 'Microphone permission was denied. Please allow access and try again.'
      } else if (err.name === 'NotFoundError') {
        errorMessage.value = 'No microphone found on this device.'
      } else {
        errorMessage.value = `Could not access microphone: ${err.message}`
      }
    }
  }

  function stopRecording() {
    if (recognition) {
      state.value = 'stopped'
      try {
        recognition.stop()
      } catch {
        // already stopped
      }
      transcribedText.value = finalTranscript.trim()
      interimText.value = ''
    }
  }

  function reset() {
    stopRecording()
    stopTimer()
    transcribedText.value = ''
    interimText.value = ''
    errorMessage.value = ''
    finalTranscript = ''
    duration.value = 0
    state.value = 'idle'
  }

  onUnmounted(() => {
    if (recognition) {
      try { recognition.stop() } catch { /* ignore */ }
    }
    stopTimer()
  })

  return {
    state,
    transcribedText,
    interimText,
    errorMessage,
    duration,
    startRecording,
    stopRecording,
    reset
  }
}
