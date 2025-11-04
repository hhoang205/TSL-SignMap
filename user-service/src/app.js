import express from "express";
import dotenv from "dotenv";
import mongoose from "mongoose";
import bcrypt from "bcryptjs";
import { connectDB } from "./config/db.js";
import userRoutes from "./routes/userRoutes.js";
import User from "./models/User.js";

dotenv.config();
connectDB();

const app = express();
app.use(express.json());

app.use("/api/users", userRoutes);

app.get("/", (req, res) => res.send("SignMap User Service running..."));

mongoose.connection.once("open", async () => {
  console.log("✅ MongoDB connected successfully");

  const adminEmail = "admin@signmap.vn";
  const existingAdmin = await User.findOne({ email: adminEmail });

  if (!existingAdmin) {
    const hashedPassword = await bcrypt.hash("Admin@123", 10);
    const admin = new User({
      name: "System Admin",
      email: adminEmail,
      password: hashedPassword,
      role: "admin",
    });
    await admin.save();
    console.log("✅ Default admin created: admin@signmap.vn / Admin@123");
  } else {
    console.log("ℹ️ Admin already exists");
  }
});

const PORT = process.env.PORT || 5001;
app.listen(PORT, () => console.log(`🚀 User Service listening on port ${PORT}`));
