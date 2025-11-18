import { Card, Typography } from "antd";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer
} from "recharts";

interface TrendDatum {
  date: string;
  value: number;
}

interface Props {
  title: string;
  data: TrendDatum[];
  loading?: boolean;
}

export const TrendLineChart = ({ title, data, loading }: Props) => (
  <Card loading={loading} style={{ height: "100%" }}>
    <Typography.Title level={4}>{title}</Typography.Title>
    <div style={{ width: "100%", height: 280 }}>
      <ResponsiveContainer>
        <LineChart data={data} margin={{ top: 10, right: 16, left: -16, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="date" />
          <YAxis allowDecimals={false} />
          <Tooltip />
          <Line type="monotone" dataKey="value" stroke="#0066ff" strokeWidth={3} />
        </LineChart>
      </ResponsiveContainer>
    </div>
  </Card>
);

