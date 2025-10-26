import api from './api';
import { 
  Client, 
  ApplicationUser,
  Contract, 
  ContractRate, 
  FixedCost, 
  WorkRecord, 
  Invoice, 
  InvoiceItem 
} from '../types';

// Generic CRUD service for all entities
export class EntityService<T> {
  private endpoint: string;

  constructor(endpoint: string) {
    this.endpoint = endpoint;
  }

  async getAll(): Promise<T[]> {
    const response = await api.get<T[]>(`/${this.endpoint}`);
    return response.data;
  }

  async getById(id: number | string): Promise<T> {
    const response = await api.get<T>(`/${this.endpoint}/${id}`);
    return response.data;
  }

  async create(entity: Omit<T, 'id'>): Promise<T> {
    const response = await api.post<T>(`/${this.endpoint}`, entity);
    return response.data;
  }

  async update(entity: T): Promise<void> {
    await api.put(`/${this.endpoint}`, entity);
  }

  async delete(id: number | string): Promise<void> {
    await api.delete(`/${this.endpoint}/${id}`);
  }
}

// Export services for all entities with proper types
export const clientsService = new EntityService<Client>('clients');
export const usersService = new EntityService<ApplicationUser>('users');
export const contractsService = new EntityService<Contract>('contracts');
export const contractRatesService = new EntityService<ContractRate>('contractrates');
export const fixedCostsService = new EntityService<FixedCost>('fixedcosts');
export const workRecordsService = new EntityService<WorkRecord>('workrecords');
export const invoicesService = new EntityService<Invoice>('invoices');
export const invoiceItemsService = new EntityService<InvoiceItem>('invoiceitems');

