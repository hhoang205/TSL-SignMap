import apiClient from "./apiClient";
import type {
  ApiListResponse,
  Contribution,
  ContributionFilterPayload,
  ContributionReviewPayload
} from "@/types";

const basePath = "/api/contributions";

const unwrapList = (response: ApiListResponse<Contribution> | Contribution[]) => {
  if (Array.isArray(response)) return response;
  return response.data ?? [];
};

export const contributionService = {
  async getAll() {
    const { data } = await apiClient.get<ApiListResponse<Contribution>>(basePath);
    return unwrapList(data);
  },
  async getById(id: number) {
    const { data } = await apiClient.get<{ data: Contribution }>(`${basePath}/${id}`);
    return data.data;
  },
  async filter(payload: ContributionFilterPayload) {
    const hasFilter = Object.values(payload).some((value) => value !== undefined && value !== "");
    if (!hasFilter) {
      return contributionService.getAll();
    }
    const { data } = await apiClient.post<ApiListResponse<Contribution>>(
      `${basePath}/filter`,
      payload
    );
    return unwrapList(data);
  },
  async approve(id: number, payload: ContributionReviewPayload) {
    const { data } = await apiClient.post(`${basePath}/${id}/approve`, payload);
    return data;
  },
  async reject(id: number, payload: ContributionReviewPayload) {
    const { data } = await apiClient.post(`${basePath}/${id}/reject`, payload);
    return data;
  }
};

