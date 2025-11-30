import api from './api';
import { WeeklySummaryResponse } from '../types';

export const weeklySummaryService = {
  getWeeklySummary: async (
    userId?: string,
    weekStartDate?: string
  ): Promise<WeeklySummaryResponse> => {
    const params = new URLSearchParams();
    if (userId) {
      params.append('userId', userId);
    }
    if (weekStartDate) {
      params.append('weekStartDate', weekStartDate);
    }
    const queryString = params.toString();
    const url = `/WeeklySummary${queryString ? `?${queryString}` : ''}`;
    const response = await api.get<WeeklySummaryResponse>(url);
    return response.data;
  },
};
