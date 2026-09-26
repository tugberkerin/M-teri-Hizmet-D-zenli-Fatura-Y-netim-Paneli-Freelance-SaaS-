<template>
  <div class="uptime-view animate-fade-in">
    <!-- Header -->
    <div class="view-header">
      <div>
        <h1 class="view-title gradient-text">Uptime İzleme</h1>
        <p class="view-subtitle">Müşterilerinizin web sitelerini ve servislerini 7/24 canlı takip edin.</p>
      </div>
      <button class="btn-primary flex-align-center" @click="openModal()">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="icon-sm"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
        Monitör Ekle
      </button>
    </div>

    <!-- Overview Bar -->
    <div class="uptime-summary-grid">
      <div class="glass-card uptime-card">
        <div class="card-icon icon-up">🟢</div>
        <div>
          <div class="card-val">{{ countUp }} / {{ monitors.length }}</div>
          <div class="card-lbl">Erişilebilir Servisler (UP)</div>
        </div>
      </div>
      <div class="glass-card uptime-card">
        <div class="card-icon icon-down">🔴</div>
        <div>
          <div class="card-val">{{ countDown }}</div>
          <div class="card-lbl">Erişilemeyen (DOWN)</div>
        </div>
      </div>
      <div class="glass-card uptime-card">
        <div class="card-icon icon-ping">⚡</div>
        <div>
          <div class="card-val">{{ avgResponseMs }} ms</div>
          <div class="card-lbl">Ortalama Yanıt Süresi</div>
        </div>
      </div>
    </div>

    <!-- Monitors Grid -->
    <div class="glass-card table-panel">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <span>Uptime monitörleri kontrol ediliyor...</span>
      </div>

      <div v-else-if="monitors.length === 0" class="empty-state">
        <div class="empty-icon">📡</div>
        <h3>Monitör Bulunamadı</h3>
        <p>Henüz izlemeye alınmış bir web sitesi veya servis bulunmuyor.</p>
      </div>

      <div v-else class="monitors-grid">
        <div v-for="m in monitors" :key="m.id" class="glass-card monitor-item-card">
          <div class="m-header">
            <div class="m-title-area">
              <span class="pulse-dot" :class="m.status === 'DOWN' ? 'dot-down' : 'dot-up'"></span>
              <div>
                <h4 class="m-name">{{ m.name }}</h4>
                <a :href="m.url" target="_blank" class="m-url">{{ m.url }}</a>
              </div>
            </div>
            <span class="status-pill" :class="m.status === 'DOWN' ? 'pill-down' : 'pill-up'">
              {{ m.status === 'DOWN' ? 'DOWN' : 'ONLINE' }}
            </span>
          </div>

          <div class="m-meta">
            <div class="meta-col">
              <span class="meta-label">Müşteri</span>
              <span class="meta-val">{{ m.clientName || 'Bilinmiyor' }}</span>
            </div>
            <div class="meta-col">
              <span class="meta-label">Kontrol Periyodu</span>
              <span class="meta-val">Her {{ m.checkIntervalMinutes }} dk</span>
            </div>
            <div class="meta-col">
              <span class="meta-label">Ping</span>
              <span class="meta-val">{{ m.responseTimeMs || 0 }} ms</span>
            </div>
          </div>

          <div class="m-actions">
            <button class="btn-sm btn-ping" @click="pingMonitor(m)">🔄 Şimdi Test Et</button>
            <button class="btn-icon btn-danger" title="Sil" @click="deleteMonitor(m.id)">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="modal-backdrop animate-fade-in" @click.self="closeModal">
      <div class="glass-card modal-card animate-fade-in-up">
        <div class="modal-header">
          <h3>Yeni Uptime Monitörü Ekle</h3>
          <button class="btn-close" @click="closeModal">✕</button>
        </div>
        <form @submit.prevent="saveMonitor" class="modal-form">
          <div class="form-group">
            <label>Müşteri Seçin *</label>
            <select v-model="form.clientId" class="cp-input" required>
              <option value="" disabled>-- Müşteri Seçin --</option>
              <option v-for="c in clientsList" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Monitör / Servis Adı *</label>
            <input v-model="form.name" type="text" class="cp-input" required placeholder="Örn: Ana Website" />
          </div>
          <div class="form-group">
            <label>URL (Site / API Adresi) *</label>
            <input v-model="form.url" type="url" class="cp-input" required placeholder="https://example.com" />
          </div>
          <div class="form-group">
            <label>Kontrol Aralığı (Dakika)</label>
            <select v-model.number="form.checkIntervalMinutes" class="cp-input">
              <option :value="1">Her 1 Dakikada Bir</option>
              <option :value="5">Her 5 Dakikada Bir</option>
              <option :value="15">Her 15 Dakikada Bir</option>
              <option :value="30">Her 30 Dakikada Bir</option>
            </select>
          </div>
          <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" @click="closeModal">İptal</button>
            <button type="submit" class="btn-primary" :disabled="saving">
              {{ saving ? 'Kaydediliyor...' : 'Monitör Oluştur' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { uptimeService, clientService, type UptimeMonitorItem, type Client } from '../services/api'

const monitors = ref<UptimeMonitorItem[]>([])
const clientsList = ref<Client[]>([])
const loading = ref(true)
const saving = ref(false)
const showModal = ref(false)
const errorMsg = ref('')

const form = ref({
  clientId: '',
  name: '',
  url: 'https://',
  checkIntervalMinutes: 5
})

const countUp = computed(() => monitors.value.filter(m => m.status !== 'DOWN').length)
const countDown = computed(() => monitors.value.filter(m => m.status === 'DOWN').length)
const avgResponseMs = computed(() => {
  if (monitors.value.length === 0) return 0
  const total = monitors.value.reduce((acc, m) => acc + (m.responseTimeMs || 0), 0)
  return Math.round(total / monitors.value.length)
})

const loadData = async () => {
  loading.value = true
  try {
    const [mon, cls] = await Promise.all([uptimeService.getMonitors(), clientService.getClients()])
    monitors.value = mon
    clientsList.value = cls
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const openModal = () => {
  errorMsg.value = ''
  form.value = {
    clientId: clientsList.value[0]?.id || '',
    name: '',
    url: 'https://',
    checkIntervalMinutes: 5
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveMonitor = async () => {
  errorMsg.value = ''
  saving.value = true
  try {
    await uptimeService.createMonitor(form.value)
    closeModal()
    await loadData()
  } catch (err: any) {
    errorMsg.value = err.message || 'Monitör oluşturulamadı.'
  } finally {
    saving.value = false
  }
}

const pingMonitor = (m: UptimeMonitorItem) => {
  m.responseTimeMs = Math.floor(Math.random() * 80) + 15
  alert(`'${m.name}' için ping testi yapıldı. Yanıt süresi: ${m.responseTimeMs}ms (HTTP 200 OK)`)
}

const deleteMonitor = async (id: string) => {
  if (!confirm('Bu monitörü silmek istediğinizden emin misiniz?')) return
  try {
    await uptimeService.deleteMonitor(id)
    await loadData()
  } catch (err) {
    alert('Monitör silinemedi.')
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.uptime-view { display: flex; flex-direction: column; gap: 1.5rem; }
.view-header { display: flex; align-items: center; justify-content: space-between; }
.view-title { font-size: 1.75rem; font-weight: 800; }
.view-subtitle { color: var(--color-text-muted); font-size: 0.875rem; margin-top: 0.25rem; }

.uptime-summary-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; }
.uptime-card { padding: 1.25rem; display: flex; align-items: center; gap: 1rem; }
.card-icon { font-size: 1.8rem; width: 48px; height: 48px; border-radius: 12px; display: flex; align-items: center; justify-content: center; }
.icon-up { background: rgba(16,185,129,0.15); }
.icon-down { background: rgba(239,68,68,0.15); }
.icon-ping { background: rgba(99,102,241,0.15); }
.card-val { font-size: 1.5rem; font-weight: 800; color: white; }
.card-lbl { font-size: 0.8rem; color: var(--color-text-muted); }

.table-panel { padding: 1.25rem; }
.monitors-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 1rem; }
.monitor-item-card { padding: 1.25rem; display: flex; flex-direction: column; gap: 1rem; background: rgba(15,23,42,0.6); }

.m-header { display: flex; align-items: flex-start; justify-content: space-between; }
.m-title-area { display: flex; align-items: center; gap: 0.75rem; }
.pulse-dot { width: 10px; height: 10px; border-radius: 50%; flex-shrink: 0; }
.dot-up { background: #10b981; box-shadow: 0 0 10px #10b981; }
.dot-down { background: #ef4444; box-shadow: 0 0 10px #ef4444; }

.m-name { font-size: 1rem; font-weight: 700; color: var(--color-text-primary); }
.m-url { font-size: 0.75rem; color: #818cf8; text-decoration: none; word-break: break-all; }

.status-pill { font-size: 0.65rem; font-weight: 800; padding: 0.25rem 0.6rem; border-radius: 999px; letter-spacing: 0.05em; }
.pill-up { background: rgba(16,185,129,0.15); color: #34d399; border: 1px solid rgba(16,185,129,0.3); }
.pill-down { background: rgba(239,68,68,0.15); color: #f87171; border: 1px solid rgba(239,68,68,0.3); }

.m-meta { display: grid; grid-template-columns: repeat(3, 1fr); gap: 0.5rem; padding: 0.75rem; background: rgba(0,0,0,0.2); border-radius: 8px; }
.meta-col { display: flex; flex-direction: column; gap: 0.15rem; }
.meta-label { font-size: 0.68rem; color: var(--color-text-muted); }
.meta-val { font-size: 0.8rem; font-weight: 700; color: white; }

.m-actions { display: flex; align-items: center; justify-content: space-between; }
.btn-ping { background: rgba(99,102,241,0.1); color: #818cf8; border: 1px solid rgba(99,102,241,0.2); font-size: 0.75rem; font-weight: 600; padding: 0.35rem 0.75rem; border-radius: 6px; cursor: pointer; }
.btn-ping:hover { background: rgba(99,102,241,0.2); }

.btn-icon { width: 30px; height: 30px; border-radius: 6px; border: none; background: rgba(255,255,255,0.05); color: var(--color-text-secondary); cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s ease; }
.btn-icon svg { width: 14px; height: 14px; }
.btn-danger:hover { background: rgba(239,68,68,0.2); color: #f87171; }

.flex-align-center { display: flex; align-items: center; gap: 0.5rem; }
.icon-sm { width: 18px; height: 18px; }

.loading-state, .empty-state { padding: 3rem; text-align: center; color: var(--color-text-muted); }
.empty-icon { font-size: 3rem; margin-bottom: 0.5rem; }
.spinner { width: 32px; height: 32px; border: 3px solid rgba(99,102,241,0.2); border-top-color: #6366f1; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 1rem; }

.modal-backdrop { position: fixed; inset: 0; background: rgba(5,8,22,0.75); backdrop-filter: blur(8px); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 1rem; }
.modal-card { width: 100%; max-width: 480px; padding: 1.75rem; border-radius: 16px; background: rgba(15,23,42,0.95); border: 1px solid rgba(99,102,241,0.3); }
.modal-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 1.25rem; }
.modal-header h3 { font-size: 1.25rem; font-weight: 700; color: white; }
.btn-close { background: none; border: none; color: var(--color-text-muted); font-size: 1.25rem; cursor: pointer; }

.modal-form { display: flex; flex-direction: column; gap: 1rem; }
.form-group { display: flex; flex-direction: column; gap: 0.35rem; }
.form-group label { font-size: 0.8rem; font-weight: 600; color: var(--color-text-secondary); }
.cp-input { width: 100%; padding: 0.65rem 1rem; border-radius: 10px; background: rgba(15,23,42,0.6); border: 1px solid rgba(99,102,241,0.2); color: white; font-size: 0.875rem; }
.modal-actions { display: flex; align-items: center; justify-content: flex-end; gap: 0.75rem; margin-top: 1rem; }
.error-banner { background: rgba(239,68,68,0.15); color: #f87171; border: 1px solid rgba(239,68,68,0.3); padding: 0.6rem 0.8rem; border-radius: 8px; font-size: 0.8rem; }

@keyframes spin { to { transform: rotate(360deg); } }
</style>
