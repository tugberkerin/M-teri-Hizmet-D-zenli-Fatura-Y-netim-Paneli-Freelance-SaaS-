const API_BASE_URL = 'http://localhost:5000/api';

export interface Client {
  id: string;
  name: string;
  email: string;
  phone?: string;
  companyName?: string;
  servicesCount?: number;
  invoicesCount?: number;
  createdAt?: string;
}

export interface ServiceItem {
  id: string;
  clientId: string;
  clientName?: string;
  name: string;
  description?: string;
  price: number;
  currency: string;
  createdAt?: string;
}

export interface Invoice {
  id: string;
  clientId: string;
  clientName?: string;
  amount: number;
  currency: string;
  issueDate: string;
  dueDate: string;
  status: 'Pending' | 'Paid' | 'Overdue' | 'Cancelled' | number;
  notes?: string;
}

export interface UptimeMonitorItem {
  id: string;
  clientId: string;
  clientName?: string;
  name: string;
  url: string;
  checkIntervalMinutes: number;
  isActive: boolean;
  status?: string;
  responseTimeMs?: number;
  createdAt?: string;
}

// LocalStorage helpers
function loadLocal<T>(key: string, initial: T): T {
  try {
    const item = localStorage.getItem(`cp_${key}`);
    return item ? JSON.parse(item) : initial;
  } catch {
    return initial;
  }
}

function saveLocal<T>(key: string, data: T): void {
  try {
    localStorage.setItem(`cp_${key}`, JSON.stringify(data));
  } catch {}
}

const defaultClients: Client[] = [
  { id: '1', name: 'Ahmet Yılmaz', email: 'ahmet@yilmaz.com', phone: '+90 532 111 2233', companyName: 'Yılmaz Yazılım Ltd.', servicesCount: 2, invoicesCount: 3, createdAt: new Date().toISOString() },
  { id: '2', name: 'Selin Demir', email: 'selin@demir.io', phone: '+90 533 444 5566', companyName: 'Demir Medya', servicesCount: 1, invoicesCount: 1, createdAt: new Date().toISOString() },
  { id: '3', name: 'Murat Kaya', email: 'murat@kaya.net', phone: '+90 535 777 8899', companyName: 'Kaya Danışmanlık', servicesCount: 3, invoicesCount: 2, createdAt: new Date().toISOString() },
  { id: '4', name: 'Ece Şahin', email: 'ece@sahin.co', phone: '+90 536 999 0011', companyName: 'Şahin E-Ticaret', servicesCount: 1, invoicesCount: 4, createdAt: new Date().toISOString() }
];

const defaultServices: ServiceItem[] = [
  { id: '101', clientId: '1', clientName: 'Ahmet Yılmaz', name: 'Web Geliştirme', description: 'Kurumsal web sitesi tasarımı ve kodlaması', price: 8500, currency: 'TRY' },
  { id: '102', clientId: '2', clientName: 'Selin Demir', name: 'SEO Danışmanlık', description: 'Aylık arama motoru optimizasyonu desteği', price: 3200, currency: 'TRY' },
  { id: '103', clientId: '3', clientName: 'Murat Kaya', name: 'UI/UX Tasarım', description: 'Mobil uygulama arayüz ve deneyim tasarımı', price: 6100, currency: 'TRY' },
  { id: '104', clientId: '4', clientName: 'Ece Şahin', name: 'Mobil Uygulama', description: 'iOS & Android Cross-platform uygulama', price: 12000, currency: 'TRY' }
];

const defaultInvoices: Invoice[] = [
  { id: '201', clientId: '1', clientName: 'Ahmet Yılmaz', amount: 8500, currency: 'TRY', issueDate: '2026-09-10', dueDate: '2026-09-20', status: 'Pending', notes: 'Web sitesi teslimatı faturası' },
  { id: '202', clientId: '2', clientName: 'Selin Demir', amount: 3200, currency: 'TRY', issueDate: '2026-09-01', dueDate: '2026-09-18', status: 'Overdue', notes: 'Eylül ayı SEO danışmanlığı' },
  { id: '203', clientId: '3', clientName: 'Murat Kaya', amount: 6100, currency: 'TRY', issueDate: '2026-09-05', dueDate: '2026-09-25', status: 'Paid', notes: 'Figma tasarım teslimi' },
  { id: '204', clientId: '4', clientName: 'Ece Şahin', amount: 12000, currency: 'TRY', issueDate: '2026-09-15', dueDate: '2026-09-30', status: 'Pending', notes: 'Mobil uygulama 1. taksit' }
];

const defaultMonitors: UptimeMonitorItem[] = [
  { id: '301', clientId: '1', clientName: 'Ahmet Yılmaz', name: 'Ana Website', url: 'https://ahmetyilmaz.com', checkIntervalMinutes: 5, isActive: true, status: 'UP', responseTimeMs: 124 },
  { id: '302', clientId: '2', clientName: 'Selin Demir', name: 'API Sunucu', url: 'https://api.selindemir.io', checkIntervalMinutes: 3, isActive: true, status: 'UP', responseTimeMs: 45 },
  { id: '303', clientId: '3', clientName: 'Murat Kaya', name: 'Müşteri Paneli', url: 'https://panel.muratkaya.net', checkIntervalMinutes: 5, isActive: true, status: 'DOWN', responseTimeMs: 0 },
  { id: '304', clientId: '4', clientName: 'Ece Şahin', name: 'E-Ticaret Mağazası', url: 'https://shop.ecesahin.co', checkIntervalMinutes: 10, isActive: true, status: 'UP', responseTimeMs: 210 }
];

let mockClients: Client[] = loadLocal('clients', defaultClients);
let mockServices: ServiceItem[] = loadLocal('services', defaultServices);
let mockInvoices: Invoice[] = loadLocal('invoices', defaultInvoices);
let mockMonitors: UptimeMonitorItem[] = loadLocal('monitors', defaultMonitors);

async function apiFetch<T>(endpoint: string, options?: RequestInit, fallbackData?: T): Promise<T> {
  try {
    const res = await fetch(`${API_BASE_URL}${endpoint}`, {
      headers: { 'Content-Type': 'application/json', ...options?.headers },
      ...options
    });
    if (!res.ok) throw new Error(`HTTP error ${res.status}`);
    return await res.json();
  } catch (err) {
    if (fallbackData !== undefined) return fallbackData;
    throw err;
  }
}

export const clientService = {
  async getClients(): Promise<Client[]> {
    return apiFetch<Client[]>('/clients', undefined, mockClients);
  },
  async getClientById(id: string): Promise<any> {
    const client = mockClients.find(c => c.id === id);
    const services = mockServices.filter(s => s.clientId === id);
    const invoices = mockInvoices.filter(i => i.clientId === id);
    const monitors = mockMonitors.filter(m => m.clientId === id);
    const fallback = client ? { ...client, services, invoices, uptimeMonitors: monitors } : null;
    return apiFetch<any>(`/clients/${id}`, undefined, fallback);
  },
  async createClient(data: Partial<Client>): Promise<string> {
    const newId = String(Date.now());
    const newClient: Client = {
      id: newId,
      name: data.name || '',
      email: data.email || '',
      phone: data.phone,
      companyName: data.companyName,
      servicesCount: 0,
      invoicesCount: 0,
      createdAt: new Date().toISOString()
    };
    mockClients.unshift(newClient);
    saveLocal('clients', mockClients);
    return apiFetch<string>('/clients', { method: 'POST', body: JSON.stringify(data) }, newId);
  },
  async updateClient(id: string, data: Partial<Client>): Promise<boolean> {
    const idx = mockClients.findIndex(c => c.id === id);
    if (idx !== -1) {
      mockClients[idx] = { ...mockClients[idx], ...data };
      saveLocal('clients', mockClients);
    }
    return apiFetch<boolean>(`/clients/${id}`, { method: 'PUT', body: JSON.stringify({ id, ...data }) }, true);
  },
  async deleteClient(id: string): Promise<boolean> {
    mockClients = mockClients.filter(c => c.id !== id);
    saveLocal('clients', mockClients);
    return apiFetch<boolean>(`/clients/${id}`, { method: 'DELETE' }, true);
  }
};

export const serviceService = {
  async getServices(): Promise<ServiceItem[]> {
    return apiFetch<ServiceItem[]>('/services', undefined, mockServices);
  },
  async createService(data: any): Promise<string> {
    const client = mockClients.find(c => c.id === data.clientId);
    const newId = String(Date.now());
    const newService: ServiceItem = {
      id: newId,
      clientId: data.clientId || '',
      clientName: client ? client.name : 'Müşteri',
      name: data.name || '',
      description: data.description,
      price: data.price || 0,
      currency: data.currency || 'TRY',
      createdAt: new Date().toISOString()
    };
    mockServices.unshift(newService);
    saveLocal('services', mockServices);

    if (data.autoCreateInvoice !== false && data.price > 0) {
      const count = data.installmentsCount && data.installmentsCount > 0 ? data.installmentsCount : 1;
      const splitAmount = Math.round((data.price / count) * 100) / 100;
      const baseDate = data.firstDueDate ? new Date(data.firstDueDate) : new Date(Date.now() + 14 * 86400000);

      for (let i = 1; i <= count; i++) {
        const d = new Date(baseDate);
        d.setMonth(d.getMonth() + (i - 1));
        const note = count === 1 ? `${data.name} - Hizmet Faturası` : `${data.name} (Taksit ${i}/${count})`;
        mockInvoices.unshift({
          id: String(Date.now() + i),
          clientId: data.clientId,
          clientName: client ? client.name : 'Müşteri',
          amount: splitAmount,
          currency: data.currency || 'TRY',
          issueDate: new Date().toISOString().split('T')[0],
          dueDate: d.toISOString().split('T')[0],
          status: 'Pending',
          notes: note
        });
      }
      saveLocal('invoices', mockInvoices);
    }

    return apiFetch<string>('/services', { method: 'POST', body: JSON.stringify(data) }, newId);
  },
  async deleteService(id: string): Promise<boolean> {
    mockServices = mockServices.filter(s => s.id !== id);
    saveLocal('services', mockServices);
    return apiFetch<boolean>(`/services/${id}`, { method: 'DELETE' }, true);
  }
};

export const invoiceService = {
  async getInvoices(): Promise<Invoice[]> {
    return apiFetch<Invoice[]>('/invoices', undefined, mockInvoices);
  },
  async createInvoice(data: any): Promise<string> {
    const client = mockClients.find(c => c.id === data.clientId);
    const count = data.installmentsCount && data.installmentsCount > 0 ? data.installmentsCount : 1;
    const splitAmount = Math.round(((data.amount || 0) / count) * 100) / 100;
    const baseDate = data.dueDate ? new Date(data.dueDate) : new Date(Date.now() + 14 * 86400000);
    let firstId = String(Date.now());

    for (let i = 1; i <= count; i++) {
      const d = new Date(baseDate);
      d.setMonth(d.getMonth() + (i - 1));
      const invId = String(Date.now() + i);
      if (i === 1) firstId = invId;
      const note = count === 1
        ? (data.notes || 'Fatura')
        : (data.notes ? `${data.notes} (Taksit ${i}/${count})` : `Taksit ${i}/${count}`);

      mockInvoices.unshift({
        id: invId,
        clientId: data.clientId || '',
        clientName: client ? client.name : 'Müşteri',
        amount: splitAmount,
        currency: data.currency || 'TRY',
        issueDate: data.issueDate || new Date().toISOString().split('T')[0],
        dueDate: d.toISOString().split('T')[0],
        status: 'Pending',
        notes: note
      });
    }
    saveLocal('invoices', mockInvoices);

    return apiFetch<string>('/invoices', { method: 'POST', body: JSON.stringify(data) }, firstId);
  },
  async updateStatus(id: string, status: 'Pending' | 'Paid' | 'Overdue' | 'Cancelled'): Promise<boolean> {
    const idx = mockInvoices.findIndex(i => i.id === id);
    if (idx !== -1) {
      mockInvoices[idx].status = status;
      saveLocal('invoices', mockInvoices);
    }
    return apiFetch<boolean>(`/invoices/${id}/status`, { method: 'PUT', body: JSON.stringify({ status }) }, true);
  },
  async deleteInvoice(id: string): Promise<boolean> {
    mockInvoices = mockInvoices.filter(i => i.id !== id);
    saveLocal('invoices', mockInvoices);
    return apiFetch<boolean>(`/invoices/${id}`, { method: 'DELETE' }, true);
  }
};

export const uptimeService = {
  async getMonitors(): Promise<UptimeMonitorItem[]> {
    return apiFetch<UptimeMonitorItem[]>('/uptime', undefined, mockMonitors);
  },
  async createMonitor(data: Partial<UptimeMonitorItem>): Promise<string> {
    const client = mockClients.find(c => c.id === data.clientId);
    const newId = String(Date.now());
    const newMonitor: UptimeMonitorItem = {
      id: newId,
      clientId: data.clientId || '',
      clientName: client ? client.name : 'Müşteri',
      name: data.name || '',
      url: data.url || '',
      checkIntervalMinutes: data.checkIntervalMinutes || 5,
      isActive: true,
      status: 'UP',
      responseTimeMs: Math.floor(Math.random() * 150) + 20,
      createdAt: new Date().toISOString()
    };
    mockMonitors.unshift(newMonitor);
    saveLocal('monitors', mockMonitors);
    return apiFetch<string>('/uptime', { method: 'POST', body: JSON.stringify(data) }, newId);
  },
  async deleteMonitor(id: string): Promise<boolean> {
    mockMonitors = mockMonitors.filter(m => m.id !== id);
    saveLocal('monitors', mockMonitors);
    return apiFetch<boolean>(`/uptime/${id}`, { method: 'DELETE' }, true);
  }
};
