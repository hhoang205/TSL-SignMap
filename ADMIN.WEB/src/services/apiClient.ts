import axios from "axios";
import { apiConfig, storageKeys } from "@/utils/constants";

const baseURL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

let runtimeToken: string | null =
  typeof window !== "undefined" ? localStorage.getItem(storageKeys.token) : null;

export const setStoredAuthToken = (token: string | null) => {
  runtimeToken = token;
  if (typeof window === "undefined") return;
  if (token) {
    localStorage.setItem(storageKeys.token, token);
  } else {
    localStorage.removeItem(storageKeys.token);
  }
};

export const getStoredAuthToken = () => runtimeToken;

export const clearStoredAuthToken = () => {
  runtimeToken = null;
  if (typeof window !== "undefined") {
    localStorage.removeItem(storageKeys.token);
  }
};

const apiClient = axios.create({
  baseURL,
  timeout: apiConfig.timeout
});

apiClient.interceptors.request.use((config) => {
  const token = runtimeToken ?? (typeof window !== "undefined"
    ? localStorage.getItem(storageKeys.token)
    : null);
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const message =
      error?.response?.data?.message ??
      error?.message ??
      "Không thể kết nối máy chủ.";
    return Promise.reject(new Error(message));
  }
);

export default apiClient;

