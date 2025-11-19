import apiClient from "./apiClient";
import type {
  Feedback,
  FeedbackFilterPayload,
  FeedbackSummary,
  PaginatedResult
} from "@/types";

const basePath = "/api/feedbacks";

export const feedbackService = {
  async getSummary() {
    const { data } = await apiClient.get<FeedbackSummary>(`${basePath}/summary`);
    return data;
  },
  async filter(payload: FeedbackFilterPayload = {}) {
    const { data } = await apiClient.post<PaginatedResult<Feedback>>(
      `${basePath}/filter`,
      payload
    );
    return data;
  },
  async updateStatus(id: number, status: Feedback["status"]) {
    const { data } = await apiClient.put(`${basePath}/${id}/status`, { status });
    return data;
  }
};

