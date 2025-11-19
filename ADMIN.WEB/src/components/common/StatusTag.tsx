import { Tag } from "antd";
import { getStatusColor } from "@/utils/formatters";

interface Props {
  status: string;
}

export const StatusTag = ({ status }: Props) => (
  <Tag color={getStatusColor(status)} style={{ textTransform: "capitalize" }}>
    {status}
  </Tag>
);

