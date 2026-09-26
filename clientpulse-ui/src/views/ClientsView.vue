<template>
  <div class="clients-view animate-fade-in">
    <!-- Header Action Bar -->
    <div class="view-header">
      <div>
        <h1 class="view-title gradient-text">Müşteri Yönetimi</h1>
        <p class="view-subtitle">Tüm müşterilerinizi listeleyin, bağlı hizmet ve faturalarını yönetin.</p>
      </div>
      <button class="btn-primary flex-align-center" @click="openModal()">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="icon-sm">
          <line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/>
        </svg>
        Yeni Müşteri Ekle
      </button>
    </div>

    <!-- Search & Filter Bar -->
    <div class="glass-card filter-bar">
      <div class="search-input-wrapper">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="search-icon">
          <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input v-model="searchQuery" type="text" placeholder="Müşteri adı, e-posta veya şirket ara..." class="cp-input" />
      </div>
      <div class="filter-stats">
        <span>Toplam <strong>{{ filteredClients.length }}</strong> Müşteri</span>
      </div>
    </div>

    <!-- Clients Grid / Table -->
    <div class="glass-card table-panel">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <span>Müşteri verileri yükleniyor...</span>
      </div>

      <div v-else-if="filteredClients.length === 0" class="empty-state">
        <div class="empty-icon">👥</div>
        <h3>Müşteri Bulunamadı</h3>
        <p>Arama kriterlerinize uyan veya henüz eklenmiş müşteri yok.</p>
      </div>

      <table v-else class="cp-table">
        <thead>
          <tr>
            <th>Müşteri / Şirket</th>
            <th>İletişim</th>
            <th>Hizmetler</th>
            <th>Faturalar</th>
            <th>Kayıt Tarihi</th>
            <th style="text-align:right;">İşlemler</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in filteredClients" :key="c.id" class="table-row-hover">
            <td>
              <div class="client-info">
                <div class="avatar">{{ c.name[0].toUpperCase() }}</div>
                <div>
                  <div class="client-name">{{ c.name }}</div>
                  <div class="company-name">{{ c.companyName || 'Bireysel' }}</div>
                </div>
              </div>
            </td>
            <td>
              <div class="contact-cell">
                <div class="email-text">✉ {{ c.email }}</div>
                <div v-if="c.phone" class="phone-text">📞 {{ c.phone }}</div>
              </div>
            </td>
            <td>
              <span class="badge badge-info">{{ c.servicesCount || 0 }} Hizmet</span>
            </td>
            <td>
              <span class="badge badge-purple">{{ c.invoicesCount || 0 }} Fatura</span>
            </td>
            <td class="date-cell">
              {{ formatDate(c.createdAt) }}
            </td>
            <td style="text-align:right;">
              <div class="action-buttons">
                <button class="btn-sm btn-detail" title="Müşteri Profili & Hizmetleri" @click="openDetailModal(c.id)">
                  👁️ Profil & Detay
                </button>
                <button class="btn-icon btn-edit" title="Düzenle" @click="openModal(c)">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
                </button>
                <button class="btn-icon btn-danger" title="Sil" @click="deleteClient(c.id)">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Modal Form -->
    <div v-if="showModal" class="modal-backdrop animate-fade-in" @click.self="closeModal">
      <div class="glass-card modal-card animate-fade-in-up">
        <div class="modal-header">
          <h3>{{ editingClient ? 'Müşteri Düzenle' : 'Yeni Müşteri Ekle' }}</h3>
          <button class="btn-close" @click="closeModal">✕</button>
        </div>
        <form @submit.prevent="saveClient" class="modal-form">
          <div class="form-group">
            <label>Müşteri / Ad Soyad *</label>
            <input v-model="form.name" type="text" class="cp-input" required placeholder="Örn: Ahmet Yılmaz" />
          </div>
          <div class="form-group">
            <label>E-Posta Adresi *</label>
            <input v-model="form.email" type="email" class="cp-input" required placeholder="Örn: ahmet@company.com" />
          </div>
          <div class="form-grid">
            <div class="form-group">
              <label>Telefon</label>
              <input v-model="form.phone" type="text" class="cp-input" placeholder="Örn: +90 532 000 0000" />
            </div>
            <div class="form-group">
              <label>Şirket Adı</label>
              <input v-model="form.companyName" type="text" class="cp-input" placeholder="Örn: Yılmaz A.Ş." />
            </div>
          </div>
          <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" @click="closeModal">İptal</button>
            <button type="submit" class="btn-primary" :disabled="saving">
              {{ saving ? 'Kaydediliyor...' : 'Kaydet' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Client Profile / Detail Modal -->
    <div v-if="showDetailModal" class="modal-backdrop animate-fade-in" @click.self="closeDetailModal">
      <div class="glass-card modal-card detail-modal-card animate-fade-in-up">
        <div class="modal-header">
          <div class="detail-header-info">
            <h3>{{ selectedClientDetail?.name }}</h3>
            <span class="sub-company">{{ selectedClientDetail?.companyName || 'Bireysel Müşteri' }}</span>
          </div>
          <button class="btn-close" @click="closeDetailModal">✕</button>
        </div>

        <div v-if="loadingDetail" class="loading-state">
          <div class="spinner"></div>
          <span>Müşteri detayları ve bağlı servisler yükleniyor...</span>
        </div>

        <div v-else-if="selectedClientDetail" class="detail-content">
          <!-- Profile Quick Stats -->
          <div class="profile-stats-bar">
            <div class="p-stat">
              <span>✉ E-Posta:</span> <strong>{{ selectedClientDetail.email }}</strong>
            </div>
            <div class="p-stat">
              <span>📞 Telefon:</span> <strong>{{ selectedClientDetail.phone || '-' }}</strong>
            </div>
          </div>

          <!-- Tabs -->
          <div class="detail-tabs">
            <button class="d-tab-btn" :class="{ active: activeTab === 'services' }" @click="activeTab = 'services'">
              ⚡ Hizmetler ({{ selectedClientDetail.services?.length || 0 }})
            </button>
            <button class="d-tab-btn" :class="{ active: activeTab === 'invoices' }" @click="activeTab = 'invoices'">
              🧾 Faturalar ({{ selectedClientDetail.invoices?.length || 0 }})
            </button>
            <button class="d-tab-btn" :class="{ active: activeTab === 'uptime' }" @click="activeTab = 'uptime'">
              📡 Uptime Sitelere ({{ selectedClientDetail.uptimeMonitors?.length || 0 }})
            </button>
          </div>

          <!-- Tab Content 1: Services -->
          <div v-if="activeTab === 'services'" class="tab-pane">
            <div class="pane-header">
              <h4>Müşterinin Aktif Hizmetleri</h4>
              <RouterLink to="/services" class="btn-sm btn-primary">+ Hizmet Ekle</RouterLink>
            </div>
            <div v-if="!selectedClientDetail.services?.length" class="empty-sub">
              Bu müşteriye henüz tanımlanmış bir hizmet bulunmuyor.
            </div>
            <div v-else class="sub-list">
              <div v-for="s in selectedClientDetail.services" :key="s.id" class="sub-item-card">
                <div>
                  <strong class="item-title">{{ s.name }}</strong>
                  <div class="item-desc">{{ s.description || 'Açıklama yok' }}</div>
                </div>
                <div class="item-price">₺{{ s.price.toLocaleString('tr-TR') }} {{ s.currency }}</div>
              </div>
            </div>
          </div>

          <!-- Tab Content 2: Invoices -->
          <div v-if="activeTab === 'invoices'" class="tab-pane">
            <div class="pane-header">
              <h4>Müşterinin Faturaları</h4>
              <RouterLink to="/invoices" class="btn-sm btn-primary">+ Fatura Kes</RouterLink>
            </div>
            <div v-if="!selectedClientDetail.invoices?.length" class="empty-sub">
              Bu müşteriye ait kesilmiş fatura bulunmuyor.
            </div>
            <div v-else class="sub-list">
              <div v-for="inv in selectedClientDetail.invoices" :key="inv.id" class="sub-item-card">
                <div>
                  <strong class="item-title">Fatura Tutarı: ₺{{ inv.amount.toLocaleString('tr-TR') }}</strong>
                  <div class="item-desc">Son Ödeme: {{ formatDate(inv.dueDate) }} | {{ inv.notes || 'Detay yok' }}</div>
                </div>
                <span class="badge" :class="statusBadge(inv.status)">{{ formatStatus(inv.status) }}</span>
              </div>
            </div>
          </div>

          <!-- Tab Content 3: Uptime -->
          <div v-if="activeTab === 'uptime'" class="tab-pane">
            <div class="pane-header">
              <h4>İzlenen Web Siteleri & Servisler</h4>
              <RouterLink to="/uptime" class="btn-sm btn-primary">+ Monitör Ekle</RouterLink>
            </div>
            <div v-if="!selectedClientDetail.uptimeMonitors?.length" class="empty-sub">
              Bu müşteriye ait izlenen bir web sitesi bulunmuyor.
            </div>
            <div v-else class="sub-list">
              <div v-for="u in selectedClientDetail.uptimeMonitors" :key="u.id" class="sub-item-card">
                <div>
                  <strong class="item-title">{{ u.name }}</strong>
                  <div class="item-desc"><a :href="u.url" target="_blank" style="color:#818cf8">{{ u.url }}</a></div>
                </div>
                <span class="badge badge-success">● ONLINE (Her {{ u.checkIntervalMinutes }} dk)</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { clientService, type Client } from '../services/api'

const clients = ref<Client[]>([])
const loading = ref(true)
const saving = ref(false)
const searchQuery = ref('')
const showModal = ref(false)
const editingClient = ref<Client | null>(null)
const errorMsg = ref('')

// Detail Modal State
const showDetailModal = ref(false)
const loadingDetail = ref(false)
const selectedClientDetail = ref<any>(null)
const activeTab = ref('services')

const form = ref({
  name: '',
  email: '',
  phone: '',
  companyName: ''
})

const filteredClients = computed(() => {
  if (!searchQuery.value.trim()) return clients.value
  const q = searchQuery.value.toLowerCase()
  return clients.value.filter(c =>
    c.name.toLowerCase().includes(q) ||
    c.email.toLowerCase().includes(q) ||
    (c.companyName && c.companyName.toLowerCase().includes(q))
  )
})

const fetchClients = async () => {
  loading.value = true
  try {
    clients.value = await clientService.getClients()
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const openModal = (client?: Client) => {
  errorMsg.value = ''
  if (client) {
    editingClient.value = client
    form.value = {
      name: client.name,
      email: client.email,
      phone: client.phone || '',
      companyName: client.companyName || ''
    }
  } else {
    editingClient.value = null
    form.value = { name: '', email: '', phone: '', companyName: '' }
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  editingClient.value = null
}

const openDetailModal = async (clientId: string) => {
  showDetailModal.value = true
  loadingDetail.value = true
  activeTab.value = 'services'
  try {
    selectedClientDetail.value = await clientService.getClientById(clientId)
  } catch (err) {
    console.error(err)
  } finally {
    loadingDetail.value = false
  }
}

const closeDetailModal = () => {
  showDetailModal.value = false
  selectedClientDetail.value = null
}

const saveClient = async () => {
  errorMsg.value = ''
  saving.value = true
  try {
    if (editingClient.value) {
      await clientService.updateClient(editingClient.value.id, form.value)
    } else {
      await clientService.createClient(form.value)
    }
    closeModal()
    await fetchClients()
  } catch (err: any) {
    errorMsg.value = err.message || 'Bir hata oluştu.'
  } finally {
    saving.value = false
  }
}

const deleteClient = async (id: string) => {
  if (!confirm('Bu müşteriyi silmek istediğinizden emin misiniz?')) return
  try {
    await clientService.deleteClient(id)
    await fetchClients()
  } catch (err) {
    alert('Silme sırasında bir hata oluştu.')
  }
}

const formatStatus = (s: string | number) => {
  const k = typeof s === 'number' ? ['Pending', 'Paid', 'Overdue', 'Cancelled'][s] : s
  if (k === 'Pending') return 'Bekliyor'
  if (k === 'Paid') return 'Ödendi'
  if (k === 'Overdue') return 'Gecikmiş'
  return 'İptal'
}

const statusBadge = (s: string | number) => {
  const k = typeof s === 'number' ? ['Pending', 'Paid', 'Overdue', 'Cancelled'][s] : s
  return {
    'badge-warning': k === 'Pending',
    'badge-success': k === 'Paid',
    'badge-danger':  k === 'Overdue',
  }
}

const formatDate = (dateStr?: string) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('tr-TR', { day: '2-digit', month: 'short', year: 'numeric' })
}

onMounted(() => {
  fetchClients()
})
</script>

<style scoped>
.clients-view { display: flex; flex-direction: column; gap: 1.5rem; }
.view-header { display: flex; align-items: center; justify-content: space-between; }
.view-title { font-size: 1.75rem; font-weight: 800; }
.view-subtitle { color: var(--color-text-muted); font-size: 0.875rem; margin-top: 0.25rem; }

.filter-bar { display: flex; align-items: center; justify-content: space-between; padding: 1rem 1.25rem; gap: 1rem; }
.search-input-wrapper { position: relative; flex: 1; max-width: 420px; }
.search-icon { position: absolute; left: 1rem; top: 50%; transform: translateY(-50%); width: 18px; height: 18px; color: var(--color-text-muted); }
.cp-input { width: 100%; padding: 0.65rem 1rem 0.65rem 2.6rem; border-radius: 10px; background: rgba(15,23,42,0.6); border: 1px solid rgba(99,102,241,0.2); color: white; font-size: 0.875rem; transition: border-color 0.2s ease; }
.cp-input:focus { outline: none; border-color: #6366f1; box-shadow: 0 0 12px rgba(99,102,241,0.3); }

.filter-stats { font-size: 0.85rem; color: var(--color-text-muted); }
.filter-stats strong { color: var(--color-text-primary); }

.table-panel { padding: 1.25rem; overflow-x: auto; }
.client-info { display: flex; align-items: center; gap: 0.75rem; }
.avatar { width: 36px; height: 36px; border-radius: 50%; background: linear-gradient(135deg, #6366f1, #a855f7); display: flex; align-items: center; justify-content: center; font-weight: 700; color: white; font-size: 0.9rem; flex-shrink: 0; }
.client-name { font-weight: 700; font-size: 0.9rem; color: var(--color-text-primary); }
.company-name { font-size: 0.75rem; color: var(--color-text-muted); }

.contact-cell { font-size: 0.8rem; }
.email-text { color: var(--color-text-secondary); }
.phone-text { color: var(--color-text-muted); margin-top: 0.15rem; }

.badge-info { background: rgba(59,130,246,0.15); color: #60a5fa; border: 1px solid rgba(59,130,246,0.3); }
.badge-purple { background: rgba(168,85,247,0.15); color: #c084fc; border: 1px solid rgba(168,85,247,0.3); }

.date-cell { font-size: 0.8rem; color: var(--color-text-muted); }

.action-buttons { display: flex; align-items: center; justify-content: flex-end; gap: 0.5rem; }
.btn-sm { padding: 0.35rem 0.75rem; border-radius: 8px; border: none; font-size: 0.75rem; font-weight: 700; cursor: pointer; text-decoration: none; display: inline-flex; align-items: center; gap: 0.3rem; }
.btn-detail { background: rgba(99,102,241,0.15); color: #818cf8; border: 1px solid rgba(99,102,241,0.3); }
.btn-detail:hover { background: rgba(99,102,241,0.25); }

.btn-icon { width: 32px; height: 32px; border-radius: 8px; border: none; background: rgba(255,255,255,0.05); color: var(--color-text-secondary); cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s ease; }
.btn-icon svg { width: 15px; height: 15px; }
.btn-edit:hover { background: rgba(99,102,241,0.2); color: #818cf8; }
.btn-danger:hover { background: rgba(239,68,68,0.2); color: #f87171; }

.flex-align-center { display: flex; align-items: center; gap: 0.5rem; }
.icon-sm { width: 18px; height: 18px; }

.loading-state, .empty-state { padding: 3rem; text-align: center; color: var(--color-text-muted); }
.empty-icon { font-size: 3rem; margin-bottom: 0.5rem; }
.spinner { width: 32px; height: 32px; border: 3px solid rgba(99,102,241,0.2); border-top-color: #6366f1; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 1rem; }

/* Modal Styles */
.modal-backdrop { position: fixed; inset: 0; background: rgba(5,8,22,0.75); backdrop-filter: blur(8px); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 1rem; }
.modal-card { width: 100%; max-width: 500px; padding: 1.75rem; border-radius: 16px; background: rgba(15,23,42,0.95); border: 1px solid rgba(99,102,241,0.3); }
.detail-modal-card { max-width: 650px; }
.modal-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 1.25rem; }
.modal-header h3 { font-size: 1.25rem; font-weight: 700; color: white; }
.sub-company { font-size: 0.8rem; color: var(--color-text-muted); }
.btn-close { background: none; border: none; color: var(--color-text-muted); font-size: 1.25rem; cursor: pointer; }

.modal-form { display: flex; flex-direction: column; gap: 1rem; }
.form-group { display: flex; flex-direction: column; gap: 0.35rem; }
.form-group label { font-size: 0.8rem; font-weight: 600; color: var(--color-text-secondary); }
.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0.75rem; }

.modal-actions { display: flex; align-items: center; justify-content: flex-end; gap: 0.75rem; margin-top: 1rem; }
.error-banner { background: rgba(239,68,68,0.15); color: #f87171; border: 1px solid rgba(239,68,68,0.3); padding: 0.6rem 0.8rem; border-radius: 8px; font-size: 0.8rem; }

/* Detail Content & Tabs */
.profile-stats-bar { display: flex; gap: 1.5rem; padding: 0.75rem 1rem; background: rgba(0,0,0,0.2); border-radius: 10px; margin-bottom: 1rem; font-size: 0.85rem; }
.p-stat { color: var(--color-text-muted); }
.p-stat strong { color: white; }

.detail-tabs { display: flex; gap: 0.5rem; margin-bottom: 1rem; border-bottom: 1px solid rgba(255,255,255,0.08); padding-bottom: 0.5rem; }
.d-tab-btn { background: none; border: none; padding: 0.45rem 0.85rem; border-radius: 8px; color: var(--color-text-muted); font-size: 0.8rem; font-weight: 600; cursor: pointer; transition: all 0.2s ease; }
.d-tab-btn.active { background: rgba(99,102,241,0.15); color: #818cf8; border: 1px solid rgba(99,102,241,0.3); }

.tab-pane { display: flex; flex-direction: column; gap: 0.75rem; }
.pane-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 0.25rem; }
.pane-header h4 { font-size: 0.95rem; font-weight: 700; color: white; }

.empty-sub { padding: 1.5rem; text-align: center; color: var(--color-text-muted); font-size: 0.85rem; background: rgba(0,0,0,0.15); border-radius: 10px; }
.sub-list { display: flex; flex-direction: column; gap: 0.5rem; max-height: 260px; overflow-y: auto; }
.sub-item-card { display: flex; align-items: center; justify-content: space-between; padding: 0.75rem 1rem; background: rgba(15,23,42,0.6); border: 1px solid rgba(255,255,255,0.06); border-radius: 10px; }
.item-title { font-size: 0.85rem; color: white; }
.item-desc { font-size: 0.75rem; color: var(--color-text-muted); margin-top: 0.15rem; }
.item-price { font-weight: 800; color: #10b981; font-size: 0.9rem; }

@keyframes spin { to { transform: rotate(360deg); } }
</style>
