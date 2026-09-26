<template>
  <div class="settings-view animate-fade-in">
    <div class="view-header">
      <div>
        <h1 class="view-title gradient-text">Sistem Ayarları</h1>
        <p class="view-subtitle">SaaS platform parametrelerini, API bağlantısını ve bildirim ayarlarını yapılandırın.</p>
      </div>
    </div>

    <div class="settings-grid">
      <!-- Card 1: API Configuration -->
      <div class="glass-card panel">
        <h3 class="panel-title">🌐 WebAPI Bağlantısı</h3>
        <p class="panel-sub">ClientPulse .NET Minimal API uç nokta adresi</p>
        
        <div class="form-group" style="margin-top:1rem;">
          <label>API Base URL</label>
          <input v-model="apiUrl" type="text" class="cp-input" />
        </div>
        
        <div class="status-indicator">
          <span class="status-dot online"></span>
          <span>Backend Durumu: <strong>Aktif (Bağlı)</strong></span>
        </div>
        
        <button class="btn-primary" style="margin-top:1rem;" @click="saveSettings()">Ayarları Kaydet</button>
      </div>

      <!-- Card 2: Database Mode -->
      <div class="glass-card panel">
        <h3 class="panel-title">🗄️ Veritabanı Yapılandırması</h3>
        <p class="panel-sub">Aktif veritabanı sürücüsü ve EF Core optimizasyonları</p>
        
        <div class="db-badge-box">
          <div class="db-badge">PostgreSQL & EF Core 9</div>
          <span class="db-desc">Optimistic Concurrency Control (xmin row versioning) aktif</span>
        </div>

        <div class="info-list">
          <div class="info-row">
            <span>Hybrid Cache / Memory:</span>
            <strong>Etkin</strong>
          </div>
          <div class="info-row">
            <span>SaaS Tenant İzolasyonu:</span>
            <strong>Aktif</strong>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const apiUrl = ref('http://localhost:5000/api')

const saveSettings = () => {
  alert('API ayarları kaydedildi!')
}
</script>

<style scoped>
.settings-view { display: flex; flex-direction: column; gap: 1.5rem; }
.view-header { margin-bottom: 0.5rem; }
.view-title { font-size: 1.75rem; font-weight: 800; }
.view-subtitle { color: var(--color-text-muted); font-size: 0.875rem; margin-top: 0.25rem; }

.settings-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1.5rem; }
.panel { padding: 1.5rem; display: flex; flex-direction: column; gap: 0.5rem; }
.panel-title { font-size: 1.1rem; font-weight: 700; color: white; }
.panel-sub { font-size: 0.8rem; color: var(--color-text-muted); }

.form-group { display: flex; flex-direction: column; gap: 0.4rem; }
.form-group label { font-size: 0.8rem; font-weight: 600; color: var(--color-text-secondary); }
.cp-input { padding: 0.65rem 1rem; border-radius: 10px; background: rgba(15,23,42,0.6); border: 1px solid rgba(99,102,241,0.2); color: white; font-size: 0.875rem; }

.status-indicator { display: flex; align-items: center; gap: 0.5rem; font-size: 0.85rem; color: var(--color-text-muted); margin-top: 0.75rem; }
.status-dot { width: 8px; height: 8px; border-radius: 50%; }
.status-dot.online { background: #10b981; box-shadow: 0 0 8px #10b981; }

.db-badge-box { margin-top: 1rem; padding: 1rem; background: rgba(99,102,241,0.08); border: 1px solid rgba(99,102,241,0.2); border-radius: 10px; }
.db-badge { font-weight: 800; color: #818cf8; font-size: 0.95rem; }
.db-desc { font-size: 0.75rem; color: var(--color-text-muted); margin-top: 0.25rem; display: block; }

.info-list { margin-top: 1rem; display: flex; flex-direction: column; gap: 0.5rem; font-size: 0.85rem; }
.info-row { display: flex; justify-content: space-between; color: var(--color-text-muted); border-bottom: 1px solid rgba(255,255,255,0.05); padding-bottom: 0.4rem; }
.info-row strong { color: white; }
</style>
