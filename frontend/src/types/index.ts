// Entity types matching backend models

export interface Client {
  id: number;
  shortName?: string;
  name?: string;
}

export interface ApplicationUser {
  id: string;
  userName?: string;
  email?: string;
}

export interface Contract {
  id: number;
  clientId: number;
  description?: string;
  price?: number;
  schedule?: string;
  chargeType: ChargeType;
  plannedStart?: string;
  plannedEnd?: string;
}

export enum ChargeType {
  Fixed = 0,
  TimeAndMaterials = 1,
}

export interface ContractRate {
  id: number;
  contractId: number;
  description: string;
  rate?: number;
  applicationUserId?: string;
  applicationUser?: ApplicationUser;
}

export interface Task {
  id: number;
  name: string;
  contractId?: number;
  contractRateId?: number;
  taskContractRates?: TaskContractRate[];
}

export interface TaskContractRate {
  id?: number;
  taskId?: number;
  contractRateId: number;
  contractRate?: ContractRate;
}

export interface FixedCost {
  id: number;
  contractId: number;
  invoiceItemId?: number;
  name?: string;
  amount?: number;
}

export interface WorkRecord {
  id: number;
  startDate: string;
  endDate: string;
  hours: number;
  applicationUserId?: string;
  taskId?: number;
  invoiceItemId?: number;
}

export interface Invoice {
  id: number;
  date: string;
  purchaseOrder?: string;
  clientId: number;
}

export interface InvoiceItem {
  id: number;
  invoiceId: number;
  taskId?: number;
  fixedCostId?: number;
  quantity: number;
  unit: number;
  rate: number;
  vatRate: number;
  price: number;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
  roles: string[];
}
