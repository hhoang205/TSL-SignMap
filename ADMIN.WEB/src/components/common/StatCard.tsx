import { Card, Flex, Typography } from "antd";
import type { DashboardHighlight } from "@/types";

interface StatCardProps extends DashboardHighlight {
  loading?: boolean;
}

export const StatCard = ({
  title,
  value,
  subtitle,
  trend,
  icon,
  loading
}: StatCardProps) => (
  <Card loading={loading} bodyStyle={{ padding: 20, minHeight: 140 }}>
    <Flex vertical gap={12}>
      <Flex justify="space-between" align="center">
        <Typography.Text type="secondary">{title}</Typography.Text>
        {icon}
      </Flex>
      <Typography.Title level={2} style={{ margin: 0 }}>
        {value}
      </Typography.Title>
      <Flex justify="space-between" align="center">
        <Typography.Text type="secondary">{subtitle}</Typography.Text>
        {trend !== undefined && (
          <Typography.Text type={trend >= 0 ? "success" : "danger"}>
            {trend >= 0 ? "+" : ""}
            {trend.toFixed(1)}%
          </Typography.Text>
        )}
      </Flex>
    </Flex>
  </Card>
);

