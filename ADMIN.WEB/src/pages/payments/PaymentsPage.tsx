import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  Card,
  Col,
  Form,
  InputNumber,
  Row,
  Select,
  Table,
  Typography,
  Button
} from "antd";
import { paymentService } from "@/services/paymentService";
import type { PaymentFilterPayload } from "@/services/paymentService";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusTag } from "@/components/common/StatusTag";
import { paymentStatuses } from "@/utils/constants";
import { formatCurrency, formatDateTime } from "@/utils/formatters";

const PaymentsPage = () => {
  const [filters, setFilters] = useState<PaymentFilterPayload>({
    pageNumber: 1,
    pageSize: 10
  });

  const { data: listData, isLoading } = useQuery({
    queryKey: ["payments", filters],
    queryFn: () => paymentService.filter(filters)
  });

  const { data: summary } = useQuery({
    queryKey: ["payments", "summary"],
    queryFn: () => paymentService.getSummary()
  });

  return (
    <>
      <PageHeader
        title="Thanh toán & top-up"
        description="Theo dõi giao dịch nạp coin và trạng thái xử lý."
      />

      <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
        <Col xs={24} md={8}>
          <Card>
            <Typography.Text type="secondary">Tổng số giao dịch</Typography.Text>
            <Typography.Title level={2}>{summary?.totalPayments ?? 0}</Typography.Title>
          </Card>
        </Col>
        <Col xs={24} md={8}>
          <Card>
            <Typography.Text type="secondary">Doanh thu hoàn tất</Typography.Text>
            <Typography.Title level={2}>
              {formatCurrency(summary?.totalCompletedAmount ?? 0)}
            </Typography.Title>
          </Card>
        </Col>
        <Col xs={24} md={8}>
          <Card>
            <Typography.Text type="secondary">Giao dịch lỗi</Typography.Text>
            <Typography.Title level={2}>{summary?.failedPayments ?? 0}</Typography.Title>
          </Card>
        </Col>
      </Row>

      <Card style={{ marginBottom: 16 }}>
        <Form layout="inline" onFinish={(values) => setFilters((prev) => ({ ...prev, ...values }))}>
          <Form.Item label="Trạng thái" name="status">
            <Select
              allowClear
              placeholder="Tất cả"
              style={{ width: 200 }}
              options={paymentStatuses.map((status) => ({ label: status, value: status }))}
            />
          </Form.Item>
          <Form.Item label="User ID" name="userId">
            <InputNumber min={1} />
          </Form.Item>
          <Button type="primary" htmlType="submit">
            Lọc
          </Button>
          <Button onClick={() => setFilters({ pageNumber: 1, pageSize: 10 })}>Đặt lại</Button>
        </Form>
      </Card>

      <Card>
        <Table
          loading={isLoading}
          rowKey="id"
          dataSource={listData?.data ?? []}
          pagination={{
            current: filters.pageNumber,
            pageSize: filters.pageSize,
            total: listData?.count ?? 0,
            onChange: (page, pageSize) => setFilters({ ...filters, pageNumber: page, pageSize })
          }}
          columns={[
            { title: "ID", dataIndex: "id", width: 80 },
            {
              title: "Người dùng",
              dataIndex: "username",
              render: (value: string, record) => value ?? `User #${record.userId}`
            },
            {
              title: "Số tiền",
              dataIndex: "amount",
              render: (value: number) => formatCurrency(value)
            },
            { title: "Phương thức", dataIndex: "paymentMethod" },
            {
              title: "Trạng thái",
              dataIndex: "status",
              render: (value: string) => <StatusTag status={value} />
            },
            {
              title: "Ngày thanh toán",
              dataIndex: "paymentDate",
              render: (value: string) => formatDateTime(value)
            }
          ]}
        />
      </Card>
    </>
  );
};

export default PaymentsPage;

