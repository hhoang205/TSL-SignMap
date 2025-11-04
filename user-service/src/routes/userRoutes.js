import express from "express";
import {
  registerUser,
  loginUser,
  getUserProfile,
  updateUserProfile,
  changePassword,
  forgotPassword
} from "../controllers/userController.js";
import { protect } from "../middlewares/authMiddleware.js";

const router = express.Router();

router.post("/register", registerUser);
router.post("/login", loginUser);
router.get("/:id", protect, getUserProfile);
router.put("/:id", protect, updateUserProfile);
router.put("/:id/change-password", protect, changePassword);
router.post("/forgot-password", forgotPassword);

export default router;