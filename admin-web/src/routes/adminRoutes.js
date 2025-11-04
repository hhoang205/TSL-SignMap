import express from "express";
import axios from "axios";

const router = express.Router();

const USER_SERVICE_URL = process.env.USER_SERVICE_URL || "http://user-service:5001";

router.get("/api/users", async (req, res) => {
  try {
    const response = await axios.get(`${USER_SERVICE_URL}/api/users`);
    res.json(response.data);
  } catch (error) {
    console.error("❌ Error loading users:", error.message);
    res.status(500).json({ message: "Cannot load users." });
  }
});

router.put("/api/users/:id/role", async (req, res) => {
  try {
    const response = await axios.put(
      `${USER_SERVICE_URL}/api/users/${req.params.id}/role`,
      { role: req.body.role }
    );
    res.json(response.data);
  } catch (error) {
    console.error("❌ Error updating role:", error.message);
    res.status(500).json({ message: "Cannot update role." });
  }
});

export default router;
