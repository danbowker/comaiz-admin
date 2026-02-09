import api from './api';
import { 
  Client, 
  ApplicationUser,
  Contract, 
  ContractRate, 
  UserContractRate,
  FixedCost, 
  WorkRecord, 
  Invoice, 
  InvoiceItem,
  Task,
  ContractDetails,
  CreateFixedCostInvoiceItemDto,
  CreateLabourCostInvoiceItemDto,
  CreateMileageCostInvoiceItemDto
} from '../types';

// Generic CRUD service for all entities
export class EntityService<T> {
  protected endpoint: string;

  constructor(endpoint: string) {
    this.endpoint = endpoint;
  }

  async getAll(params?: Record<string, any>): Promise<T[]> {
    const queryParams = new URLSearchParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== null && value !== undefined) {
          queryParams.append(key, String(value));
        }
      });
    }
    const queryString = queryParams.toString();
    const url = queryString ? `/${this.endpoint}?${queryString}` : `/${this.endpoint}`;
    const response = await api.get<T[]>(url);
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

  async duplicate(id: number | string): Promise<T> {
    const response = await api.post<T>(`/${this.endpoint}/${id}/duplicate`);
    return response.data;
  }
}

// Extended InvoiceItemsService with specialized creation methods
class InvoiceItemsService extends EntityService<InvoiceItem> {
  async createFixedCost(dto: CreateFixedCostInvoiceItemDto): Promise<InvoiceItem> {
    const response = await api.post<InvoiceItem>(`/${this.endpoint}/create-fixed-cost`, dto);
    return response.data;
  }

  async createLabourCost(dto: CreateLabourCostInvoiceItemDto): Promise<InvoiceItem> {
    const response = await api.post<InvoiceItem>(`/${this.endpoint}/create-labour-cost`, dto);
    return response.data;
  }

  async createMileageCost(dto: CreateMileageCostInvoiceItemDto): Promise<InvoiceItem> {
    const response = await api.post<InvoiceItem>(`/${this.endpoint}/create-mileage-cost`, dto);
    return response.data;
  }
}

// Export services for all entities with proper types
export const clientsService = new EntityService<Client>('clients');
export const usersService = new EntityService<ApplicationUser>('users');
export const contractsService = new EntityService<Contract>('contracts');
export const contractRatesService = new EntityService<ContractRate>('contractrates');
export const userContractRatesService = new EntityService<UserContractRate>('usercontractrates');
export const fixedCostsService = new EntityService<FixedCost>('fixedcosts');
export const workRecordsService = new EntityService<WorkRecord>('workrecords');
export const invoicesService = new EntityService<Invoice>('invoices');
export const invoiceItemsService = new InvoiceItemsService('invoiceitems');
export const tasksService = new EntityService<Task>('tasks');

// Contract details service
export const getContractDetails = async (contractId: number): Promise<ContractDetails> => {
  const response = await api.get<ContractDetails>(`/contracts/${contractId}/details`);
  return response.data;
};

