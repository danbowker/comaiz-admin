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
  state: RecordState;
  plannedStart?: string;
  plannedEnd?: string;
}

export enum ChargeType {
  Fixed = 0,
  TimeAndMaterials = 1,
}

export enum RecordState {
  Active = 0,
  Complete = 1,
}

export enum InvoiceState {
  Draft = 0,
  Issued = 1,
  Paid = 2,
}

export interface ContractRate {
  id: number;
  contractId: number;
  description: string;
  invoiceDescription: string;
  rate?: number;
  userContractRates?: UserContractRate[];
}

export interface UserContractRate {
  id: number;
  contractRateId: number;
  contractRate?: ContractRate;
  applicationUserId: string;
  applicationUser?: ApplicationUser;
}

export interface Task {
  id: number;
  name: string;
  contractId?: number;
  contractRateId?: number;
  state: RecordState;
  taskContractRates?: TaskContractRate[];
}

export interface TaskContractRate {
  id?: number;
  taskId?: number;
  userContractRateId: number;
  userContractRate?: UserContractRate;
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
  state: InvoiceState;
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
  startDate?: string;
  endDate?: string;
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

export interface TaskHours {
  taskId?: number;
  taskName: string;
  hours: number;
}

export interface DailySummary {
  date: string;
  taskHours: TaskHours[];
  totalHours: number;
}

export interface WeeklySummaryResponse {
  weekStartDate: string;
  weekEndDate: string;
  userId?: string;
  dailySummaries: DailySummary[];
  taskWeeklyTotals: TaskHours[];
  weekTotalHours: number;
}
