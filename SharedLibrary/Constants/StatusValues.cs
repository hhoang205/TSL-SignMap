namespace SharedLibrary.Constants
{

    /// Common status values used across services

    public static class StatusValues
    {
        // Contribution statuses
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string UnderReview = "UnderReview";

        // Payment statuses
        public const string PaymentPending = "Pending";
        public const string PaymentCompleted = "Completed";
        public const string PaymentFailed = "Failed";
        public const string PaymentCancelled = "Cancelled";

        // Feedback statuses
        public const string FeedbackPending = "Pending";
        public const string FeedbackReviewed = "Reviewed";
        public const string FeedbackResolved = "Resolved";

        // Notification statuses
        public const string NotificationUnread = "Unread";
        public const string NotificationRead = "Read";
    }
}

