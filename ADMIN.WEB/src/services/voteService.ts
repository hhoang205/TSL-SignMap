import apiClient from "./apiClient";
import type { VoteSummary } from "@/types";

const basePath = "/api/votes";

export const voteService = {
  async getSummary(contributionId: number) {
    const { data } = await apiClient.get<VoteSummary>(
      `${basePath}/contribution/${contributionId}/summary`
    );
    return data;
  }
};

