// API Types based on the .NET models

export interface Client {
  id: number;
  shortName?: string;
  name?: string;
  contracts?: Contract[];
}

export interface Contract {
  id: number;
  clientId: number;
  client?: Client;
  name?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  contractRates?: ContractRate[];
}

export interface ContractRate {
  id: number;
  contractId: number;
  contract?: Contract;
  workerId: number;
  worker?: Worker;
  rate: number;
  effectiveDate: string;
}

export interface Worker {
  id: number;
  firstName?: string;
  lastName?: string;
  email?: string;
  contractRates?: ContractRate[];
  workRecords?: WorkRecord[];
}

export interface WorkRecord {
  id: number;
  workerId: number;
  worker?: Worker;
  contractId: number;
  contract?: Contract;
  date: string;
  hours: number;
  description?: string;
}

export interface Invoice {
  id: number;
  clientId: number;
  client?: Client;
  invoiceNumber?: string;
  invoiceDate: string;
  dueDate?: string;
  totalAmount: number;
  paidAmount?: number;
  status?: string;
  invoiceItems?: InvoiceItem[];
}

export interface InvoiceItem {
  id: number;
  invoiceId: number;
  invoice?: Invoice;
  description?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Cost {
  id: number;
  description?: string;
  amount: number;
  date: string;
  category?: string;
}

// API Response types
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}