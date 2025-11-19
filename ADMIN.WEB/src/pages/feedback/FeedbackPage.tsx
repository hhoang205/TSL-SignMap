import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  Button,
  Card,
  Form,
  Input,
  PaginationProps,
  Select,
  Space,
  Table,
  message
} from "antd";
import { feedbackService } from "@/services/feedbackService";
import type { FeedbackFilterPayload } from "@/types";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusTag } from "@/components/common/StatusTag";
import { formatDateTime } from "@/utils/formatters";
import { feedbackStatuses } from "@/utils/constants";

const FeedbackPage = () => {
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState<FeedbackFilterPayload>({
    pageNumber: 1,
    pageSize: 10
  });

  const { data, isLoading } = useQuery({
    queryKey: ["feedback", filters],
    queryFn: () => feedbackService.filter(filters)
  });

  const updateStatusMutation = useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) =>
      feedbackService.updateStatus(id, status as never),
    onSuccess: () => {
      message.success("Đã cập nhật trạng thái.");
      queryClient.invalidateQueries({ queryKey: ["feedback"] });
    }
  });

  const handlePagination: PaginationProps["onChange"] = (page, pageSize) => {
    setFilters((prev) => ({ ...prev, pageNumber: page, pageSize }));
  };

  return (
    <>
      <PageHeader
        title="Phản hồi & báo cáo"
        description="Theo dõi và xử lý phản hồi từ cộng đồng."
        actions={<Button onClick={() => queryClient.invalidateQueries({ queryKey: ["feedback"] })}>Làm mới</Button>}
      />
      <Card style={{ marginBottom: 16 }}>
        <Form layout="inline" onFinish={(values) => setFilters((prev) => ({ ...prev, ...values }))}>
          <Form.Item name="status" label="Trạng thái">
            <Select
              allowClear
              placeholder="Chọn trạng thái"
              options={feedbackStatuses.map((status) => ({ label: status, value: status }))}
              style={{ width: 200 }}
            />
          </Form.Item>
          <Form.Item name="search" label="Từ khóa">
            <Input placeholder="Nội dung" allowClear />
          </Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Lọc
            </Button>
            <Button onClick={() => setFilters({ pageNumber: 1, pageSize: 10 })}>Xóa bộ lọc</Button>
          </Space>
        </Form>
      </Card>

      <Card>
        <Table
          loading={isLoading}
          rowKey="id"
          dataSource={data?.data ?? []}
          pagination={{
            current: filters.pageNumber,
            pageSize: filters.pageSize,
            total: data?.count ?? 0,
            onChange: handlePagination
          }}
          columns={[
            { title: "ID", dataIndex: "id", width: 70 },
            { title: "User", dataIndex: "userId", width: 90 },
            { title: "Nội dung", dataIndex: "content" },
            {
              title: "Trạng thái",
              dataIndex: "status",
              render: (value: string) => <StatusTag status={value} />
            },
            {
              title: "Ngày tạo",
              dataIndex: "createdAt",
              render: (value: string) => formatDateTime(value)
            },
            {
              title: "Cập nhật trạng thái",
              key: "action",
              render: (_: unknown, record) => (
                <Select
                  defaultValue={record.status}
                  style={{ width: 150 }}
                  onChange={(value) => updateStatusMutation.mutate({ id: record.id, status: value })}
                  options={feedbackStatuses.map((status) => ({ label: status, value: status }))}
                />
              )
            }
          ]}
        />
      </Card>
    </>
  );
};

export default FeedbackPage;

