import express from "express";
import axios from "axios";
import bodyParser from "body-parser";
import cookieParser from "cookie-parser";
import dotenv from "dotenv";
import expressLayouts from "express-ejs-layouts";
dotenv.config();

const app = express();
const PORT = process.env.PORT || 5002;
const API_URL = process.env.USER_API_URL || "http://user-service:5001/api/users";


app.set("view engine", "ejs");
app.set("views", "./src/views");
app.use(express.static("./src/public"));
app.use(expressLayouts);
app.set("layout", "layout");

app.use(bodyParser.urlencoded({ extended: true }));
app.use(cookieParser());


app.use((req, res, next) => {
  res.locals.title = "Trang quản trị";
  next();
});


function requireLogin(req, res, next) {
  const token = req.cookies.token;
  if (!token) return res.redirect("/login");
  next();
}


app.get("/", (req, res) => res.redirect("/dashboard"));


app.get("/login", (req, res) => res.render("login", { error: null }));

app.post("/login", async (req, res) => {
  try {
    const { email, password } = req.body;
    const response = await axios.post(`${API_URL}/login`, { email, password });
    res.cookie("token", response.data.token, { httpOnly: true });
    res.redirect("/dashboard");
  } catch (err) {
    res.render("login", { error: "Sai tài khoản hoặc mật khẩu" });
  }
});


app.get("/logout", (req, res) => {
  res.clearCookie("token");
  res.redirect("/login");
});


app.get("/dashboard", requireLogin, (req, res) => {
  res.render("dashboard", { title: "Trang Quản Trị" });
});


app.get("/users", requireLogin, async (req, res) => {
  try {
    const users = (await axios.get(API_URL)).data;
    res.render("users", { users, error: null });
  } catch (e) {
    res.render("users", { users: [], error: "Không tải được danh sách người dùng" });
  }
});


app.get("/feedback", requireLogin, (req, res) => {
  res.render("feedback", { title: "Góp ý người dùng" });
});


app.get("/coins", requireLogin, (req, res) => {
  res.render("coins", { title: "Quản lý xu" });
});


app.listen(PORT, () => console.log(`🚀 Admin Web Service running on port ${PORT}`));
