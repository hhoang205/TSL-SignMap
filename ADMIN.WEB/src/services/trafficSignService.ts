import apiClient from "./apiClient";
import type { TrafficSign, TrafficSignPayload } from "@/types";

const basePath = "/api/signs";

export const trafficSignService = {
  async getAll() {
    const { data } = await apiClient.get<TrafficSign[]>(basePath);
    return data;
  },
  async getById(id: number) {
    const { data } = await apiClient.get<TrafficSign>(`${basePath}/${id}`);
    return data;
  },
  async create(payload: TrafficSignPayload) {
    const { data } = await apiClient.post(`${basePath}`, payload);
    return data;
  },
  async update(id: number, payload: Partial<TrafficSignPayload>) {
    const { data } = await apiClient.put(`${basePath}/${id}`, payload);
    return data;
  },
  async remove(id: number) {
    const { data } = await apiClient.delete(`${basePath}/${id}`);
    return data;
  }
};

