import apiClient from "./apiClient";
import type { PaginatedResult, Payment, PaymentSummary } from "@/types";

const basePath = "/api/payments";

export interface PaymentFilterPayload {
  userId?: number;
  status?: string;
  paymentMethod?: string;
  startDate?: string;
  endDate?: string;
  minAmount?: number;
  maxAmount?: number;
  pageNumber?: number;
  pageSize?: number;
}

export interface VnPayCreatePayload {
  userId: number;
  amount: number;
  bankCode?: string;
}

export interface VnPayCreateResponse {
  paymentUrl: string;
}

export const paymentService = {
  async getSummary() {
    const { data } = await apiClient.get<PaymentSummary>(`${basePath}/summary`);
    return data;
  },
  async filter(payload: PaymentFilterPayload = {}) {
    const { data } = await apiClient.post<PaginatedResult<Payment>>(
      `${basePath}/filter`,
      payload
    );
    return data;
  },
  async createVnPayPayment(payload: VnPayCreatePayload) {
    const { data } = await apiClient.post<VnPayCreateResponse>(
      `${basePath}/vnpay/create`,
      payload
    );
    return data;
  }
};

