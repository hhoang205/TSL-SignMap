import { Flex, Spin } from "antd";

export const FullScreenSpinner = () => (
  <Flex
    align="center"
    justify="center"
    style={{ minHeight: "70vh", width: "100%" }}
  >
    <Spin tip="Đang tải dữ liệu..." size="large" />
  </Flex>
);

