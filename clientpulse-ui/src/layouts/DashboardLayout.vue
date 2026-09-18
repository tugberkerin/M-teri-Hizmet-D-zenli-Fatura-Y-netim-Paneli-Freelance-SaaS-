<template>
  <div class="dashboard-layout">
    <AppSidebar />
    <div class="main-area">
      <AppTopbar :title="pageTitle" :subtitle="pageSubtitle" />
      <main class="content">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import AppSidebar from '../components/layout/AppSidebar.vue'
import AppTopbar from '../components/layout/AppTopbar.vue'

const route = useRoute()

const pageTitle = computed(() => (route.meta.title as string) ?? 'Dashboard')
const pageSubtitle = computed(() => (route.meta.subtitle as string) ?? '')
</script>

<style scoped>
.dashboard-layout {
  display: flex;
  min-height: 100vh;
  background: var(--color-bg-primary);
}

.main-area {
  margin-left: 260px;
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  min-width: 0;
}

.content {
  flex: 1;
  padding: 1.75rem;
  overflow-y: auto;
}

/* Subtle background grid pattern */
.dashboard-layout::before {
  content: '';
  position: fixed;
  inset: 0;
  background-image:
    radial-gradient(circle at 25% 25%, rgba(99, 102, 241, 0.05) 0%, transparent 50%),
    radial-gradient(circle at 75% 75%, rgba(139, 92, 246, 0.04) 0%, transparent 50%);
  pointer-events: none;
  z-index: 0;
}

.main-area, .content { position: relative; z-index: 1; }
</style>
