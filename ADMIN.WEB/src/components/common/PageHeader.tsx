import type { ReactNode } from "react";
import { Flex, Typography } from "antd";

interface Props {
  title: string;
  description?: string;
  actions?: ReactNode;
}

export const PageHeader = ({ title, description, actions }: Props) => (
  <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
    <div>
      <Typography.Title level={2} style={{ margin: 0 }}>
        {title}
      </Typography.Title>
      {description && (
        <Typography.Text type="secondary">{description}</Typography.Text>
      )}
    </div>
    {actions}
  </Flex>
);

