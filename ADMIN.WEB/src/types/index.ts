import type { ReactNode } from "react";
export type Role = "User" | "Staff" | "Admin";

export interface AuthUser {
  id: number;
  username: string;
  firstname?: string;
  lastname?: string;
  email: string;
  phoneNumber?: string;
  reputation?: number;
  roleId?: number;
  role: Role;
  createdAt?: string;
  updatedAt?: string | null;
}

export type ContributionAction = "Add" | "Update" | "Delete";
export type ContributionStatus = "Pending" | "Approved" | "Rejected";

export interface Contribution {
  id: number;
  userId: number;
  signId?: number | null;
  action: ContributionAction;
  description?: string;
  imageUrl?: string;
  status: ContributionStatus;
  createdAt: string;
  type?: string;
  latitude?: number | null;
  longitude?: number | null;
}

export interface ContributionFilterPayload {
  status?: ContributionStatus;
  action?: ContributionAction;
  userId?: number;
}

export interface ContributionReviewPayload {
  status: Exclude<ContributionStatus, "Pending">;
  adminNote?: string;
  rewardAmount?: number;
}

export interface VoteSummary {
  contributionId: number;
  totalVotes: number;
  upvotes: number;
  downvotes: number;
  averageWeight: number;
  totalScore: number;
}

export interface UserProfile {
  user: AuthUser;
  coinBalance: number;
  totalContributions: number;
  totalVotes: number;
}

export interface PaginatedResult<T> {
  data: T[];
  count: number;
  pageNumber?: number;
  pageSize?: number;
  totalPages?: number;
}

export interface ApiListResponse<T> {
  message?: string;
  count?: number;
  data: T[];
}

export interface TrafficSign {
  id: number;
  type: string;
  latitude: number;
  longitude: number;
  status: string;
  validFrom: string;
  validTo?: string | null;
  imageUrl?: string;
}

export interface TrafficSignPayload {
  type: string;
  latitude: number;
  longitude: number;
  status?: string;
  imageUrl?: string;
  validFrom?: string;
  validTo?: string;
}

export interface Payment {
  id: number;
  amount: number;
  paymentDate: string;
  paymentMethod: string;
  status: string;
  userId: number;
  username?: string;
}

export interface PaymentSummary {
  totalPayments: number;
  completedPayments: number;
  pendingPayments: number;
  failedPayments: number;
  totalAmount: number;
  averageAmount: number;
  totalCompletedAmount: number;
}

export interface Feedback {
  id: number;
  userId: number;
  content: string;
  status: "Pending" | "Reviewed" | "Resolved";
  createdAt: string;
  resolvedAt?: string | null;
  username?: string;
}

export interface FeedbackSummary {
  totalFeedbacks: number;
  pendingFeedbacks: number;
  reviewedFeedbacks: number;
  resolvedFeedbacks: number;
  averageResolutionTime: number;
}

export interface FeedbackFilterPayload {
  userId?: number;
  status?: Feedback["status"];
  startDate?: string;
  endDate?: string;
  search?: string;
  isResolved?: boolean;
  pageNumber?: number;
  pageSize?: number;
}

export interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<AuthUser>;
  logout: () => void;
}

export interface DashboardHighlight {
  title: string;
  value: string | number;
  subtitle?: string;
  trend?: number;
  icon?: ReactNode;
}

