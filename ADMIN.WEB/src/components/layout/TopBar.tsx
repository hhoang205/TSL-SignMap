import { Layout, Dropdown, Avatar, Typography, Space, Button } from "antd";
import { LogoutOutlined, UserOutlined } from "@ant-design/icons";
import { useAuth } from "@/hooks/useAuth";

const { Header } = Layout;

export const TopBar = () => {
  const { user, logout } = useAuth();

  const menuItems = [
    {
      key: "logout",
      label: "Đăng xuất",
      icon: <LogoutOutlined />,
      onClick: logout
    }
  ];

  return (
    <Header
      style={{
        paddingInline: 24,
        background: "#fff",
        display: "flex",
        alignItems: "center",
        justifyContent: "flex-end",
        gap: 16,
        borderBottom: "1px solid #f2f4f7"
      }}
    >
      <Dropdown menu={{ items: menuItems }}>
        <Button type="text">
          <Space size={12}>
            <Avatar icon={<UserOutlined />} />
            <div style={{ textAlign: "left" }}>
              <Typography.Text strong>{user?.username}</Typography.Text>
              <Typography.Paragraph style={{ margin: 0 }} type="secondary">
                {user?.role}
              </Typography.Paragraph>
            </div>
          </Space>
        </Button>
      </Dropdown>
    </Header>
  );
};

