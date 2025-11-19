import type { ContributionAction, ContributionStatus } from "@/types";

export const APP_NAME = "SignMap Admin";

export const storageKeys = {
  token: "signmap_admin_token",
  user: "signmap_admin_user"
};

export const contributionStatuses: ContributionStatus[] = [
  "Pending",
  "Approved",
  "Rejected"
];

export const contributionActions: ContributionAction[] = [
  "Add",
  "Update",
  "Delete"
];

export const feedbackStatuses = ["Pending", "Reviewed", "Resolved"] as const;

export const paymentStatuses = ["Pending", "Completed", "Failed"];

export const defaultMapZoom = 16;

export const apiConfig = {
  timeout: 15000
};

