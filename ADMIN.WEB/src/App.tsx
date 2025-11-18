import { Navigate, Route, Routes } from "react-router-dom";
import AdminLayout from "@/components/layout/AdminLayout";
import LoginPage from "@/pages/auth/LoginPage";
import DashboardPage from "@/pages/dashboard/DashboardPage";
import ContributionsPage from "@/pages/contributions/ContributionsPage";
import ContributionDetailPage from "@/pages/contributions/ContributionDetailPage";
import UsersPage from "@/pages/users/UsersPage";
import UserDetailPage from "@/pages/users/UserDetailPage";
import TrafficSignsPage from "@/pages/trafficSigns/TrafficSignsPage";
import FeedbackPage from "@/pages/feedback/FeedbackPage";
import PaymentsPage from "@/pages/payments/PaymentsPage";
import VnPayPaymentPage from "@/pages/payments/VnPayPaymentPage";
import { ProtectedRoute } from "@/routes/ProtectedRoute";

const App = () => (
  <Routes>
    <Route path="/login" element={<LoginPage />} />
    <Route element={<ProtectedRoute />}>
      <Route element={<AdminLayout />}>
        <Route index element={<DashboardPage />} />
        <Route path="/contributions" element={<ContributionsPage />} />
        <Route path="/contributions/:id" element={<ContributionDetailPage />} />
        <Route path="/users" element={<UsersPage />} />
        <Route path="/users/:id" element={<UserDetailPage />} />
        <Route path="/traffic-signs" element={<TrafficSignsPage />} />
        <Route path="/feedback" element={<FeedbackPage />} />
        <Route path="/payments" element={<PaymentsPage />} />
        <Route path="/payments/vnpay" element={<VnPayPaymentPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Route>
  </Routes>
);

export default App;

