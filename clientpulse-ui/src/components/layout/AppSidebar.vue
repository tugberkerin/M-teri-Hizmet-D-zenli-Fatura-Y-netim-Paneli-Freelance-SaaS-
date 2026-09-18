<template>
  <div class="sidebar">
    <!-- Logo -->
    <div class="sidebar-logo">
      <div class="logo-icon">
        <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M12 2L2 7L12 12L22 7L12 2Z" stroke="url(#grad1)" stroke-width="2" stroke-linejoin="round"/>
          <path d="M2 17L12 22L22 17" stroke="url(#grad1)" stroke-width="2" stroke-linejoin="round"/>
          <path d="M2 12L12 17L22 12" stroke="url(#grad1)" stroke-width="2" stroke-linejoin="round"/>
          <defs>
            <linearGradient id="grad1" x1="2" y1="2" x2="22" y2="22" gradientUnits="userSpaceOnUse">
              <stop offset="0%" stop-color="#6366f1"/>
              <stop offset="100%" stop-color="#a78bfa"/>
            </linearGradient>
          </defs>
        </svg>
      </div>
      <div class="logo-text">
        <span class="logo-name gradient-text">ClientPulse</span>
        <span class="logo-sub">SaaS Panel</span>
      </div>
    </div>

    <!-- Navigation -->
    <nav class="sidebar-nav">
      <div class="nav-section-label">ANA MENÜ</div>
      <RouterLink to="/" class="nav-item" :class="{ active: route.path === '/' }">
        <span class="nav-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>
        </span>
        <span>Dashboard</span>
      </RouterLink>

      <RouterLink to="/clients" class="nav-item" :class="{ active: route.path.startsWith('/clients') }">
        <span class="nav-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
        </span>
        <span>Müşteriler</span>
        <span v-if="clientCount > 0" class="nav-badge">{{ clientCount }}</span>
      </RouterLink>

      <RouterLink to="/services" class="nav-item" :class="{ active: route.path.startsWith('/services') }">
        <span class="nav-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/></svg>
        </span>
        <span>Hizmetler</span>
      </RouterLink>

      <RouterLink to="/invoices" class="nav-item" :class="{ active: route.path.startsWith('/invoices') }">
        <span class="nav-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/><polyline points="10 9 9 9 8 9"/></svg>
        </span>
        <span>Faturalar</span>
        <span class="nav-badge badge-warning-dot"></span>
      </RouterLink>

      <RouterLink to="/uptime" class="nav-item" :class="{ active: route.path.startsWith('/uptime') }">
        <span class="nav-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>
        </span>
        <span>Uptime İzleme</span>
        <span class="live-dot"></span>
      </RouterLink>

      <div class="nav-section-label" style="margin-top: 1.5rem;">AYARLAR</div>
      <RouterLink to="/settings" class="nav-item" :class="{ active: route.path.startsWith('/settings') }">
        <span class="nav-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="3"/><path d="M19.07 4.93a10 10 0 0 1 0 14.14M4.93 4.93a10 10 0 0 0 0 14.14"/></svg>
        </span>
        <span>Ayarlar</span>
      </RouterLink>
    </nav>

    <!-- Bottom: User Profile -->
    <div class="sidebar-footer">
      <div class="user-profile">
        <div class="user-avatar">
          <span>A</span>
        </div>
        <div class="user-info">
          <span class="user-name">Admin</span>
          <span class="user-role">Freelancer</span>
        </div>
        <button class="user-menu-btn" title="Çıkış">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/></svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'

const route = useRoute()
const clientCount = 3 // placeholder
</script>

<style scoped>
.sidebar {
  width: 260px;
  min-height: 100vh;
  background: rgba(10, 15, 30, 0.95);
  border-right: 1px solid rgba(99, 120, 180, 0.12);
  display: flex;
  flex-direction: column;
  padding: 1.25rem 0;
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  z-index: 50;
}

.sidebar-logo {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  padding: 0 1.25rem 1.5rem;
  border-bottom: 1px solid rgba(99, 120, 180, 0.1);
  margin-bottom: 1.25rem;
}

.logo-icon {
  width: 38px;
  height: 38px;
  background: linear-gradient(135deg, rgba(99,102,241,0.15), rgba(139,92,246,0.15));
  border: 1px solid rgba(99,102,241,0.3);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.logo-icon svg { width: 20px; height: 20px; }

.logo-text { display: flex; flex-direction: column; }

.logo-name {
  font-size: 1rem;
  font-weight: 800;
  letter-spacing: -0.02em;
  line-height: 1.2;
}

.logo-sub {
  font-size: 0.7rem;
  color: var(--color-text-muted);
  font-weight: 500;
  letter-spacing: 0.05em;
}

.sidebar-nav {
  flex: 1;
  padding: 0 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
  overflow-y: auto;
}

.nav-section-label {
  font-size: 0.65rem;
  font-weight: 700;
  letter-spacing: 0.1em;
  color: var(--color-text-muted);
  padding: 0.25rem 0.5rem;
  margin: 0.25rem 0 0.375rem;
}

.nav-icon {
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.nav-icon svg { width: 17px; height: 17px; }

.nav-badge {
  margin-left: auto;
  background: rgba(99, 102, 241, 0.2);
  color: #818cf8;
  font-size: 0.7rem;
  font-weight: 700;
  padding: 0.1rem 0.45rem;
  border-radius: 999px;
  border: 1px solid rgba(99, 102, 241, 0.3);
}

.badge-warning-dot {
  margin-left: auto;
  width: 8px;
  height: 8px;
  background: #f59e0b;
  border-radius: 50%;
  box-shadow: 0 0 8px rgba(245, 158, 11, 0.5);
}

.live-dot {
  margin-left: auto;
  width: 8px;
  height: 8px;
  background: #10b981;
  border-radius: 50%;
  box-shadow: 0 0 8px rgba(16, 185, 129, 0.5);
  animation: pulse-live 2s ease-in-out infinite;
}

@keyframes pulse-live {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.6; transform: scale(0.8); }
}

.sidebar-footer {
  padding: 1rem 0.75rem 0.25rem;
  border-top: 1px solid rgba(99, 120, 180, 0.1);
  margin-top: 0.5rem;
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.625rem 0.75rem;
  border-radius: 10px;
  transition: background 0.2s ease;
}

.user-profile:hover { background: rgba(99, 102, 241, 0.08); }

.user-avatar {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 0.875rem;
  color: white;
  flex-shrink: 0;
}

.user-info { display: flex; flex-direction: column; flex: 1; min-width: 0; }

.user-name {
  font-size: 0.825rem;
  font-weight: 600;
  color: var(--color-text-primary);
}

.user-role {
  font-size: 0.7rem;
  color: var(--color-text-muted);
}

.user-menu-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: var(--color-text-muted);
  padding: 0.25rem;
  border-radius: 6px;
  transition: color 0.2s ease, background 0.2s ease;
  display: flex;
  align-items: center;
}

.user-menu-btn:hover { color: var(--color-text-primary); background: rgba(99,102,241,0.1); }
.user-menu-btn svg { width: 16px; height: 16px; }
</style>
