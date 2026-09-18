<template>
  <header class="topbar">
    <div class="topbar-left">
      <div class="page-info">
        <h1 class="page-title">{{ title }}</h1>
        <p class="page-subtitle">{{ subtitle }}</p>
      </div>
    </div>
    <div class="topbar-right">
      <!-- Search -->
      <div class="search-box">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
        <input type="text" placeholder="Ara..." class="search-input" />
      </div>

      <!-- Notification Bell -->
      <button class="icon-btn" title="Bildirimler">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/></svg>
        <span class="notif-dot"></span>
      </button>

      <!-- Refresh -->
      <button class="icon-btn" title="Yenile">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="23 4 23 10 17 10"/><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/></svg>
      </button>

      <!-- Date -->
      <div class="topbar-date">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
        <span>{{ currentDate }}</span>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from 'vue'

defineProps<{
  title: string
  subtitle?: string
}>()

const currentDate = computed(() => {
  return new Intl.DateTimeFormat('tr-TR', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  }).format(new Date())
})
</script>

<style scoped>
.topbar {
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 1.5rem;
  background: rgba(10, 15, 30, 0.8);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid rgba(99, 120, 180, 0.1);
  position: sticky;
  top: 0;
  z-index: 40;
}

.topbar-left { display: flex; align-items: center; gap: 1rem; }

.page-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: var(--color-text-primary);
  line-height: 1.2;
}

.page-subtitle {
  font-size: 0.75rem;
  color: var(--color-text-muted);
}

.topbar-right {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.search-box {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(15, 23, 42, 0.8);
  border: 1px solid rgba(99, 120, 180, 0.15);
  border-radius: 10px;
  padding: 0.5rem 0.875rem;
  transition: border-color 0.2s ease;
}

.search-box:focus-within { border-color: rgba(99, 102, 241, 0.5); }
.search-box svg { width: 15px; height: 15px; color: var(--color-text-muted); flex-shrink: 0; }

.search-input {
  background: none;
  border: none;
  outline: none;
  color: var(--color-text-primary);
  font-size: 0.825rem;
  font-family: var(--font-family-sans);
  width: 160px;
}

.search-input::placeholder { color: var(--color-text-muted); }

.icon-btn {
  width: 36px;
  height: 36px;
  background: rgba(15, 23, 42, 0.8);
  border: 1px solid rgba(99, 120, 180, 0.15);
  border-radius: 9px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--color-text-muted);
  transition: all 0.2s ease;
  position: relative;
}

.icon-btn:hover {
  border-color: rgba(99, 102, 241, 0.4);
  color: var(--color-text-primary);
  background: rgba(99, 102, 241, 0.08);
}

.icon-btn svg { width: 16px; height: 16px; }

.notif-dot {
  position: absolute;
  top: 6px;
  right: 6px;
  width: 7px;
  height: 7px;
  background: #6366f1;
  border: 1.5px solid var(--color-bg-primary);
  border-radius: 50%;
}

.topbar-date {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.875rem;
  background: rgba(99, 102, 241, 0.06);
  border: 1px solid rgba(99, 102, 241, 0.15);
  border-radius: 10px;
  color: var(--color-text-secondary);
  font-size: 0.8rem;
  font-weight: 500;
}

.topbar-date svg { width: 14px; height: 14px; color: #6366f1; }
</style>
