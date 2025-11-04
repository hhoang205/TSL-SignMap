const axios = require("axios");
const path = require("path");

const API_BASE =
  process.env.API_MODE === "gateway"
    ? "http://api-gateway:5000/api/users"
    : "http://user-service:5001/api/users";

exports.getLogin = (req, res) => {
  res.sendFile("login.html", { root: path.join(__dirname, "../views") });
};

exports.login = async (req, res) => {
  const { email, password } = req.body;
  try {
    const response = await axios.post(`${API_BASE}/login`, { email, password });
    const token = response.data.token;
    res.redirect(`/users?token=${token}`);
  } catch (err) {
    res.send("❌ Login failed. Check your credentials.");
  }
};

exports.getUsers = async (req, res) => {
  const token = req.query.token;
  try {
    const response = await axios.get(`${API_BASE}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const users = response.data;
    let html = "<h1>User List</h1><ul>";
    users.forEach((u) => {
      html += `<li>${u.email} - ${u.role}</li>`;
    });
    html += "</ul>";
    res.send(html);
  } catch (err) {
    res.send("⚠️ Cannot load users.");
  }
};
