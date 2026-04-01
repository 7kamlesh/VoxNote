import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  timeout: 60000
})

// Attach JWT token to every request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Redirect to login on 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// --- Auth types ---
export interface AuthResponse {
  token: string
  username: string
  role: string
}

export interface UserSummary {
  id: number
  username: string
  role: string
  noteCount: number
  createdAt: string
}

// --- Note types ---
export interface NoteResponse {
  id: number
  title: string
  content: string
  createdAt: string
  updatedAt: string | null
  userId: number
  username: string
}

export interface TranscriptionResponse {
  success: boolean
  text: string
  error?: string
}

// --- Auth API ---
export async function login(username: string, password: string): Promise<AuthResponse> {
  const { data } = await api.post<AuthResponse>('/auth/login', { username, password })
  return data
}

export async function register(username: string, password: string): Promise<AuthResponse> {
  const { data } = await api.post<AuthResponse>('/auth/register', { username, password })
  return data
}

// --- Transcription API ---
export async function transcribeAudio(audioBlob: Blob): Promise<TranscriptionResponse> {
  const formData = new FormData()
  formData.append('audio', audioBlob, 'recording.webm')
  const { data } = await api.post<TranscriptionResponse>('/transcription', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
  return data
}

// --- Notes API (current user) ---
export async function getNotes(): Promise<NoteResponse[]> {
  const { data } = await api.get<NoteResponse[]>('/notes')
  return data
}

export async function getNote(id: number): Promise<NoteResponse> {
  const { data } = await api.get<NoteResponse>(`/notes/${id}`)
  return data
}

export async function createNote(title: string, content: string): Promise<NoteResponse> {
  const { data } = await api.post<NoteResponse>('/notes', { title, content })
  return data
}

export async function updateNote(id: number, title: string, content: string): Promise<NoteResponse> {
  const { data } = await api.put<NoteResponse>(`/notes/${id}`, { title, content })
  return data
}

export async function deleteNote(id: number): Promise<void> {
  await api.delete(`/notes/${id}`)
}

// --- Admin API ---
export async function getAdminUsers(): Promise<UserSummary[]> {
  const { data } = await api.get<UserSummary[]>('/admin/users')
  return data
}

export async function getAdminUserNotes(userId: number): Promise<NoteResponse[]> {
  const { data } = await api.get<NoteResponse[]>(`/admin/users/${userId}/notes`)
  return data
}

export async function getAdminAllNotes(): Promise<NoteResponse[]> {
  const { data } = await api.get<NoteResponse[]>('/admin/notes')
  return data
}
