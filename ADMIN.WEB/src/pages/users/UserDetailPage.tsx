import { useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { Card, Col, Descriptions, Row, Typography } from "antd";
import { userService } from "@/services/userService";
import { PageHeader } from "@/components/common/PageHeader";
import { FullScreenSpinner } from "@/components/common/FullScreenSpinner";
import { StatusTag } from "@/components/common/StatusTag";
import { formatCoins, formatDateTime } from "@/utils/formatters";

const UserDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const userId = Number(id);

  const { data, isLoading } = useQuery({
    queryKey: ["user-profile", userId],
    queryFn: () => userService.getUserProfile(userId),
    enabled: Number.isFinite(userId)
  });

  if (isLoading || !data) {
    return <FullScreenSpinner />;
  }

  return (
    <>
      <PageHeader
        title={`Người dùng #${data.user.id}`}
        description={`Thông tin tài khoản ${data.user.username}`}
      />
      <Row gutter={[16, 16]}>
        <Col xs={24} md={14}>
          <Card title="Thông tin chung">
            <Descriptions column={1} bordered labelStyle={{ width: 150 }}>
              <Descriptions.Item label="Tên đăng nhập">{data.user.username}</Descriptions.Item>
              <Descriptions.Item label="Email">{data.user.email}</Descriptions.Item>
              <Descriptions.Item label="Số điện thoại">
                {data.user.phoneNumber ?? "Chưa cập nhật"}
              </Descriptions.Item>
              <Descriptions.Item label="Vai trò">
                <StatusTag status={data.user.role} />
              </Descriptions.Item>
              <Descriptions.Item label="Ngày tạo">
                {formatDateTime(data.user.createdAt ?? "")}
              </Descriptions.Item>
              <Descriptions.Item label="Lần cập nhật">
                {data.user.updatedAt ? formatDateTime(data.user.updatedAt) : "—"}
              </Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>
        <Col xs={24} md={10}>
          <Card title="Hiệu suất cộng đồng">
            <Typography.Paragraph>
              <strong>Số dư coin:</strong> {formatCoins(data.coinBalance)}
            </Typography.Paragraph>
            <Typography.Paragraph>
              <strong>Đóng góp đã gửi:</strong> {data.totalContributions}
            </Typography.Paragraph>
            <Typography.Paragraph>
              <strong>Bình chọn:</strong> {data.totalVotes}
            </Typography.Paragraph>
          </Card>
        </Col>
      </Row>
    </>
  );
};

export default UserDetailPage;

