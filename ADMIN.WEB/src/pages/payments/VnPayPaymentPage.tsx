import { useState } from "react";
import { Button, Card, Form, InputNumber, Select, Typography, message } from "antd";
import { paymentService } from "@/services/paymentService";
import { useAuth } from "@/hooks/useAuth";

const bankOptions = [
  { label: "Không chọn (tự để VNPAY)", value: "" },
  { label: "Vietcombank", value: "VCB" },
  { label: "VietinBank", value: "ICB" },
  { label: "BIDV", value: "BIDV" },
  { label: "Techcombank", value: "TCB" }
];

const VnPayPaymentPage = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);

  const handleFinish = async (values: { amount: number; bankCode?: string }) => {
    if (!user) {
      message.error("Bạn cần đăng nhập để nạp coin.");
      return;
    }

    try {
      setLoading(true);
      const { amount, bankCode } = values;
      const { paymentUrl } = await paymentService.createVnPayPayment({
        userId: user.id,
        amount,
        bankCode: bankCode || undefined
      });

      window.location.href = paymentUrl;
    } catch (error: any) {
      // eslint-disable-next-line no-console
      console.error(error);
      const apiMessage =
        error?.response?.data?.message || "Không thể tạo giao dịch VNPAY. Vui lòng thử lại.";
      message.error(apiMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card style={{ maxWidth: 480, margin: "0 auto" }}>
      <Typography.Title level={3}>Nạp coin qua VNPAY</Typography.Title>
      <Typography.Paragraph type="secondary">
        Nhập số tiền muốn nạp, hệ thống sẽ chuyển bạn sang cổng thanh toán VNPAY an toàn.
      </Typography.Paragraph>

      <Form layout="vertical" onFinish={handleFinish}>
        <Form.Item
          label="Số tiền (VND)"
          name="amount"
          rules={[
            { required: true, message: "Vui lòng nhập số tiền" },
            {
              validator: (_, value) => {
                if (!value || value <= 0) {
                  return Promise.reject(new Error("Số tiền phải lớn hơn 0"));
                }
                return Promise.resolve();
              }
            }
          ]}
        >
          <InputNumber
            min={10000}
            step={10000}
            style={{ width: "100%" }}
            placeholder="Ví dụ: 100000"
          />
        </Form.Item>

        <Form.Item label="Ngân hàng" name="bankCode" initialValue="">
          <Select options={bankOptions} />
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" loading={loading} block>
            Thanh toán qua VNPAY
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default VnPayPaymentPage;


