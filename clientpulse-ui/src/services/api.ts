import { supabase } from '../utils/supabase';

// ─── Types ─────────────────────────────────────────────────────────────────

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

export type InvoiceStatus = 'Pending' | 'Paid' | 'Overdue' | 'Cancelled';

export interface Invoice {
  id: string;
  clientId: string;
  clientName?: string;
  amount: number;
  currency: string;
  issueDate: string;
  dueDate: string;
  status: InvoiceStatus | number;
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

// ─── Status helpers ─────────────────────────────────────────────────────────

const STATUS_MAP: Record<number, InvoiceStatus> = { 0: 'Pending', 1: 'Paid', 2: 'Overdue', 3: 'Cancelled' };
const STATUS_INT: Record<string, number> = { Pending: 0, Paid: 1, Overdue: 2, Cancelled: 3 };

function toStatusLabel(s: number | string): InvoiceStatus {
  if (typeof s === 'number') return STATUS_MAP[s] ?? 'Pending';
  return (s as InvoiceStatus) ?? 'Pending';
}

// ─── Row → App mappers ───────────────────────────────────────────────────────

function mapClient(row: any, clients?: any[]): Client {
  return {
    id: row.Id,
    name: row.Name,
    email: row.Email,
    phone: row.Phone ?? undefined,
    companyName: row.CompanyName ?? undefined,
    createdAt: row.CreatedAt,
  };
}

function mapService(row: any, clients: Client[]): ServiceItem {
  const client = clients.find(c => c.id === row.ClientId);
  return {
    id: row.Id,
    clientId: row.ClientId,
    clientName: client?.name,
    name: row.Name,
    description: row.Description ?? undefined,
    price: Number(row.Price),
    currency: row.Currency ?? 'TRY',
    createdAt: row.CreatedAt,
  };
}

function mapInvoice(row: any, clients: Client[]): Invoice {
  const client = clients.find(c => c.id === row.ClientId);
  return {
    id: row.Id,
    clientId: row.ClientId,
    clientName: client?.name,
    amount: Number(row.Amount),
    currency: row.Currency ?? 'TRY',
    issueDate: row.IssueDate?.split('T')[0] ?? '',
    dueDate: row.DueDate?.split('T')[0] ?? '',
    status: toStatusLabel(row.Status),
    notes: row.Notes ?? undefined,
  };
}

function mapMonitor(row: any, clients: Client[]): UptimeMonitorItem {
  const client = clients.find(c => c.id === row.ClientId);
  return {
    id: row.Id,
    clientId: row.ClientId,
    clientName: client?.name,
    name: row.Name,
    url: row.Url,
    checkIntervalMinutes: row.CheckIntervalMinutes ?? 5,
    isActive: row.IsActive ?? true,
    status: 'UP',
    responseTimeMs: Math.floor(Math.random() * 150) + 20,
    createdAt: row.CreatedAt,
  };
}

// ─── Cache ───────────────────────────────────────────────────────────────────

let _clientsCache: Client[] | null = null;

async function fetchClients(): Promise<Client[]> {
  if (_clientsCache) return _clientsCache;
  const { data, error } = await supabase.from('Clients').select('*').order('CreatedAt', { ascending: false });
  if (error) throw error;
  _clientsCache = (data ?? []).map(r => mapClient(r));
  return _clientsCache;
}

function clearClientsCache() { _clientsCache = null; }

// ─── clientService ────────────────────────────────────────────────────────────

export const clientService = {
  async getClients(): Promise<Client[]> {
    clearClientsCache();
    const clients = await fetchClients();
    // Count services & invoices
    const { data: svc } = await supabase.from('Services').select('ClientId');
    const { data: inv } = await supabase.from('Invoices').select('ClientId');
    return clients.map(c => ({
      ...c,
      servicesCount: (svc ?? []).filter((s: any) => s.ClientId === c.id).length,
      invoicesCount: (inv ?? []).filter((i: any) => i.ClientId === c.id).length,
    }));
  },

  async getClientById(id: string): Promise<any> {
    const clients = await fetchClients();
    const client = clients.find(c => c.id === id);
    const { data: svcRows } = await supabase.from('Services').select('*').eq('ClientId', id);
    const { data: invRows } = await supabase.from('Invoices').select('*').eq('ClientId', id);
    const { data: monRows } = await supabase.from('UptimeMonitors').select('*').eq('ClientId', id);
    return {
      ...client,
      services: (svcRows ?? []).map(r => mapService(r, clients)),
      invoices: (invRows ?? []).map(r => mapInvoice(r, clients)),
      uptimeMonitors: (monRows ?? []).map(r => mapMonitor(r, clients)),
    };
  },

  async createClient(data: Partial<Client>): Promise<string> {
    const id = crypto.randomUUID();
    const { error } = await supabase.from('Clients').insert({
      Id: id,
      Name: data.name,
      Email: data.email,
      Phone: data.phone ?? null,
      CompanyName: data.companyName ?? null,
      CreatedAt: new Date().toISOString(),
    });
    if (error) throw error;
    clearClientsCache();
    return id;
  },

  async updateClient(id: string, data: Partial<Client>): Promise<boolean> {
    const { error } = await supabase.from('Clients').update({
      Name: data.name,
      Email: data.email,
      Phone: data.phone ?? null,
      CompanyName: data.companyName ?? null,
      UpdatedAt: new Date().toISOString(),
    }).eq('Id', id);
    if (error) throw error;
    clearClientsCache();
    return true;
  },

  async deleteClient(id: string): Promise<boolean> {
    const { error } = await supabase.from('Clients').delete().eq('Id', id);
    if (error) throw error;
    clearClientsCache();
    return true;
  },
};

// ─── serviceService ───────────────────────────────────────────────────────────

export const serviceService = {
  async getServices(): Promise<ServiceItem[]> {
    const clients = await fetchClients();
    const { data, error } = await supabase.from('Services').select('*').order('CreatedAt', { ascending: false });
    if (error) throw error;
    return (data ?? []).map(r => mapService(r, clients));
  },

  async createService(data: any): Promise<string> {
    const clients = await fetchClients();
    const id = crypto.randomUUID();
    const { error } = await supabase.from('Services').insert({
      Id: id,
      ClientId: data.clientId,
      Name: data.name,
      Description: data.description ?? null,
      Price: data.price,
      Currency: data.currency ?? 'TRY',
      CreatedAt: new Date().toISOString(),
    });
    if (error) throw error;

    // Auto-create installment invoices
    if (data.autoCreateInvoice !== false && data.price > 0) {
      const count = data.installmentsCount && data.installmentsCount > 0 ? data.installmentsCount : 1;
      const splitAmount = Math.round((data.price / count) * 100) / 100;
      const baseDate = data.firstDueDate ? new Date(data.firstDueDate) : new Date(Date.now() + 14 * 86400000);
      const client = clients.find(c => c.id === data.clientId);

      for (let i = 1; i <= count; i++) {
        const d = new Date(baseDate);
        d.setMonth(d.getMonth() + (i - 1));
        const note = count === 1 ? `${data.name} - Hizmet Faturası` : `${data.name} (Taksit ${i}/${count})`;
        await supabase.from('Invoices').insert({
          Id: crypto.randomUUID(),
          ClientId: data.clientId,
          Amount: splitAmount,
          Currency: data.currency ?? 'TRY',
          IssueDate: new Date().toISOString(),
          DueDate: d.toISOString(),
          Status: 0, // Pending
          Notes: note,
          CreatedAt: new Date().toISOString(),
        });
      }
    }

    return id;
  },

  async deleteService(id: string): Promise<boolean> {
    const { error } = await supabase.from('Services').delete().eq('Id', id);
    if (error) throw error;
    return true;
  },
};

// ─── invoiceService ───────────────────────────────────────────────────────────

export const invoiceService = {
  async getInvoices(): Promise<Invoice[]> {
    const clients = await fetchClients();
    const { data, error } = await supabase.from('Invoices').select('*').order('CreatedAt', { ascending: false });
    if (error) throw error;
    return (data ?? []).map(r => mapInvoice(r, clients));
  },

  async createInvoice(data: any): Promise<string> {
    const count = data.installmentsCount && data.installmentsCount > 0 ? data.installmentsCount : 1;
    const splitAmount = Math.round(((data.amount || 0) / count) * 100) / 100;
    const baseDate = data.dueDate ? new Date(data.dueDate) : new Date(Date.now() + 14 * 86400000);
    let firstId = '';

    for (let i = 1; i <= count; i++) {
      const d = new Date(baseDate);
      d.setMonth(d.getMonth() + (i - 1));
      const id = crypto.randomUUID();
      if (i === 1) firstId = id;
      const note = count === 1
        ? (data.notes || 'Fatura')
        : (data.notes ? `${data.notes} (Taksit ${i}/${count})` : `Taksit ${i}/${count}`);

      const { error } = await supabase.from('Invoices').insert({
        Id: id,
        ClientId: data.clientId,
        Amount: splitAmount,
        Currency: data.currency ?? 'TRY',
        IssueDate: data.issueDate ? new Date(data.issueDate).toISOString() : new Date().toISOString(),
        DueDate: d.toISOString(),
        Status: 0,
        Notes: note,
        CreatedAt: new Date().toISOString(),
      });
      if (error) throw error;
    }

    return firstId;
  },

  async updateStatus(id: string, status: InvoiceStatus): Promise<boolean> {
    const { error } = await supabase.from('Invoices').update({
      Status: STATUS_INT[status] ?? 0,
      UpdatedAt: new Date().toISOString(),
    }).eq('Id', id);
    if (error) throw error;
    return true;
  },

  async deleteInvoice(id: string): Promise<boolean> {
    const { error } = await supabase.from('Invoices').delete().eq('Id', id);
    if (error) throw error;
    return true;
  },
};

// ─── uptimeService ────────────────────────────────────────────────────────────

export const uptimeService = {
  async getMonitors(): Promise<UptimeMonitorItem[]> {
    const clients = await fetchClients();
    const { data, error } = await supabase.from('UptimeMonitors').select('*').order('CreatedAt', { ascending: false });
    if (error) throw error;
    return (data ?? []).map(r => mapMonitor(r, clients));
  },

  async createMonitor(data: Partial<UptimeMonitorItem>): Promise<string> {
    const id = crypto.randomUUID();
    const { error } = await supabase.from('UptimeMonitors').insert({
      Id: id,
      ClientId: data.clientId,
      Name: data.name,
      Url: data.url,
      CheckIntervalMinutes: data.checkIntervalMinutes ?? 5,
      IsActive: true,
      CreatedAt: new Date().toISOString(),
    });
    if (error) throw error;
    return id;
  },

  async deleteMonitor(id: string): Promise<boolean> {
    const { error } = await supabase.from('UptimeMonitors').delete().eq('Id', id);
    if (error) throw error;
    return true;
  },
};
