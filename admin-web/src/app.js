import express from "express";
import dotenv from "dotenv";
import cors from "cors";
import adminRoutes from "./routes/adminRoutes.js";

dotenv.config();

const app = express();
app.use(cors());
app.use(express.json());

app.use("/", adminRoutes);

app.get("/", (req, res) => res.send("Admin Web Service running..."));

const PORT = process.env.PORT || 5002;
app.listen(PORT, () => {
  console.log(`🚀 Admin Web Service running on port ${PORT}`);
});
