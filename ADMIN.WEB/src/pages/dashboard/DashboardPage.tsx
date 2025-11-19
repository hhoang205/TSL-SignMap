import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { Card, Col, Row, Table, Typography } from "antd";
import {
  CheckCircleOutlined,
  ClockCircleOutlined,
  TeamOutlined,
  AimOutlined
} from "@ant-design/icons";
import dayjs from "dayjs";
import { contributionService } from "@/services/contributionService";
import { trafficSignService } from "@/services/trafficSignService";
import { userService } from "@/services/userService";
import { paymentService } from "@/services/paymentService";
import { feedbackService } from "@/services/feedbackService";
import { StatusPieChart } from "@/components/charts/StatusPieChart";
import { TrendLineChart } from "@/components/charts/TrendLineChart";
import { StatCard } from "@/components/common/StatCard";
import { StatusTag } from "@/components/common/StatusTag";
import { PageHeader } from "@/components/common/PageHeader";
import { formatDateTime, formatCurrency } from "@/utils/formatters";

const DashboardPage = () => {
  const navigate = useNavigate();
  const { data: contributions, isLoading: contributionsLoading } = useQuery({
    queryKey: ["contributions", "dashboard"],
    queryFn: () => contributionService.getAll()
  });

  const { data: trafficSigns, isLoading: trafficSignLoading } = useQuery({
    queryKey: ["traffic-signs"],
    queryFn: () => trafficSignService.getAll()
  });

  const { data: users, isLoading: userLoading } = useQuery({
    queryKey: ["users", { pageNumber: 1, pageSize: 5 }],
    queryFn: () => userService.getUsers({ pageNumber: 1, pageSize: 5 })
  });

  const { data: paymentSummary, isLoading: paymentSummaryLoading } = useQuery({
    queryKey: ["payments", "summary"],
    queryFn: () => paymentService.getSummary()
  });

  const { data: paymentList } = useQuery({
    queryKey: ["payments", "latest"],
    queryFn: () => paymentService.filter({ pageSize: 6, pageNumber: 1 })
  });

  const { data: feedbackSummary, isLoading: feedbackSummaryLoading } = useQuery({
    queryKey: ["feedback", "summary"],
    queryFn: () => feedbackService.getSummary()
  });

  const pendingContributions = contributions?.filter((c) => c.status === "Pending").length ?? 0;
  const totalContributions = contributions?.length ?? 0;
  const activeTrafficSigns = trafficSigns?.length ?? 0;
  const totalUsers = users?.count ?? 0;

  const statusDistribution = useMemo(
    () =>
      ["Pending", "Approved", "Rejected"].map((status) => ({
        name: status,
        value: contributions?.filter((c) => c.status === status).length ?? 0
      })),
    [contributions]
  );

  const trendData = useMemo(() => {
    if (!contributions) return [];
    const grouped: Record<string, number> = {};
    contributions.forEach((item) => {
      const key = dayjs(item.createdAt).format("DD/MM");
      grouped[key] = (grouped[key] ?? 0) + 1;
    });
    return Object.entries(grouped)
      .map(([date, value]) => ({ date, value }))
      .sort((a, b) => dayjs(a.date, "DD/MM").diff(dayjs(b.date, "DD/MM")));
  }, [contributions]);

  const recentContributions =
    contributions
      ?.slice()
      .sort((a, b) => dayjs(b.createdAt).valueOf() - dayjs(a.createdAt).valueOf())
      .slice(0, 6) ?? [];

  return (
    <>
      <PageHeader
        title="Tổng quan hệ thống"
        description="Theo dõi sức khỏe toàn bộ nền tảng SignMap theo thời gian thực."
      />
      <Row gutter={[16, 16]}>
        <Col xs={24} md={12} lg={6}>
          <StatCard
            loading={contributionsLoading}
            title="Đóng góp chờ duyệt"
            value={pendingContributions}
            subtitle="Cần xử lý sớm"
            icon={<ClockCircleOutlined style={{ color: "#f97316", fontSize: 28 }} />}
          />
        </Col>
        <Col xs={24} md={12} lg={6}>
          <StatCard
            loading={contributionsLoading}
            title="Đóng góp đã xử lý"
            value={totalContributions - pendingContributions}
            subtitle="Bao gồm approve/reject"
            icon={<CheckCircleOutlined style={{ color: "#10b981", fontSize: 28 }} />}
          />
        </Col>
        <Col xs={24} md={12} lg={6}>
          <StatCard
            loading={trafficSignLoading}
            title="Biển báo hoạt động"
            value={activeTrafficSigns}
            subtitle="Đang hiển thị trên bản đồ"
            icon={<AimOutlined style={{ color: "#6366f1", fontSize: 28 }} />}
          />
        </Col>
        <Col xs={24} md={12} lg={6}>
          <StatCard
            loading={userLoading}
            title="Thành viên cộng đồng"
            value={totalUsers}
            subtitle="Đã đăng ký hệ thống"
            icon={<TeamOutlined style={{ color: "#0ea5e9", fontSize: 28 }} />}
          />
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} lg={12}>
          <StatusPieChart
            title="Phân bố trạng thái đóng góp"
            data={statusDistribution}
            loading={contributionsLoading}
          />
        </Col>
        <Col xs={24} lg={12}>
          <TrendLineChart
            title="Đóng góp theo thời gian"
            data={trendData}
            loading={contributionsLoading}
          />
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} lg={12}>
          <Card title="Đóng góp mới nhất">
            <Table
              size="small"
              rowKey="id"
              dataSource={recentContributions}
              pagination={false}
              onRow={(record) => ({
                onClick: () => navigate(`/contributions/${record.id}`)
              })}
              columns={[
                { title: "ID", dataIndex: "id", width: 70 },
                { title: "Thao tác", dataIndex: "action" },
                {
                  title: "Trạng thái",
                  dataIndex: "status",
                  render: (value: string) => <StatusTag status={value} />
                },
                {
                  title: "Người dùng",
                  dataIndex: "userId",
                  render: (value: number) => `User #${value}`
                },
                {
                  title: "Thời gian",
                  dataIndex: "createdAt",
                  render: (value: string) => formatDateTime(value)
                }
              ]}
            />
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card
            title="Tài chính & phản hồi"
            loading={paymentSummaryLoading || feedbackSummaryLoading}
          >
            <Row gutter={[16, 16]}>
              <Col span={12}>
                <Typography.Text type="secondary">Doanh thu hoàn tất</Typography.Text>
                <Typography.Title level={3} style={{ marginTop: 4 }}>
                  {formatCurrency(paymentSummary?.totalCompletedAmount ?? 0)}
                </Typography.Title>
                <Typography.Text type="secondary">
                  Tổng giao dịch: {paymentSummary?.totalPayments ?? 0}
                </Typography.Text>
              </Col>
              <Col span={12}>
                <Typography.Text type="secondary">Phản hồi chưa xử lý</Typography.Text>
                <Typography.Title level={3} style={{ marginTop: 4 }}>
                  {feedbackSummary?.pendingFeedbacks ?? 0}
                </Typography.Title>
                <Typography.Text type="secondary">
                  Tổng phản hồi: {feedbackSummary?.totalFeedbacks ?? 0}
                </Typography.Text>
              </Col>
            </Row>
            <Typography.Paragraph style={{ marginTop: 16 }} type="secondary">
              Trung bình thời gian xử lý phản hồi:{" "}
              {(feedbackSummary?.averageResolutionTime ?? 0).toFixed(1)} ngày.
            </Typography.Paragraph>
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} lg={12}>
          <Card title="Thanh toán gần đây">
            <Table
              size="small"
              rowKey="id"
              pagination={false}
              dataSource={paymentList?.data ?? []}
              columns={[
                { title: "ID", dataIndex: "id", width: 70 },
                {
                  title: "Số tiền",
                  dataIndex: "amount",
                  render: (value: number) => formatCurrency(value)
                },
                {
                  title: "Trạng thái",
                  dataIndex: "status",
                  render: (value: string) => <StatusTag status={value} />
                },
                {
                  title: "Người dùng",
                  dataIndex: "username",
                  render: (_: string, record) => record.username ?? `User #${record.userId}`
                },
                {
                  title: "Ngày",
                  dataIndex: "paymentDate",
                  render: (value: string) => formatDateTime(value)
                }
              ]}
            />
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card title="Top thành viên mới" loading={userLoading}>
            <Table
              size="small"
              rowKey="id"
              pagination={false}
              dataSource={users?.data ?? []}
              columns={[
                { title: "Tên", dataIndex: "username" },
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
                }
              ]}
            />
          </Card>
        </Col>
      </Row>
    </>
  );
};

export default DashboardPage;

