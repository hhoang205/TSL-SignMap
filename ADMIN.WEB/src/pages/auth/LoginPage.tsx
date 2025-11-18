import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button, Card, Col, Form, Input, Row, Typography, Alert } from "antd";
import { LockOutlined, MailOutlined } from "@ant-design/icons";
import { useAuth } from "@/hooks/useAuth";

interface LoginFormValues {
  email: string;
  password: string;
}

const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleFinish = async (values: LoginFormValues) => {
    try {
      setError(null);
      setIsSubmitting(true);
      await login(values.email, values.password);
      navigate("/");
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Row
      justify="center"
      align="middle"
      style={{ minHeight: "100vh", background: "#f5f7fb", padding: 24 }}
    >
      <Col xs={24} sm={18} md={12} lg={8}>
        <Card>
          <Typography.Title level={3} style={{ textAlign: "center" }}>
            SignMap Admin
          </Typography.Title>
          <Typography.Paragraph type="secondary" style={{ textAlign: "center" }}>
            Đăng nhập để quản trị hệ thống biển báo giao thông
          </Typography.Paragraph>
          {error && (
            <Alert
              type="error"
              message="Đăng nhập thất bại"
              description={error}
              style={{ marginBottom: 16 }}
            />
          )}
          <Form layout="vertical" onFinish={handleFinish} requiredMark={false}>
            <Form.Item
              label="Email"
              name="email"
              rules={[
                { required: true, message: "Vui lòng nhập email" },
                { type: "email", message: "Email không hợp lệ" }
              ]}
            >
              <Input prefix={<MailOutlined />} placeholder="admin@signmap.vn" />
            </Form.Item>
            <Form.Item
              label="Mật khẩu"
              name="password"
              rules={[{ required: true, message: "Vui lòng nhập mật khẩu" }]}
            >
              <Input.Password prefix={<LockOutlined />} placeholder="••••••••" />
            </Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              block
              size="large"
              loading={isSubmitting}
            >
              Đăng nhập
            </Button>
          </Form>
        </Card>
      </Col>
    </Row>
  );
};

export default LoginPage;

