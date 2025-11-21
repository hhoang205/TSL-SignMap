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

  const adminEmail = "caobahungdeptrai@gmail.com";
  const adminPassword = "123456";
  const hashedPassword = await bcrypt.hash(adminPassword, 10);

  const existingAdmin = await User.findOne({ email: adminEmail });

  if (!existingAdmin) {
  
    const admin = new User({
      name: "Cao Bá Hưng",
      email: adminEmail,
      password: hashedPassword,
      role: "admin",
      phone: "0123456789",
    });
    await admin.save();
    console.log("✅ Default admin created: " + adminEmail + " / " + adminPassword);
  } else {
   
    existingAdmin.password = hashedPassword;
    existingAdmin.role = "admin";
    existingAdmin.name = "Cao Bá Hưng";
    existingAdmin.phone = "0123456789";
    await existingAdmin.save();
    console.log("🔁 Admin password reset to default (123456)");
  }
});

const PORT = process.env.PORT || 5001;
app.listen(PORT, () => console.log(`🚀 User Service listening on port ${PORT}`));
