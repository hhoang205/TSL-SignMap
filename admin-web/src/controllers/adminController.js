// src/controllers/adminController.js
import axios from "axios";
import dotenv from "dotenv";
dotenv.config();

// URL của user-service (nếu chạy qua Docker)
const USER_SERVICE_URL = process.env.USER_SERVICE_URL || "http://user-service:5001/api/users";

// 🟢 Trang đăng nhập (render file login.html)
export const getLoginPage = (req, res) => {
  res.sendFile("/app/src/views/login.html");
};

// 🟢 Xử lý đăng nhập admin
export const handleLogin = async (req, res) => {
  const { email, password } = req.body;

  try {
    const response = await axios.post(`${USER_SERVICE_URL}/login`, { email, password });
    const { token } = response.data;

    // Lưu token vào cookie
    res.cookie("token", token, { httpOnly: true });
    res.json({ message: "Login successful", token });
  } catch (error) {
    res.status(401).json({ message: "Invalid credentials" });
  }
};

// 🟢 Lấy danh sách user (chỉ admin)
export const getAllUsers = async (req, res) => {
  try {
    const response = await axios.get(`${USER_SERVICE_URL}/`);
    res.json(response.data);
  } catch (error) {
    console.error("Error fetching users:", error.message);
    res.status(500).json({ message: "Failed to fetch users" });
  }
};
