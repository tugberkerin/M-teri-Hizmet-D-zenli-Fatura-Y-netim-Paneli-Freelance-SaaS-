import { createRouter, createWebHistory } from 'vue-router'
import DashboardLayout from '../layouts/DashboardLayout.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: DashboardLayout,
      children: [
        {
          path: '',
          name: 'dashboard',
          component: () => import('../views/DashboardView.vue'),
          meta: { title: 'Dashboard', subtitle: 'Genel özet ve hızlı bakış' },
        },
        {
          path: 'clients',
          name: 'clients',
          component: () => import('../views/ClientsView.vue'),
          meta: { title: 'Müşteriler', subtitle: 'Müşteri listesi ve yönetimi' },
        },
        {
          path: 'clients/new',
          name: 'clients-new',
          component: () => import('../views/ClientsView.vue'),
          meta: { title: 'Yeni Müşteri', subtitle: 'Müşteri ekle' },
        },
        {
          path: 'services',
          name: 'services',
          component: () => import('../views/ServicesView.vue'),
          meta: { title: 'Hizmetler', subtitle: 'Sunulan hizmetler ve yönetimi' },
        },
        {
          path: 'invoices',
          name: 'invoices',
          component: () => import('../views/InvoicesView.vue'),
          meta: { title: 'Faturalar', subtitle: 'Fatura takibi ve ödeme durumu' },
        },
        {
          path: 'uptime',
          name: 'uptime',
          component: () => import('../views/UptimeView.vue'),
          meta: { title: 'Uptime İzleme', subtitle: 'Site ve sunucu sağlık durumu' },
        },
        {
          path: 'settings',
          name: 'settings',
          component: () => import('../views/SettingsView.vue'),
          meta: { title: 'Ayarlar', subtitle: 'Panel ve hesap ayarları' },
        },
      ],
    },
  ],
})

export default router
