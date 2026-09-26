<template>
  <div class="dashboard-view animate-fade-in">
    <!-- Stat Cards -->
    <div class="stats-grid">
      <div v-for="(stat, i) in stats" :key="stat.label"
           class="glass-card stat-card animate-fade-in-up"
           :class="[`stat-glow-${stat.color}`, `delay-${i * 100}`]">
        <div class="stat-header">
          <div class="stat-icon" :style="{ background: stat.iconBg }">
            <span>{{ stat.icon }}</span>
          </div>
          <span class="stat-trend" :class="stat.trend >= 0 ? 'trend-up' : 'trend-down'">
            {{ stat.trend >= 0 ? '↑' : '↓' }} {{ Math.abs(stat.trend) }}%
          </span>
        </div>
        <div class="stat-body">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-label">{{ stat.label }}</div>
        </div>
        <div class="stat-bar">
          <div class="stat-bar-fill" :style="{ width: stat.progress + '%', background: stat.barColor }"></div>
        </div>
      </div>
    </div>

    <!-- Main Grid -->
    <div class="main-grid">
      <!-- Recent Invoices -->
      <div class="glass-card panel animate-fade-in-up delay-200">
        <div class="panel-header">
          <div>
            <h2 class="panel-title">Son Faturalar</h2>
            <p class="panel-subtitle">Yaklaşan ve gecikmiş ödemeler</p>
          </div>
          <RouterLink to="/invoices" class="btn-secondary" style="font-size:0.8rem;padding:0.45rem 1rem;">
            Tümünü Gör →
          </RouterLink>
        </div>
        
        <table class="cp-table">
          <thead>
            <tr>
              <th>Müşteri</th>
              <th>Tutar</th>
              <th>Vade</th>
              <th>Durum</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="inv in recentInvoices" :key="inv.id">
              <td>
                <div class="client-cell">
                  <div class="client-avatar">{{ (inv.clientName || 'M')[0] }}</div>
                  <div>
                    <div class="client-name">{{ inv.clientName || 'Müşteri' }}</div>
                    <div class="client-sub">{{ inv.notes || 'Fatura' }}</div>
                  </div>
                </div>
              </td>
              <td class="amount-cell">₺{{ inv.amount.toLocaleString('tr-TR') }}</td>
              <td style="color:var(--color-text-muted)">{{ formatDate(inv.dueDate) }}</td>
              <td><span class="badge" :class="statusBadge(inv.status)">{{ formatStatus(inv.status) }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Right Column -->
      <div class="right-column">
        <!-- Uptime Card -->
        <div class="glass-card panel animate-fade-in-up delay-300">
          <div class="panel-header">
            <div>
              <h2 class="panel-title">Uptime Özeti</h2>
              <p class="panel-subtitle">Canlı servis durumları</p>
            </div>
            <span class="live-badge">● CANLI</span>
          </div>
          <div class="uptime-list">
            <div v-for="site in uptimeSites" :key="site.id" class="uptime-item">
              <div class="uptime-info">
                <span class="uptime-dot" :class="site.status === 'DOWN' ? 'dot-down' : 'dot-up'"></span>
                <div>
                  <div class="uptime-name">{{ site.name }}</div>
                  <div class="uptime-url">{{ site.url }}</div>
                </div>
              </div>
              <div class="uptime-right">
                <span class="uptime-rate">{{ site.status === 'DOWN' ? '0%' : '99.9%' }}</span>
                <span class="uptime-ms">{{ site.responseTimeMs || 45 }}ms</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Add -->
        <div class="glass-card panel animate-fade-in-up delay-400">
          <h2 class="panel-title" style="margin-bottom:1rem;">Hızlı İşlemler</h2>
          <div class="quick-actions">
            <RouterLink to="/clients" class="quick-action-btn">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="8.5" cy="7" r="4"/><line x1="20" y1="8" x2="20" y2="14"/><line x1="23" y1="11" x2="17" y2="11"/></svg>
              Yeni Müşteri Ekle
            </RouterLink>
            <RouterLink to="/invoices" class="quick-action-btn">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
              Fatura Kes
            </RouterLink>
            <RouterLink to="/uptime" class="quick-action-btn">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>
              Uptime Takibi Ekle
            </RouterLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { clientService, serviceService, invoiceService, uptimeService, type Invoice, type UptimeMonitorItem } from '../services/api'

const stats = ref([
  { label: 'Toplam Müşteri', value: '0', trend: 12, progress: 60, color: 'indigo', iconBg: 'rgba(99,102,241,0.15)', barColor: '#6366f1', icon: '👥' },
  { label: 'Aktif Hizmetler', value: '0', trend: 8, progress: 78, color: 'purple', iconBg: 'rgba(139,92,246,0.15)', barColor: '#8b5cf6', icon: '⚡' },
  { label: 'Aylık Gelir', value: '₺0', trend: 18, progress: 85, color: 'emerald', iconBg: 'rgba(16,185,129,0.15)', barColor: '#10b981', icon: '💰' },
  { label: 'Gecikmiş Fatura', value: '0', trend: -25, progress: 20, color: 'amber', iconBg: 'rgba(245,158,11,0.15)', barColor: '#f59e0b', icon: '⚠️' },
])

const recentInvoices = ref<Invoice[]>([])
const uptimeSites = ref<UptimeMonitorItem[]>([])

const getStatusKey = (s: string | number): string => {
  if (typeof s === 'number') return ['Pending', 'Paid', 'Overdue', 'Cancelled'][s] || 'Pending'
  return s
}

const loadDashboard = async () => {
  try {
    const [clients, services, invoices, monitors] = await Promise.all([
      clientService.getClients(),
      serviceService.getServices(),
      invoiceService.getInvoices(),
      uptimeService.getMonitors()
    ])

    const totalRev = services.reduce((acc, s) => acc + s.price, 0)
    const overdueCount = invoices.filter(i => getStatusKey(i.status) === 'Overdue').length

    stats.value[0].value = clients.length.toString()
    stats.value[1].value = services.length.toString()
    stats.value[2].value = `₺${totalRev.toLocaleString('tr-TR')}`
    stats.value[3].value = overdueCount.toString()

    recentInvoices.value = invoices.slice(0, 5)
    uptimeSites.value = monitors.slice(0, 4)
  } catch (err) {
    console.error(err)
  }
}

const formatStatus = (s: string | number) => {
  const k = getStatusKey(s)
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

const formatDate = (d?: string) => {
  if (!d) return '-'
  return new Date(d).toLocaleDateString('tr-TR', { day: '2-digit', month: 'short' })
}

onMounted(() => {
  loadDashboard()
})
</script>

<style scoped>
.dashboard-view { display: flex; flex-direction: column; gap: 1.5rem; }

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.stat-card { padding: 1.25rem 1.25rem 1rem; }

.stat-header { display: flex; align-items: flex-start; justify-content: space-between; margin-bottom: 1rem; }

.stat-icon {
  width: 42px;
  height: 42px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
}

.stat-trend {
  font-size: 0.75rem;
  font-weight: 700;
  padding: 0.2rem 0.5rem;
  border-radius: 6px;
}

.trend-up { color: #10b981; background: rgba(16,185,129,0.1); }
.trend-down { color: #ef4444; background: rgba(239,68,68,0.1); }

.stat-value { font-size: 1.75rem; font-weight: 800; letter-spacing: -0.03em; line-height: 1.1; color: white; }
.stat-label { font-size: 0.8rem; color: var(--color-text-muted); margin-top: 0.25rem; font-weight: 500; }

.stat-bar { height: 3px; background: rgba(255,255,255,0.06); border-radius: 999px; margin-top: 1rem; overflow: hidden; }
.stat-bar-fill { height: 100%; border-radius: 999px; transition: width 0.8s ease; }

.main-grid {
  display: grid;
  grid-template-columns: 1fr 360px;
  gap: 1rem;
  align-items: start;
}

.panel { padding: 1.5rem; }

.panel-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 1.25rem;
}

.panel-title { font-size: 1rem; font-weight: 700; color: var(--color-text-primary); }
.panel-subtitle { font-size: 0.75rem; color: var(--color-text-muted); margin-top: 0.125rem; }

.client-cell { display: flex; align-items: center; gap: 0.75rem; }

.client-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
}

.client-name { font-size: 0.85rem; font-weight: 600; color: var(--color-text-primary); }
.client-sub  { font-size: 0.72rem; color: var(--color-text-muted); }

.amount-cell { font-weight: 700; color: var(--color-text-primary) !important; font-size: 0.9rem !important; }

.right-column { display: flex; flex-direction: column; gap: 1rem; }

.live-badge {
  font-size: 0.65rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  color: #10b981;
  background: rgba(16,185,129,0.1);
  border: 1px solid rgba(16,185,129,0.2);
  padding: 0.25rem 0.6rem;
  border-radius: 999px;
}

.uptime-list { display: flex; flex-direction: column; gap: 0.875rem; }

.uptime-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem;
  background: rgba(15,23,42,0.5);
  border-radius: 10px;
  border: 1px solid rgba(99,120,180,0.08);
  transition: background 0.2s ease;
}

.uptime-item:hover { background: rgba(99,102,241,0.05); }

.uptime-info { display: flex; align-items: center; gap: 0.75rem; }

.uptime-dot { width: 9px; height: 9px; border-radius: 50%; flex-shrink: 0; }
.dot-up { background: #10b981; box-shadow: 0 0 8px rgba(16,185,129,0.5); }
.dot-down { background: #ef4444; box-shadow: 0 0 8px rgba(239,68,68,0.5); }

.uptime-name { font-size: 0.825rem; font-weight: 600; color: var(--color-text-primary); }
.uptime-url  { font-size: 0.7rem; color: var(--color-text-muted); }

.uptime-right { display: flex; flex-direction: column; align-items: flex-end; }
.uptime-rate { font-size: 0.875rem; font-weight: 700; color: var(--color-text-primary); }
.uptime-ms   { font-size: 0.7rem; color: var(--color-text-muted); }

.quick-actions { display: flex; flex-direction: column; gap: 0.625rem; }

.quick-action-btn {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  padding: 0.75rem 1rem;
  border-radius: 10px;
  background: rgba(99,102,241,0.06);
  border: 1px solid rgba(99,102,241,0.15);
  color: var(--color-text-secondary);
  font-size: 0.85rem;
  font-weight: 500;
  text-decoration: none;
  transition: all 0.2s ease;
}

.quick-action-btn:hover {
  background: rgba(99,102,241,0.12);
  border-color: rgba(99,102,241,0.3);
  color: #818cf8;
  transform: translateX(3px);
}

.quick-action-btn svg { width: 16px; height: 16px; flex-shrink: 0; }
</style>
