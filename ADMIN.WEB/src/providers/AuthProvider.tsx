import { createContext, useCallback, useEffect, useMemo, useState } from "react";
import type { ReactNode } from "react";
import { message } from "antd";
import { storageKeys } from "@/utils/constants";
import type { AuthContextValue, AuthUser } from "@/types";
import { setStoredAuthToken, clearStoredAuthToken } from "@/services/apiClient";
import { userService } from "@/services/userService";

export const AuthContext = createContext<AuthContextValue | null>(null);

interface Props {
  children: ReactNode;
}

export const AuthProvider = ({ children }: Props) => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (typeof window === "undefined") return;
    const persistedUser = localStorage.getItem(storageKeys.user);
    const persistedToken = localStorage.getItem(storageKeys.token);
    if (persistedUser && persistedToken) {
      setUser(JSON.parse(persistedUser));
      setToken(persistedToken);
      setStoredAuthToken(persistedToken);
    }
    setIsLoading(false);
  }, []);

  const persistSession = (nextUser: AuthUser, nextToken: string) => {
    setUser(nextUser);
    setToken(nextToken);
    if (typeof window !== "undefined") {
      localStorage.setItem(storageKeys.user, JSON.stringify(nextUser));
    }
    setStoredAuthToken(nextToken);
  };

  const login = useCallback<AuthContextValue["login"]>(async (email, password) => {
    const response = await userService.login({ email, password });
    if (!["Admin", "Staff"].includes(response.user.role)) {
      throw new Error("Tài khoản không có quyền truy cập trang quản trị.");
    }
    const generatedToken =
      window.crypto?.randomUUID?.() ?? `token-${response.user.id}-${Date.now()}`;
    persistSession(response.user, generatedToken);
    message.success(`Xin chào ${response.user.username}!`);
    return response.user;
  }, []);

  const logout = useCallback(() => {
    setUser(null);
    setToken(null);
    if (typeof window !== "undefined") {
      localStorage.removeItem(storageKeys.user);
    }
    clearStoredAuthToken();
    message.success("Đã đăng xuất.");
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isLoading,
      login,
      logout
    }),
    [isLoading, login, logout, token, user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

