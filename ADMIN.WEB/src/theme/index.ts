import type { ThemeConfig } from "antd";

export const themeConfig: ThemeConfig = {
  token: {
    colorPrimary: "#0066ff",
    borderRadiusLG: 12,
    fontFamily:
      "Inter, 'Segoe UI', system-ui, -apple-system, BlinkMacSystemFont, 'Helvetica Neue', sans-serif",
    colorBgLayout: "#f5f7fb"
  },
  components: {
    Layout: {
      headerBg: "#ffffff",
      bodyBg: "#f5f7fb"
    },
    Menu: {
      itemSelectedBg: "#e6f0ff",
      itemSelectedColor: "#0066ff"
    },
    Card: {
      borderRadiusLG: 16,
      boxShadow: "0 10px 40px rgba(15, 23, 42, 0.08)"
    }
  }
};

