<template>
  <div class="invoices-view animate-fade-in">
    <!-- Header -->
    <div class="view-header">
      <div>
        <h1 class="view-title gradient-text">Fatura & Taksit Takibi</h1>
        <p class="view-subtitle">Ödemeleri ve taksitli faturaları takip edin, son ödeme tarihlerini yönetin.</p>
      </div>
      <button class="btn-primary flex-align-center" @click="openModal()">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="icon-sm"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
        Fatura / Taksit Oluştur
      </button>
    </div>

    <!-- Quick Stats -->
    <div class="invoice-stats-grid">
      <div class="glass-card stat-box">
        <div class="stat-lbl">Bekleyen Tutarlar</div>
        <div class="stat-val text-amber">₺{{ pendingTotal.toLocaleString('tr-TR') }}</div>
      </div>
      <div class="glass-card stat-box">
        <div class="stat-lbl">Tahsil Edilen (Ödendi)</div>
        <div class="stat-val text-emerald">₺{{ paidTotal.toLocaleString('tr-TR') }}</div>
      </div>
      <div class="glass-card stat-box">
        <div class="stat-lbl">Gecikmiş Faturalar</div>
        <div class="stat-val text-rose">₺{{ overdueTotal.toLocaleString('tr-TR') }}</div>
      </div>
    </div>

    <!-- Status Tabs & Filter -->
    <div class="glass-card filter-bar">
      <div class="tab-buttons">
        <button class="tab-btn" :class="{ active: statusFilter === 'ALL' }" @click="statusFilter = 'ALL'">Tümü ({{ invoices.length }})</button>
        <button class="tab-btn" :class="{ active: statusFilter === 'Pending' }" @click="statusFilter = 'Pending'">Bekliyor</button>
        <button class="tab-btn" :class="{ active: statusFilter === 'Paid' }" @click="statusFilter = 'Paid'">Ödendi</button>
        <button class="tab-btn" :class="{ active: statusFilter === 'Overdue' }" @click="statusFilter = 'Overdue'">Gecikmiş</button>
      </div>
    </div>

    <!-- Invoices Table -->
    <div class="glass-card table-panel">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <span>Faturalar yükleniyor...</span>
      </div>

      <div v-else-if="filteredInvoices.length === 0" class="empty-state">
        <div class="empty-icon">🧾</div>
        <h3>Fatura Kaydı Yok</h3>
        <p>Seçilen filtreye ait hiçbir fatura kaydı bulunmamaktadır.</p>
      </div>

      <table v-else class="cp-table">
        <thead>
          <tr>
            <th>Müşteri / Açıklama</th>
            <th>Taktis Bilgisi</th>
            <th>Tutar</th>
            <th>Son Ödeme</th>
            <th>Durum</th>
            <th style="text-align:right;">Aksiyon</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="inv in filteredInvoices" :key="inv.id" class="table-row-hover">
            <td>
              <div class="client-cell">
                <div class="client-avatar">{{ (inv.clientName || 'M')[0] }}</div>
                <div>
                  <div class="client-name">{{ inv.clientName || 'Müşteri' }}</div>
                  <div class="notes-sub">{{ inv.notes || 'Fatura' }}</div>
                </div>
              </div>
            </td>
            <td>
              <span v-if="getInstallmentTag(inv.notes)" class="badge badge-taksit">
                💳 {{ getInstallmentTag(inv.notes) }}
              </span>
              <span v-else class="badge badge-single">
                Tek Çekim
              </span>
            </td>
            <td>
              <div class="amount-val">₺{{ inv.amount.toLocaleString('tr-TR') }} <span class="curr-lbl">{{ inv.currency }}</span></div>
            </td>
            <td class="date-lbl" :class="{ 'date-overdue': getStatusKey(inv.status) === 'Overdue' }">{{ formatDate(inv.dueDate) }}</td>
            <td>
              <span class="badge" :class="statusBadge(inv.status)">{{ formatStatus(inv.status) }}</span>
            </td>
            <td style="text-align:right;">
              <div class="action-buttons">
                <button v-if="getStatusKey(inv.status) !== 'Paid'" class="btn-sm btn-success" title="Ödendi İşaretle" @click="markAsPaid(inv.id)">
                  ✓ Ödendi Yap
                </button>
                <button class="btn-icon btn-danger" title="Sil" @click="deleteInvoice(inv.id)">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="modal-backdrop animate-fade-in" @click.self="closeModal">
      <div class="glass-card modal-card animate-fade-in-up">
        <div class="modal-header">
          <h3>Yeni Fatura / Taksit Planı Oluştur</h3>
          <button class="btn-close" @click="closeModal">✕</button>
        </div>
        <form @submit.prevent="saveInvoice" class="modal-form">
          <div class="form-group">
            <label>Müşteri Seçin *</label>
            <select v-model="form.clientId" class="cp-input" required>
              <option value="" disabled>-- Müşteri Seçin --</option>
              <option v-for="c in clientsList" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>
          <div class="form-grid">
            <div class="form-group">
              <label>Toplam Tutar (₺) *</label>
              <input v-model.number="form.amount" type="number" step="0.01" class="cp-input" required placeholder="5500" />
            </div>
            <div class="form-group">
              <label>Para Birimi</label>
              <select v-model="form.currency" class="cp-input">
                <option value="TRY">TRY (₺)</option>
                <option value="USD">USD ($)</option>
                <option value="EUR">EUR (€)</option>
              </select>
            </div>
          </div>

          <div class="form-grid">
            <div class="form-group">
              <label>Taksit Sayısı</label>
              <select v-model.number="form.installmentsCount" class="cp-input">
                <option :value="1">Tek Çekim (Peşin)</option>
                <option :value="2">2 Taksit (Aylık)</option>
                <option :value="3">3 Taksit (Aylık)</option>
                <option :value="6">6 Taksit (Aylık)</option>
                <option :value="12">12 Taksit (Aylık)</option>
              </select>
            </div>
            <div class="form-group">
              <label>İlk Taksit Son Ödeme *</label>
              <input v-model="form.dueDate" type="date" class="cp-input" required />
            </div>
          </div>

          <div class="form-group">
            <label>Fatura Açıklaması / Başlık</label>
            <input v-model="form.notes" type="text" class="cp-input" placeholder="Örn: Web Tasarım Ödemesi" />
          </div>

          <div class="calc-preview">
            💡 <strong>Taksit Planı Özeti:</strong> ₺{{ form.amount.toLocaleString('tr-TR') }} {{ form.installmentsCount > 1 ? `(${form.installmentsCount} Taksit)` : 'Tek Çekim' }}
            <span v-if="form.installmentsCount > 1" class="calc-sub">
              ➔ Her Ay <strong>₺{{ (form.amount / form.installmentsCount).toFixed(2) }}</strong> × {{ form.installmentsCount }} Fatura Oluşturulacak
            </span>
          </div>

          <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" @click="closeModal">İptal</button>
            <button type="submit" class="btn-primary" :disabled="saving">
              {{ saving ? 'Oluşturuluyor...' : 'Faturayı / Taksitleri Oluştur' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { invoiceService, clientService, type Invoice, type Client } from '../services/api'

const invoices = ref<Invoice[]>([])
const clientsList = ref<Client[]>([])
const loading = ref(true)
const saving = ref(false)
const statusFilter = ref('ALL')
const showModal = ref(false)
const errorMsg = ref('')

const form = ref({
  clientId: '',
  amount: 5500,
  currency: 'TRY',
  issueDate: new Date().toISOString().split('T')[0],
  dueDate: new Date(Date.now() + 14 * 86400000).toISOString().split('T')[0],
  notes: '',
  installmentsCount: 1
})

const getStatusKey = (s: string | number): string => {
  if (typeof s === 'number') {
    return ['Pending', 'Paid', 'Overdue', 'Cancelled'][s] || 'Pending'
  }
  return s
}

const filteredInvoices = computed(() => {
  if (statusFilter.value === 'ALL') return invoices.value
  return invoices.value.filter(i => getStatusKey(i.status) === statusFilter.value)
})

const pendingTotal = computed(() => {
  return invoices.value.filter(i => getStatusKey(i.status) === 'Pending').reduce((acc, i) => acc + i.amount, 0)
})

const paidTotal = computed(() => {
  return invoices.value.filter(i => getStatusKey(i.status) === 'Paid').reduce((acc, i) => acc + i.amount, 0)
})

const overdueTotal = computed(() => {
  return invoices.value.filter(i => getStatusKey(i.status) === 'Overdue').reduce((acc, i) => acc + i.amount, 0)
})

const loadData = async () => {
  loading.value = true
  try {
    const [inv, cls] = await Promise.all([invoiceService.getInvoices(), clientService.getClients()])
    invoices.value = inv
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
    amount: 5500,
    currency: 'TRY',
    issueDate: new Date().toISOString().split('T')[0],
    dueDate: new Date(Date.now() + 14 * 86400000).toISOString().split('T')[0],
    notes: '',
    installmentsCount: 1
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveInvoice = async () => {
  errorMsg.value = ''
  saving.value = true
  try {
    await invoiceService.createInvoice(form.value)
    closeModal()
    await loadData()
  } catch (err: any) {
    errorMsg.value = err.message || 'Fatura oluşturulamadı.'
  } finally {
    saving.value = false
  }
}

const markAsPaid = async (id: string) => {
  try {
    await invoiceService.updateStatus(id, 'Paid')
    await loadData()
  } catch (err) {
    alert('Fatura durumu güncellenemedi.')
  }
}

const deleteInvoice = async (id: string) => {
  if (!confirm('Bu faturayı silmek istediğinizden emin misiniz?')) return
  try {
    await invoiceService.deleteInvoice(id)
    await loadData()
  } catch (err) {
    alert('Fatura silinemedi.')
  }
}

const getInstallmentTag = (notes?: string) => {
  if (!notes) return null
  const match = notes.match(/Taksit\s+(\d+\/\d+)/i)
  return match ? `Taksit ${match[1]}` : null
}

const formatStatus = (statusStr: string | number) => {
  const k = getStatusKey(statusStr)
  if (k === 'Pending') return 'Bekliyor'
  if (k === 'Paid') return 'Ödendi'
  if (k === 'Overdue') return 'Gecikmiş'
  return 'İptal'
}

const statusBadge = (statusStr: string | number) => {
  const k = getStatusKey(statusStr)
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
  loadData()
})
</script>

<style scoped>
.invoices-view { display: flex; flex-direction: column; gap: 1.5rem; }
.view-header { display: flex; align-items: center; justify-content: space-between; }
.view-title { font-size: 1.75rem; font-weight: 800; }
.view-subtitle { color: var(--color-text-muted); font-size: 0.875rem; margin-top: 0.25rem; }

.invoice-stats-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; }
.stat-box { padding: 1.25rem; display: flex; flex-direction: column; gap: 0.25rem; }
.stat-lbl { font-size: 0.8rem; color: var(--color-text-muted); font-weight: 500; }
.stat-val { font-size: 1.6rem; font-weight: 800; }

.text-amber { color: #f59e0b; }
.text-emerald { color: #10b981; }
.text-rose { color: #f43f5e; }

.filter-bar { padding: 0.75rem 1.25rem; }
.tab-buttons { display: flex; gap: 0.5rem; }
.tab-btn { background: none; border: none; padding: 0.5rem 1rem; border-radius: 8px; color: var(--color-text-muted); font-size: 0.85rem; font-weight: 600; cursor: pointer; transition: all 0.2s ease; }
.tab-btn.active { background: rgba(99,102,241,0.15); color: #818cf8; border: 1px solid rgba(99,102,241,0.3); }

.table-panel { padding: 1.25rem; overflow-x: auto; }
.client-cell { display: flex; align-items: center; gap: 0.75rem; }
.client-avatar { width: 32px; height: 32px; border-radius: 50%; background: linear-gradient(135deg, #6366f1, #8b5cf6); display: flex; align-items: center; justify-content: center; font-weight: 700; color: white; font-size: 0.8rem; flex-shrink: 0; }
.client-name { font-weight: 700; font-size: 0.9rem; color: var(--color-text-primary); }
.notes-sub { font-size: 0.72rem; color: var(--color-text-muted); }

.badge-taksit { background: rgba(139,92,246,0.15); color: #c084fc; border: 1px solid rgba(139,92,246,0.3); font-size: 0.75rem; font-weight: 700; }
.badge-single { background: rgba(255,255,255,0.05); color: var(--color-text-muted); border: 1px solid rgba(255,255,255,0.1); font-size: 0.72rem; }

.amount-val { font-weight: 800; font-size: 0.95rem; color: var(--color-text-primary); }
.curr-lbl { font-size: 0.75rem; color: var(--color-text-muted); }
.date-lbl { font-size: 0.8rem; color: var(--color-text-muted); }
.date-overdue { color: #f43f5e; font-weight: 700; }

.action-buttons { display: flex; align-items: center; justify-content: flex-end; gap: 0.5rem; }
.btn-sm { padding: 0.35rem 0.7rem; border-radius: 6px; border: none; font-size: 0.75rem; font-weight: 700; cursor: pointer; transition: all 0.2s ease; }
.btn-success { background: rgba(16,185,129,0.15); color: #34d399; border: 1px solid rgba(16,185,129,0.3); }
.btn-success:hover { background: rgba(16,185,129,0.25); }

.btn-icon { width: 30px; height: 30px; border-radius: 6px; border: none; background: rgba(255,255,255,0.05); color: var(--color-text-secondary); cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s ease; }
.btn-icon svg { width: 14px; height: 14px; }
.btn-danger:hover { background: rgba(239,68,68,0.2); color: #f87171; }

.flex-align-center { display: flex; align-items: center; gap: 0.5rem; }
.icon-sm { width: 18px; height: 18px; }

.loading-state, .empty-state { padding: 3rem; text-align: center; color: var(--color-text-muted); }
.empty-icon { font-size: 3rem; margin-bottom: 0.5rem; }
.spinner { width: 32px; height: 32px; border: 3px solid rgba(99,102,241,0.2); border-top-color: #6366f1; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 1rem; }

.modal-backdrop { position: fixed; inset: 0; background: rgba(5,8,22,0.75); backdrop-filter: blur(8px); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 1rem; }
.modal-card { width: 100%; max-width: 500px; padding: 1.75rem; border-radius: 16px; background: rgba(15,23,42,0.95); border: 1px solid rgba(99,102,241,0.3); }
.modal-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 1.25rem; }
.modal-header h3 { font-size: 1.25rem; font-weight: 700; color: white; }
.btn-close { background: none; border: none; color: var(--color-text-muted); font-size: 1.25rem; cursor: pointer; }

.modal-form { display: flex; flex-direction: column; gap: 1rem; }
.form-group { display: flex; flex-direction: column; gap: 0.35rem; }
.form-group label { font-size: 0.8rem; font-weight: 600; color: var(--color-text-secondary); }
.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0.75rem; }
.cp-input { width: 100%; padding: 0.65rem 1rem; border-radius: 10px; background: rgba(15,23,42,0.6); border: 1px solid rgba(99,102,241,0.2); color: white; font-size: 0.875rem; }

.calc-preview { font-size: 0.8rem; color: #818cf8; background: rgba(99,102,241,0.08); padding: 0.65rem 0.8rem; border-radius: 8px; border: 1px solid rgba(99,102,241,0.2); }
.calc-sub { display: block; margin-top: 0.2rem; color: #34d399; }

.modal-actions { display: flex; align-items: center; justify-content: flex-end; gap: 0.75rem; margin-top: 1rem; }
.error-banner { background: rgba(239,68,68,0.15); color: #f87171; border: 1px solid rgba(239,68,68,0.3); padding: 0.6rem 0.8rem; border-radius: 8px; font-size: 0.8rem; }

@keyframes spin { to { transform: rotate(360deg); } }
</style>
