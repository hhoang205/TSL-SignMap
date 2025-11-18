import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  Button,
  Card,
  Form,
  Input,
  InputNumber,
  Modal,
  Space,
  Table,
  message
} from "antd";
import { PlusOutlined, DeleteOutlined } from "@ant-design/icons";
import { trafficSignService } from "@/services/trafficSignService";
import { PageHeader } from "@/components/common/PageHeader";
import { StatusTag } from "@/components/common/StatusTag";
import { formatCoordinate, formatDateTime } from "@/utils/formatters";

const TrafficSignsPage = () => {
  const queryClient = useQueryClient();
  const [isModalVisible, setModalVisible] = useState(false);
  const [form] = Form.useForm();

  const { data: signs = [], isLoading } = useQuery({
    queryKey: ["traffic-signs"],
    queryFn: () => trafficSignService.getAll()
  });

  const createMutation = useMutation({
    mutationFn: trafficSignService.create,
    onSuccess: () => {
      message.success("Đã tạo biển báo mới.");
      queryClient.invalidateQueries({ queryKey: ["traffic-signs"] });
      setModalVisible(false);
      form.resetFields();
    }
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => trafficSignService.remove(id),
    onSuccess: () => {
      message.success("Đã xóa biển báo.");
      queryClient.invalidateQueries({ queryKey: ["traffic-signs"] });
    }
  });

  return (
    <>
      <PageHeader
        title="Quản lý biển báo"
        description="Theo dõi danh mục biển báo được hiển thị trên bản đồ."
        actions={
          <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalVisible(true)}>
            Tạo biển báo
          </Button>
        }
      />

      <Card>
        <Table
          loading={isLoading}
          rowKey="id"
          dataSource={signs}
          columns={[
            { title: "ID", dataIndex: "id", width: 70 },
            { title: "Loại", dataIndex: "type" },
            {
              title: "Tọa độ",
              render: (_: unknown, record) =>
                `${formatCoordinate(record.latitude)} / ${formatCoordinate(record.longitude)}`
            },
            {
              title: "Trạng thái",
              dataIndex: "status",
              render: (value: string) => <StatusTag status={value} />
            },
            {
              title: "Ngày hiệu lực",
              dataIndex: "validFrom",
              render: (value: string) => formatDateTime(value)
            },
            {
              title: "Hết hạn",
              dataIndex: "validTo",
              render: (value: string) => (value ? formatDateTime(value) : "—")
            },
            {
              title: "Thao tác",
              render: (_: unknown, record) => (
                <Space>
                  {record.imageUrl && (
                    <a href={record.imageUrl} target="_blank" rel="noopener noreferrer">
                      Xem ảnh
                    </a>
                  )}
                  <Button
                    danger
                    icon={<DeleteOutlined />}
                    onClick={() => deleteMutation.mutate(record.id)}
                    loading={deleteMutation.isPending && deleteMutation.variables === record.id}
                  >
                    Xóa
                  </Button>
                </Space>
              )
            }
          ]}
        />
      </Card>

      <Modal
        title="Tạo biển báo mới"
        open={isModalVisible}
        onCancel={() => setModalVisible(false)}
        onOk={() => form.submit()}
        confirmLoading={createMutation.isPending}
        okText="Lưu"
        cancelText="Hủy"
      >
        <Form
          layout="vertical"
          form={form}
          onFinish={(values) => createMutation.mutate(values)}
          initialValues={{ status: "Active" }}
        >
          <Form.Item name="type" label="Loại biển báo" rules={[{ required: true }]}>
            <Input placeholder="VD: Cấm dừng đỗ" />
          </Form.Item>
          <Form.Item label="Vĩ độ" name="latitude" rules={[{ required: true }]}>
            <InputNumber style={{ width: "100%" }} placeholder="10.762622" />
          </Form.Item>
          <Form.Item label="Kinh độ" name="longitude" rules={[{ required: true }]}>
            <InputNumber style={{ width: "100%" }} placeholder="106.660172" />
          </Form.Item>
          <Form.Item label="Trạng thái" name="status">
            <Input placeholder="Active / Inactive" />
          </Form.Item>
          <Form.Item label="Ảnh minh họa" name="imageUrl">
            <Input placeholder="https://..." />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default TrafficSignsPage;

