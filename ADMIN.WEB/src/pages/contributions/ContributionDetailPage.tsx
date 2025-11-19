import { useParams } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Button, Card, Col, Descriptions, Flex, Row, Space, Typography, message } from "antd";
import { contributionService } from "@/services/contributionService";
import { voteService } from "@/services/voteService";
import { trafficSignService } from "@/services/trafficSignService";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusTag } from "@/components/common/StatusTag";
import { FullScreenSpinner } from "@/components/common/FullScreenSpinner";
import { formatCoordinate, formatDateTime } from "@/utils/formatters";

const ContributionDetailPage = () => {
  const params = useParams<{ id: string }>();
  const contributionId = Number(params.id);
  const queryClient = useQueryClient();

  const { data: contribution, isLoading } = useQuery({
    queryKey: ["contribution", contributionId],
    queryFn: () => contributionService.getById(contributionId),
    enabled: Number.isFinite(contributionId)
  });

  const { data: voteSummary } = useQuery({
    queryKey: ["votes", "summary", contributionId],
    queryFn: () => voteService.getSummary(contributionId),
    enabled: Number.isFinite(contributionId)
  });

  const { data: relatedSign } = useQuery({
    queryKey: ["traffic-sign", contribution?.signId],
    queryFn: () => trafficSignService.getById(contribution?.signId ?? 0),
    enabled: Boolean(contribution?.signId)
  });

  const approveMutation = useMutation({
    mutationFn: () =>
      contributionService.approve(contributionId, { status: "Approved", rewardAmount: 10 }),
    onSuccess: () => {
      message.success("Đóng góp đã được duyệt.");
      queryClient.invalidateQueries({ queryKey: ["contribution", contributionId] });
      queryClient.invalidateQueries({ queryKey: ["contributions"] });
    }
  });

  const rejectMutation = useMutation({
    mutationFn: () =>
      contributionService.reject(contributionId, { status: "Rejected", adminNote: "Manual review" }),
    onSuccess: () => {
      message.success("Đã từ chối đóng góp.");
      queryClient.invalidateQueries({ queryKey: ["contribution", contributionId] });
      queryClient.invalidateQueries({ queryKey: ["contributions"] });
    }
  });

  if (isLoading || !contribution) {
    return <FullScreenSpinner />;
  }

  return (
    <>
      <PageHeader
        title={`Đóng góp #${contribution.id}`}
        description={`Hành động: ${contribution.action}`}
        actions={
          <Space>
            <Button type="primary" onClick={() => approveMutation.mutate()} loading={approveMutation.isPending}>
              Duyệt
            </Button>
            <Button danger onClick={() => rejectMutation.mutate()} loading={rejectMutation.isPending}>
              Từ chối
            </Button>
          </Space>
        }
      />

      <Row gutter={[16, 16]}>
        <Col xs={24} lg={14}>
          <Card title="Thông tin chi tiết">
            <Descriptions column={1} bordered labelStyle={{ width: 160 }}>
              <Descriptions.Item label="Người dùng">User #{contribution.userId}</Descriptions.Item>
              <Descriptions.Item label="Trạng thái">
                <StatusTag status={contribution.status} />
              </Descriptions.Item>
              <Descriptions.Item label="Mô tả">
                {contribution.description ?? "Không có mô tả."}
              </Descriptions.Item>
              <Descriptions.Item label="Loại biển báo">
                {contribution.type ?? "Chưa xác định"}
              </Descriptions.Item>
              <Descriptions.Item label="Tọa độ">
                {formatCoordinate(contribution.latitude)} / {formatCoordinate(contribution.longitude)}
              </Descriptions.Item>
              <Descriptions.Item label="Ngày tạo">
                {formatDateTime(contribution.createdAt)}
              </Descriptions.Item>
            </Descriptions>
            {contribution.imageUrl && (
              <img
                src={contribution.imageUrl}
                alt="Contribution"
                style={{ width: "100%", marginTop: 16, borderRadius: 12 }}
              />
            )}
          </Card>
        </Col>
        <Col xs={24} lg={10}>
          <Space direction="vertical" style={{ width: "100%" }} size="large">
            <Card title="Tổng hợp bình chọn">
              {voteSummary ? (
                <Flex justify="space-between">
                  <div>
                    <Typography.Text type="secondary">Upvotes</Typography.Text>
                    <Typography.Title level={3}>{voteSummary.upvotes}</Typography.Title>
                  </div>
                  <div>
                    <Typography.Text type="secondary">Downvotes</Typography.Text>
                    <Typography.Title level={3}>{voteSummary.downvotes}</Typography.Title>
                  </div>
                  <div>
                    <Typography.Text type="secondary">Điểm tổng</Typography.Text>
                    <Typography.Title level={3}>{voteSummary.totalScore.toFixed(2)}</Typography.Title>
                  </div>
                </Flex>
              ) : (
                <Typography.Text>Chưa có dữ liệu bình chọn.</Typography.Text>
              )}
            </Card>
            <Card title="Biển báo liên quan">
              {relatedSign ? (
                <Descriptions column={1} bordered labelStyle={{ width: 130 }}>
                  <Descriptions.Item label="ID">#{relatedSign.id}</Descriptions.Item>
                  <Descriptions.Item label="Loại">{relatedSign.type}</Descriptions.Item>
                  <Descriptions.Item label="Trạng thái">
                    <StatusTag status={relatedSign.status} />
                  </Descriptions.Item>
                  <Descriptions.Item label="Hiệu lực">
                    {formatDateTime(relatedSign.validFrom)} →{" "}
                    {relatedSign.validTo ? formatDateTime(relatedSign.validTo) : "Chưa xác định"}
                  </Descriptions.Item>
                </Descriptions>
              ) : (
                <Typography.Text>Chưa có biển báo được tạo từ đóng góp này.</Typography.Text>
              )}
            </Card>
          </Space>
        </Col>
      </Row>
    </>
  );
};

export default ContributionDetailPage;

