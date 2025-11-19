import dayjs from "dayjs";

export const formatDate = (value?: string | Date | null, pattern = "DD/MM/YYYY") => {
  if (!value) return "—";
  return dayjs(value).format(pattern);
};

export const formatDateTime = (value?: string | Date | null) =>
  formatDate(value, "DD/MM/YYYY HH:mm");

export const formatCurrency = (value?: number, currency = "USD") => {
  if (value === undefined || value === null) return "—";
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
    maximumFractionDigits: 2
  }).format(value);
};

export const formatCoins = (value?: number) => {
  if (value === undefined || value === null) return "—";
  return `${value.toLocaleString("en-US")} Coins`;
};

export const formatCoordinate = (value?: number | null) => {
  if (value === undefined || value === null) return "—";
  return value.toFixed(5);
};

export const getStatusColor = (status: string) => {
  switch (status.toLowerCase()) {
    case "approved":
    case "completed":
    case "resolved":
      return "green";
    case "pending":
      return "gold";
    case "rejected":
    case "failed":
      return "red";
    case "reviewed":
      return "blue";
    default:
      return "default";
  }
};

