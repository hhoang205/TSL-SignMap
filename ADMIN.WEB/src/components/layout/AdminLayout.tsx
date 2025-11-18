import { Layout } from "antd";
import { Outlet } from "react-router-dom";
import { SideNav } from "./SideNav";
import { TopBar } from "./TopBar";

const { Content } = Layout;

const AdminLayout = () => (
  <Layout style={{ minHeight: "100vh" }}>
    <SideNav />
    <Layout>
      <TopBar />
      <Content style={{ margin: 24 }}>
        <Outlet />
      </Content>
    </Layout>
  </Layout>
);

export default AdminLayout;

