import { Card, Typography } from "antd";
import {
  PieChart,
  Pie,
  Cell,
  ResponsiveContainer,
  Tooltip,
  Legend
} from "recharts";

interface PieDatum {
  name: string;
  value: number;
  [key: string]: string | number;
}

const colors = ["#2f80ed", "#10b981", "#f97316", "#ef4444", "#a855f7"];

interface Props {
  title: string;
  data: PieDatum[];
  loading?: boolean;
}

export const StatusPieChart = ({ title, data, loading }: Props) => (
  <Card loading={loading} style={{ height: "100%" }}>
    <Typography.Title level={4}>{title}</Typography.Title>
    <div style={{ width: "100%", height: 280 }}>
      <ResponsiveContainer>
        <PieChart>
          <Pie
            dataKey="value"
            data={data}
            cx="50%"
            cy="50%"
            outerRadius={90}
            label
          >
            {data.map((entry, index) => (
              <Cell key={`cell-${entry.name}`} fill={colors[index % colors.length]} />
            ))}
          </Pie>
          <Tooltip />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </div>
  </Card>
);

