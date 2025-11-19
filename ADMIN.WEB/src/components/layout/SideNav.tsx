import { Layout, Menu } from "antd";
import {
  DashboardOutlined,
  FlagOutlined,
  TeamOutlined,
  ProfileOutlined,
  DollarOutlined,
  CommentOutlined
} from "@ant-design/icons";
import { useLocation, useNavigate } from "react-router-dom";

const { Sider } = Layout;

const navItems = [
  { key: "/", label: "Tổng quan", icon: <DashboardOutlined /> },
  { key: "/contributions", label: "Đóng góp", icon: <FlagOutlined /> },
  { key: "/traffic-signs", label: "Biển báo", icon: <ProfileOutlined /> },
  { key: "/users", label: "Người dùng", icon: <TeamOutlined /> },
  { key: "/payments", label: "Thanh toán", icon: <DollarOutlined /> },
  { key: "/feedback", label: "Phản hồi", icon: <CommentOutlined /> }
];

const getSelectedKey = (pathname: string) => {
  if (pathname === "/") return "/";
  const match = navItems
    .map((item) => item.key)
    .filter((key) => key !== "/")
    .find((key) => pathname.startsWith(key));
  return match ?? "/";
};

export const SideNav = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const selectedKey = getSelectedKey(location.pathname);

  return (
    <Sider width={230} style={{ background: "#fff" }} breakpoint="lg" collapsedWidth={64}>
      <div
        style={{
          height: 64,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          fontWeight: 700,
          fontSize: 18,
          letterSpacing: 0.4,
          color: "#0f172a"
        }}
      >
        SignMap
      </div>
      <Menu
        mode="inline"
        selectedKeys={[selectedKey]}
        items={navItems}
        onClick={(info) => navigate(info.key)}
      />
    </Sider>
  );
};

