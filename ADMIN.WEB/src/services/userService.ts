import apiClient from "./apiClient";
import type {
  AuthUser,
  PaginatedResult,
  UserProfile
} from "@/types";

const basePath = "/api/users";

export interface LoginPayload {
  email: string;
  password: string;
}

interface AuthResponse {
  user: AuthUser;
  message: string;
}

export interface UserQueryParams {
  pageNumber?: number;
  pageSize?: number;
  username?: string;
}

export interface UpdateUserPayload {
  username?: string;
  email?: string;
  phoneNumber?: string;
  firstname?: string;
  lastname?: string;
  role?: "User" | "Staff" | "Admin";
}

export const userService = {
  async login(payload: LoginPayload) {
    const { data } = await apiClient.post<AuthResponse>(`${basePath}/login`, payload);
    return data;
  },
  async getUsers(params: UserQueryParams = {}) {
    const { data } = await apiClient.get<PaginatedResult<AuthUser>>(basePath, { params });
    return data;
  },
  async getUserProfile(userId: number) {
    const { data } = await apiClient.get<UserProfile>(`${basePath}/${userId}/profile`);
    return data;
  },
  async updateUser(userId: number, payload: UpdateUserPayload) {
    const { data } = await apiClient.put(`${basePath}/${userId}`, payload);
    return data;
  },
  async deleteUser(userId: number) {
    const { data } = await apiClient.delete(`${basePath}/${userId}`);
    return data;
  }
};

