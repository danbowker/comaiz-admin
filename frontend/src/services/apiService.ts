import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';
import {
  Client,
  Contract,
  ContractRate,
  Worker,
  WorkRecord,
  Invoice,
  InvoiceItem,
  Cost,
} from '../types';

class ApiService {
  private api: AxiosInstance;
  private baseURL = process.env.REACT_APP_API_URL || 'https://localhost:7208/api';

  constructor() {
    this.api = axios.create({
      baseURL: this.baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor to add auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('access_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor for error handling
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Handle unauthorized access
          localStorage.removeItem('access_token');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Generic CRUD methods
  private async get<T>(endpoint: string): Promise<T[]> {
    const response = await this.api.get<T[]>(endpoint);
    return response.data;
  }

  private async getById<T>(endpoint: string, id: number): Promise<T> {
    const response = await this.api.get<T>(`${endpoint}/${id}`);
    return response.data;
  }

  private async create<T>(endpoint: string, data: Omit<T, 'id'>): Promise<T> {
    const response = await this.api.post<T>(endpoint, data);
    return response.data;
  }

  private async update<T extends { id: number }>(endpoint: string, data: T): Promise<void> {
    await this.api.put(`${endpoint}`, data);
  }

  private async delete(endpoint: string, id: number): Promise<void> {
    await this.api.delete(`${endpoint}/${id}`);
  }

  // Clients
  getClients = () => this.get<Client>('/clients');
  getClient = (id: number) => this.getById<Client>('/clients', id);
  createClient = (client: Omit<Client, 'id'>) => this.create<Client>('/clients', client);
  updateClient = (client: Client) => this.update('/clients', client);
  deleteClient = (id: number) => this.delete('/clients', id);

  // Contracts
  getContracts = () => this.get<Contract>('/contracts');
  getContract = (id: number) => this.getById<Contract>('/contracts', id);
  createContract = (contract: Omit<Contract, 'id'>) => this.create<Contract>('/contracts', contract);
  updateContract = (contract: Contract) => this.update('/contracts', contract);
  deleteContract = (id: number) => this.delete('/contracts', id);

  // Contract Rates
  getContractRates = () => this.get<ContractRate>('/contractrates');
  getContractRate = (id: number) => this.getById<ContractRate>('/contractrates', id);
  createContractRate = (rate: Omit<ContractRate, 'id'>) => this.create<ContractRate>('/contractrates', rate);
  updateContractRate = (rate: ContractRate) => this.update('/contractrates', rate);
  deleteContractRate = (id: number) => this.delete('/contractrates', id);

  // Workers
  getWorkers = () => this.get<Worker>('/workers');
  getWorker = (id: number) => this.getById<Worker>('/workers', id);
  createWorker = (worker: Omit<Worker, 'id'>) => this.create<Worker>('/workers', worker);
  updateWorker = (worker: Worker) => this.update('/workers', worker);
  deleteWorker = (id: number) => this.delete('/workers', id);

  // Work Records
  getWorkRecords = () => this.get<WorkRecord>('/workrecords');
  getWorkRecord = (id: number) => this.getById<WorkRecord>('/workrecords', id);
  createWorkRecord = (record: Omit<WorkRecord, 'id'>) => this.create<WorkRecord>('/workrecords', record);
  updateWorkRecord = (record: WorkRecord) => this.update('/workrecords', record);
  deleteWorkRecord = (id: number) => this.delete('/workrecords', id);

  // Invoices
  getInvoices = () => this.get<Invoice>('/invoices');
  getInvoice = (id: number) => this.getById<Invoice>('/invoices', id);
  createInvoice = (invoice: Omit<Invoice, 'id'>) => this.create<Invoice>('/invoices', invoice);
  updateInvoice = (invoice: Invoice) => this.update('/invoices', invoice);
  deleteInvoice = (id: number) => this.delete('/invoices', id);

  // Invoice Items
  getInvoiceItems = () => this.get<InvoiceItem>('/invoiceitems');
  getInvoiceItem = (id: number) => this.getById<InvoiceItem>('/invoiceitems', id);
  createInvoiceItem = (item: Omit<InvoiceItem, 'id'>) => this.create<InvoiceItem>('/invoiceitems', item);
  updateInvoiceItem = (item: InvoiceItem) => this.update('/invoiceitems', item);
  deleteInvoiceItem = (id: number) => this.delete('/invoiceitems', id);

  // Fixed Costs
  getFixedCosts = () => this.get<Cost>('/fixedcosts');
  getFixedCost = (id: number) => this.getById<Cost>('/fixedcosts', id);
  createFixedCost = (cost: Omit<Cost, 'id'>) => this.create<Cost>('/fixedcosts', cost);
  updateFixedCost = (cost: Cost) => this.update('/fixedcosts', cost);
  deleteFixedCost = (id: number) => this.delete('/fixedcosts', id);
}

export const apiService = new ApiService();