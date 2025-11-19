import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import {
  Button,
  Card,
  Drawer,
  Form,
  Input,
  PaginationProps,
  Space,
  Table,
  Typography
} from "antd";
import { userService } from "@/services/userService";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusTag } from "@/components/common/StatusTag";
import { formatDateTime, formatCoins } from "@/utils/formatters";

const UsersPage = () => {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const [pagination, setPagination] = useState<{ pageNumber: number; pageSize: number }>({
    pageNumber: 1,
    pageSize: 10
  });

  const usersQuery = useQuery({
    queryKey: ["users", pagination, searchTerm],
    queryFn: () =>
      userService.getUsers({
        pageNumber: pagination.pageNumber,
        pageSize: pagination.pageSize,
        username: searchTerm || undefined
      })
  });

  const profileQuery = useQuery({
    queryKey: ["user-profile", selectedUserId],
    queryFn: () => userService.getUserProfile(selectedUserId ?? 0),
    enabled: selectedUserId !== null
  });

  const tableData = useMemo(() => usersQuery.data?.data ?? [], [usersQuery.data]);

  const handleTableChange: PaginationProps["onChange"] = (page, pageSize) => {
    setPagination({ pageNumber: page, pageSize });
  };

  return (
    <>
      <PageHeader
        title="Quản lý người dùng"
        description="Theo dõi, tìm kiếm và quản lý tài khoản người dùng."
        actions={
          <Space>
            <Form layout="inline" onFinish={({ keyword }) => setSearchTerm(keyword ?? "")}>
              <Form.Item name="keyword">
                <Input.Search placeholder="Tìm theo email, username" allowClear enterButton />
              </Form.Item>
            </Form>
            <Button onClick={() => usersQuery.refetch()}>Làm mới</Button>
          </Space>
        }
      />

      <Card>
        <Table
          loading={usersQuery.isLoading}
          rowKey="id"
          dataSource={tableData}
          pagination={{
            current: pagination.pageNumber,
            pageSize: pagination.pageSize,
            total: usersQuery.data?.count ?? 0,
            onChange: handleTableChange
          }}
          columns={[
            { title: "ID", dataIndex: "id", width: 70 },
            { title: "Username", dataIndex: "username" },
            { title: "Email", dataIndex: "email" },
            {
              title: "Vai trò",
              dataIndex: "role",
              render: (value: string) => <StatusTag status={value} />
            },
            {
              title: "Ngày tạo",
              dataIndex: "createdAt",
              render: (value: string) => formatDateTime(value)
            },
            {
              title: "Thao tác",
              key: "actions",
              render: (_: unknown, record) => (
                <Space>
                  <Button type="link" onClick={() => setSelectedUserId(record.id)}>
                    Xem nhanh
                  </Button>
                  <Button onClick={() => navigate(`/users/${record.id}`)}>Trang chi tiết</Button>
                </Space>
              )
            }
          ]}
        />
      </Card>

      <Drawer
        open={selectedUserId !== null}
        width={420}
        title={`Người dùng #${selectedUserId ?? ""}`}
        onClose={() => setSelectedUserId(null)}
      >
        {profileQuery.isFetching && <Typography.Text>Đang tải...</Typography.Text>}
        {profileQuery.data && (
          <Space direction="vertical" style={{ width: "100%" }} size="large">
            <div>
              <Typography.Text type="secondary">Tên đăng nhập</Typography.Text>
              <Typography.Title level={4}>{profileQuery.data.user.username}</Typography.Title>
            </div>
            <div>
              <Typography.Text type="secondary">Email</Typography.Text>
              <Typography.Paragraph>{profileQuery.data.user.email}</Typography.Paragraph>
            </div>
            <div>
              <Typography.Text type="secondary">Số dư coin</Typography.Text>
              <Typography.Title level={3}>{formatCoins(profileQuery.data.coinBalance)}</Typography.Title>
            </div>
            <div>
              <Typography.Text type="secondary">Đóng góp / Bình chọn</Typography.Text>
              <Typography.Paragraph>
                {profileQuery.data.totalContributions} đóng góp · {profileQuery.data.totalVotes} bình chọn
              </Typography.Paragraph>
            </div>
          </Space>
        )}
      </Drawer>
    </>
  );
};

export default UsersPage;

