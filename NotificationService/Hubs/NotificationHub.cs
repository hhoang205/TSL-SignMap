using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs
{
    /// SignalR Hub cho real-time notifications
    /// Clients có thể subscribe để nhận notifications real-time
    public class NotificationHub : Hub
    {
        /// Join group theo UserId để nhận notifications của user đó
        public async Task JoinUserGroup(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        /// Leave group
        public async Task LeaveUserGroup(int userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
    }
}

