<template>
  <div class="services-view animate-fade-in">
    <!-- Header -->
    <div class="view-header">
      <div>
        <h1 class="view-title gradient-text">Hizmet Yönetimi</h1>
        <p class="view-subtitle">Müşterilerinize sunduğunuz düzenli ve projelik hizmetleri tanımlayın.</p>
      </div>
      <button class="btn-primary flex-align-center" @click="openModal()">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="icon-sm"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
        Yeni Hizmet Ekle
      </button>
    </div>

    <!-- Filter & Summary -->
    <div class="glass-card filter-bar">
      <div class="search-input-wrapper">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="search-icon"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
        <input v-model="searchQuery" type="text" placeholder="Hizmet adı veya müşteri ara..." class="cp-input" />
      </div>
      <div class="filter-stats">
        <span>Toplam Hizmet Değeri: <strong class="highlight-val">₺{{ totalPrice.toLocaleString('tr-TR') }}</strong></span>
      </div>
    </div>

    <!-- Services Grid / Table -->
    <div class="glass-card table-panel">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <span>Hizmetler yükleniyor...</span>
      </div>

      <div v-else-if="filteredServices.length === 0" class="empty-state">
        <div class="empty-icon">⚡</div>
        <h3>Hizmet Bulunamadı</h3>
        <p>Aramanıza uygun aktif bir hizmet kaydı yok.</p>
      </div>

      <table v-else class="cp-table">
        <thead>
          <tr>
            <th>Hizmet Adı</th>
            <th>Müşteri</th>
            <th>Açıklama</th>
            <th>Fiyat</th>
            <th style="text-align:right;">İşlemler</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="s in filteredServices" :key="s.id" class="table-row-hover">
            <td>
              <div class="service-name-cell">
                <div class="service-icon-box">⚡</div>
                <div class="service-title">{{ s.name }}</div>
              </div>
            </td>
            <td>
              <div class="client-badge">
                👤 {{ s.clientName || 'Bilinmeyen Müşteri' }}
              </div>
            </td>
            <td>
              <div class="desc-text">{{ s.description || 'Açıklama girilmedi.' }}</div>
            </td>
            <td>
              <div class="price-text">₺{{ s.price.toLocaleString('tr-TR') }} <span class="curr-sub">{{ s.currency }}</span></div>
            </td>
            <td style="text-align:right;">
              <button class="btn-icon btn-danger" title="Sil" @click="deleteService(s.id)">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal Form -->
    <div v-if="showModal" class="modal-backdrop animate-fade-in" @click.self="closeModal">
      <div class="glass-card modal-card animate-fade-in-up">
        <div class="modal-header">
          <h3>Yeni Hizmet Tanımla</h3>
          <button class="btn-close" @click="closeModal">✕</button>
        </div>
        <form @submit.prevent="saveService" class="modal-form">
          <div class="form-group">
            <label>Müşteri Seçin *</label>
            <select v-model="form.clientId" class="cp-input" required>
              <option value="" disabled>-- Müşteri Seçin --</option>
              <option v-for="c in clientsList" :key="c.id" :value="c.id">{{ c.name }} ({{ c.companyName || 'Bireysel' }})</option>
            </select>
          </div>
          <div class="form-group">
            <label>Hizmet Adı *</label>
            <input v-model="form.name" type="text" class="cp-input" required placeholder="Örn: SEO Danışmanlık" />
          </div>
          <div class="form-group">
            <label>Açıklama</label>
            <textarea v-model="form.description" class="cp-input textarea" placeholder="Hizmet detayları..."></textarea>
          </div>
          <div class="form-grid">
            <div class="form-group">
              <label>Fiyat (₺) *</label>
              <input v-model.number="form.price" type="number" step="0.01" class="cp-input" required placeholder="5500" />
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

          <!-- Invoice & Installment Options -->
          <div class="installment-box">
            <div class="checkbox-row">
              <input type="checkbox" id="autoInvoice" v-model="form.autoCreateInvoice" />
              <label for="autoInvoice"><strong>Otomatik Fatura Oluşturulsun</strong></label>
            </div>

            <div v-if="form.autoCreateInvoice" class="installment-options animate-fade-in">
              <div class="form-grid">
                <div class="form-group">
                  <label>Ödeme Düzeni / Taksit</label>
                  <select v-model.number="form.installmentsCount" class="cp-input">
                    <option :value="1">Tek Çekim (1 Fatura)</option>
                    <option :value="2">2 Taksit (Aylık)</option>
                    <option :value="3">3 Taksit (Aylık)</option>
                    <option :value="6">6 Taksit (Aylık)</option>
                    <option :value="12">12 Taksit (Aylık)</option>
                  </select>
                </div>
                <div class="form-group">
                  <label>İlk Vade Tarihi</label>
                  <input v-model="form.firstDueDate" type="date" class="cp-input" />
                </div>
              </div>

              <div class="calc-preview">
                💡 <strong>Ödeme Özeti:</strong> ₺{{ form.price.toLocaleString('tr-TR') }} {{ form.installmentsCount > 1 ? `(${form.installmentsCount} Taksit)` : 'Tek Çekim' }}
                <span v-if="form.installmentsCount > 1" class="calc-sub">
                  ➔ Aylık <strong>₺{{ (form.price / form.installmentsCount).toFixed(2) }}</strong> × {{ form.installmentsCount }} Fatura
                </span>
              </div>
            </div>
          </div>

          <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" @click="closeModal">İptal</button>
            <button type="submit" class="btn-primary" :disabled="saving">
              {{ saving ? 'Kaydediliyor...' : 'Hizmet ve Faturayı Oluştur' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { serviceService, clientService, type ServiceItem, type Client } from '../services/api'

const services = ref<ServiceItem[]>([])
const clientsList = ref<Client[]>([])
const loading = ref(true)
const saving = ref(false)
const searchQuery = ref('')
const showModal = ref(false)
const errorMsg = ref('')

const form = ref({
  clientId: '',
  name: '',
  description: '',
  price: 5500,
  currency: 'TRY',
  autoCreateInvoice: true,
  installmentsCount: 1,
  firstDueDate: new Date(Date.now() + 14 * 86400000).toISOString().split('T')[0]
})

const filteredServices = computed(() => {
  if (!searchQuery.value.trim()) return services.value
  const q = searchQuery.value.toLowerCase()
  return services.value.filter(s =>
    s.name.toLowerCase().includes(q) ||
    (s.clientName && s.clientName.toLowerCase().includes(q))
  )
})

const totalPrice = computed(() => {
  return filteredServices.value.reduce((acc, s) => acc + (s.price || 0), 0)
})

const loadData = async () => {
  loading.value = true
  try {
    const [svc, cls] = await Promise.all([serviceService.getServices(), clientService.getClients()])
    services.value = svc
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
    description: '',
    price: 5500,
    currency: 'TRY',
    autoCreateInvoice: true,
    installmentsCount: 1,
    firstDueDate: new Date(Date.now() + 14 * 86400000).toISOString().split('T')[0]
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveService = async () => {
  errorMsg.value = ''
  saving.value = true
  try {
    await serviceService.createService(form.value)
    closeModal()
    await loadData()
  } catch (err: any) {
    errorMsg.value = err.message || 'Hizmet oluşturulamadı.'
  } finally {
    saving.value = false
  }
}

const deleteService = async (id: string) => {
  if (!confirm('Bu hizmeti silmek istediğinizden emin misiniz?')) return
  try {
    await serviceService.deleteService(id)
    await loadData()
  } catch (err) {
    alert('Hizmet silinemedi.')
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.services-view { display: flex; flex-direction: column; gap: 1.5rem; }
.view-header { display: flex; align-items: center; justify-content: space-between; }
.view-title { font-size: 1.75rem; font-weight: 800; }
.view-subtitle { color: var(--color-text-muted); font-size: 0.875rem; margin-top: 0.25rem; }

.filter-bar { display: flex; align-items: center; justify-content: space-between; padding: 1rem 1.25rem; gap: 1rem; }
.search-input-wrapper { position: relative; flex: 1; max-width: 420px; }
.search-icon { position: absolute; left: 1rem; top: 50%; transform: translateY(-50%); width: 18px; height: 18px; color: var(--color-text-muted); }
.cp-input { width: 100%; padding: 0.65rem 1rem 0.65rem 2.6rem; border-radius: 10px; background: rgba(15,23,42,0.6); border: 1px solid rgba(99,102,241,0.2); color: white; font-size: 0.875rem; transition: border-color 0.2s ease; }
.cp-input.textarea { padding-left: 1rem; min-height: 70px; resize: vertical; }
.cp-input:focus { outline: none; border-color: #6366f1; box-shadow: 0 0 12px rgba(99,102,241,0.3); }

.filter-stats { font-size: 0.85rem; color: var(--color-text-muted); }
.highlight-val { color: #10b981; font-weight: 800; font-size: 1.1rem; }

.table-panel { padding: 1.25rem; overflow-x: auto; }
.service-name-cell { display: flex; align-items: center; gap: 0.75rem; }
.service-icon-box { width: 34px; height: 34px; border-radius: 10px; background: rgba(139,92,246,0.15); border: 1px solid rgba(139,92,246,0.3); display: flex; align-items: center; justify-content: center; font-size: 1rem; color: #a855f7; flex-shrink: 0; }
.service-title { font-weight: 700; color: var(--color-text-primary); font-size: 0.9rem; }

.client-badge { font-size: 0.8rem; font-weight: 600; color: #818cf8; background: rgba(99,102,241,0.1); padding: 0.25rem 0.6rem; border-radius: 6px; display: inline-block; }
.desc-text { font-size: 0.8rem; color: var(--color-text-muted); max-width: 280px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.price-text { font-weight: 800; color: var(--color-text-primary); font-size: 0.95rem; }
.curr-sub { font-size: 0.75rem; color: var(--color-text-muted); font-weight: 500; }

.btn-icon { width: 32px; height: 32px; border-radius: 8px; border: none; background: rgba(255,255,255,0.05); color: var(--color-text-secondary); cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s ease; }
.btn-icon svg { width: 15px; height: 15px; }
.btn-danger:hover { background: rgba(239,68,68,0.2); color: #f87171; }

.flex-align-center { display: flex; align-items: center; gap: 0.5rem; }
.icon-sm { width: 18px; height: 18px; }

.loading-state, .empty-state { padding: 3rem; text-align: center; color: var(--color-text-muted); }
.empty-icon { font-size: 3rem; margin-bottom: 0.5rem; }
.spinner { width: 32px; height: 32px; border: 3px solid rgba(99,102,241,0.2); border-top-color: #6366f1; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 1rem; }

.modal-backdrop { position: fixed; inset: 0; background: rgba(5,8,22,0.75); backdrop-filter: blur(8px); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 1rem; }
.modal-card { width: 100%; max-width: 520px; padding: 1.75rem; border-radius: 16px; background: rgba(15,23,42,0.95); border: 1px solid rgba(99,102,241,0.3); }
.modal-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 1.25rem; }
.modal-header h3 { font-size: 1.25rem; font-weight: 700; color: white; }
.btn-close { background: none; border: none; color: var(--color-text-muted); font-size: 1.25rem; cursor: pointer; }

.modal-form { display: flex; flex-direction: column; gap: 1rem; }
.form-group { display: flex; flex-direction: column; gap: 0.35rem; }
.form-group label { font-size: 0.8rem; font-weight: 600; color: var(--color-text-secondary); }
.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0.75rem; }

.installment-box { background: rgba(99,102,241,0.08); border: 1px solid rgba(99,102,241,0.2); border-radius: 12px; padding: 1rem; display: flex; flex-direction: column; gap: 0.75rem; }
.checkbox-row { display: flex; align-items: center; gap: 0.5rem; font-size: 0.85rem; color: white; cursor: pointer; }
.checkbox-row input { accent-color: #6366f1; width: 16px; height: 16px; }

.installment-options { display: flex; flex-direction: column; gap: 0.75rem; margin-top: 0.25rem; }
.calc-preview { font-size: 0.8rem; color: #818cf8; background: rgba(15,23,42,0.6); padding: 0.6rem 0.8rem; border-radius: 8px; border: 1px solid rgba(99,102,241,0.15); }
.calc-sub { display: block; margin-top: 0.2rem; color: #34d399; }

.modal-actions { display: flex; align-items: center; justify-content: flex-end; gap: 0.75rem; margin-top: 0.5rem; }
.error-banner { background: rgba(239,68,68,0.15); color: #f87171; border: 1px solid rgba(239,68,68,0.3); padding: 0.6rem 0.8rem; border-radius: 8px; font-size: 0.8rem; }

@keyframes spin { to { transform: rotate(360deg); } }
</style>
