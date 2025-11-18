import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  Button,
  Card,
  Col,
  Drawer,
  Form,
  InputNumber,
  Row,
  Select,
  Space,
  Table,
  Tag,
  Typography,
  message
} from "antd";
import {
  CheckOutlined,
  CloseOutlined,
  EyeOutlined,
  ReloadOutlined
} from "@ant-design/icons";
import { contributionService } from "@/services/contributionService";
import type {
  Contribution,
  ContributionFilterPayload,
  ContributionStatus
} from "@/types";
import { contributionActions, contributionStatuses } from "@/utils/constants";
import { formatCoordinate, formatDateTime } from "@/utils/formatters";
import { StatusTag } from "@/components/common/StatusTag";
import { PageHeader } from "@/components/common/PageHeader";

const defaultFilters: ContributionFilterPayload = {};

const ContributionsPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState<ContributionFilterPayload>(defaultFilters);
  const [selected, setSelected] = useState<Contribution | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);

  const { data: contributions = [], isLoading } = useQuery({
    queryKey: ["contributions", filters],
    queryFn: () => contributionService.filter(filters)
  });

  const approveMutation = useMutation({
    mutationFn: (payload: { id: number; note?: string; rewardAmount?: number }) =>
      contributionService.approve(payload.id, {
        status: "Approved",
        adminNote: payload.note,
        rewardAmount: payload.rewardAmount ?? 10
      }),
    onSuccess: () => {
      message.success("Đã duyệt đóng góp.");
      queryClient.invalidateQueries({ queryKey: ["contributions"] });
    }
  });

  const rejectMutation = useMutation({
    mutationFn: (payload: { id: number; note?: string }) =>
      contributionService.reject(payload.id, {
        status: "Rejected",
        adminNote: payload.note
      }),
    onSuccess: () => {
      message.success("Đã từ chối đóng góp.");
      queryClient.invalidateQueries({ queryKey: ["contributions"] });
    }
  });

  const filteredPendingCount = useMemo(
    () => contributions.filter((item) => item.status === "Pending").length,
    [contributions]
  );

  const openDrawer = (record: Contribution) => {
    setSelected(record);
    setDrawerOpen(true);
  };

  const handleApprove = (record: Contribution) => {
    approveMutation.mutate({ id: record.id });
  };

  const handleReject = (record: Contribution) => {
    rejectMutation.mutate({ id: record.id });
  };

  return (
    <>
      <PageHeader
        title="Quản lý đóng góp"
        description={`Có ${filteredPendingCount} đóng góp đang chờ xử lý.`}
        actions={
          <Button
            icon={<ReloadOutlined />}
            onClick={() => queryClient.invalidateQueries({ queryKey: ["contributions"] })}
          >
            Làm mới
          </Button>
        }
      />

      <Card style={{ marginBottom: 24 }}>
        <Form
          layout="vertical"
          initialValues={filters}
          onFinish={(values) => setFilters(values)}
        >
          <Row gutter={16}>
            <Col xs={24} md={8}>
              <Form.Item label="Trạng thái" name="status">
                <Select
                  allowClear
                  placeholder="Chọn trạng thái"
                  options={contributionStatuses.map((status) => ({
                    label: status,
                    value: status
                  }))}
                />
              </Form.Item>
            </Col>
            <Col xs={24} md={8}>
              <Form.Item label="Hành động" name="action">
                <Select
                  allowClear
                  placeholder="Chọn hành động"
                  options={contributionActions.map((action) => ({
                    label: action,
                    value: action
                  }))}
                />
              </Form.Item>
            </Col>
            <Col xs={24} md={8}>
              <Form.Item label="User ID" name="userId">
                <InputNumber min={1} style={{ width: "100%" }} placeholder="VD: 12" />
              </Form.Item>
            </Col>
          </Row>
          <Space>
            <Button type="primary" htmlType="submit">
              Áp dụng bộ lọc
            </Button>
            <Button onClick={() => setFilters(defaultFilters)}>Xóa bộ lọc</Button>
          </Space>
        </Form>
      </Card>

      <Card>
        <Table
          loading={isLoading}
          rowKey="id"
          dataSource={contributions}
          pagination={{ pageSize: 10 }}
          columns={[
            { title: "ID", dataIndex: "id", width: 80 },
            {
              title: "User",
              dataIndex: "userId",
              render: (value: number) => (
                <Space>
                  <Tag color="blue">#{value}</Tag>
                  <Button type="link" onClick={() => navigate(`/users/${value}`)}>
                    Chi tiết
                  </Button>
                </Space>
              )
            },
            { title: "Hành động", dataIndex: "action" },
            {
              title: "Trạng thái",
              dataIndex: "status",
              render: (value: ContributionStatus) => <StatusTag status={value} />
            },
            {
              title: "Mô tả",
              dataIndex: "description",
              ellipsis: true
            },
            {
              title: "Thời gian",
              dataIndex: "createdAt",
              render: (value: string) => formatDateTime(value)
            },
            {
              title: "Thao tác",
              key: "actions",
              render: (_: unknown, record: Contribution) => (
                <Space>
                  <Button icon={<EyeOutlined />} onClick={() => openDrawer(record)}>
                    Xem
                  </Button>
                  <Button
                    type="primary"
                    icon={<CheckOutlined />}
                    onClick={() => handleApprove(record)}
                    loading={approveMutation.isPending && approveMutation.variables?.id === record.id}
                    disabled={record.status !== "Pending"}
                  >
                    Duyệt
                  </Button>
                  <Button
                    danger
                    icon={<CloseOutlined />}
                    onClick={() => handleReject(record)}
                    loading={rejectMutation.isPending && rejectMutation.variables?.id === record.id}
                    disabled={record.status !== "Pending"}
                  >
                    Từ chối
                  </Button>
                </Space>
              )
            }
          ]}
        />
      </Card>

      <Drawer
        title={`Đóng góp #${selected?.id}`}
        width={480}
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        extra={
          selected && (
            <Space>
              <Button onClick={() => navigate(`/contributions/${selected.id}`)}>
                Mở trang chi tiết
              </Button>
              <Button type="primary" onClick={() => handleApprove(selected)}>
                Duyệt
              </Button>
              <Button danger onClick={() => handleReject(selected)}>
                Từ chối
              </Button>
            </Space>
          )
        }
      >
        {selected && (
          <Space direction="vertical" size="large" style={{ width: "100%" }}>
            <div>
              <Typography.Text type="secondary">Người dùng</Typography.Text>
              <Typography.Title level={4}>User #{selected.userId}</Typography.Title>
            </div>
            <div>
              <Typography.Text type="secondary">Loại hành động</Typography.Text>
              <Typography.Paragraph>{selected.action}</Typography.Paragraph>
            </div>
            <div>
              <Typography.Text type="secondary">Mô tả</Typography.Text>
              <Typography.Paragraph>{selected.description ?? "—"}</Typography.Paragraph>
            </div>
            <Row gutter={16}>
              <Col span={12}>
                <Typography.Text type="secondary">Vĩ độ</Typography.Text>
                <Typography.Paragraph>{formatCoordinate(selected.latitude)}</Typography.Paragraph>
              </Col>
              <Col span={12}>
                <Typography.Text type="secondary">Kinh độ</Typography.Text>
                <Typography.Paragraph>{formatCoordinate(selected.longitude)}</Typography.Paragraph>
              </Col>
            </Row>
            {selected.imageUrl && (
              <div>
                <Typography.Text type="secondary">Ảnh đính kèm</Typography.Text>
                <img
                  src={selected.imageUrl}
                  alt="Contribution"
                  style={{ width: "100%", borderRadius: 8 }}
                />
              </div>
            )}
          </Space>
        )}
      </Drawer>
    </>
  );
};

export default ContributionsPage;

