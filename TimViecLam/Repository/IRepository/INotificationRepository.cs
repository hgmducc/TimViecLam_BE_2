using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface INotificationRepository
    {
        // Get notifications
        Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationQueryParameters queryParams);
        Task<ApiResult<NotificationDto>> GetNotificationByIdAsync(int notificationId, int userId);

        // Create notification
        Task<ApiResult<NotificationDto>> CreateNotificationAsync(CreateNotificationRequest request);
        Task<ApiResult<List<NotificationDto>>> CreateBulkNotificationsAsync(List<CreateNotificationRequest> requests);

        // Mark as read
        Task<ApiResult<bool>> MarkAsReadAsync(int notificationId, int userId);
        Task<ApiResult<bool>> MarkAllAsReadAsync(int userId);
        Task<ApiResult<bool>> MarkMultipleAsReadAsync(List<int> notificationIds, int userId);

        // Delete
        Task<ApiResult<bool>> DeleteNotificationAsync(int notificationId, int userId);
        Task<ApiResult<bool>> DeleteAllReadNotificationsAsync(int userId);
        Task<ApiResult<bool>> DeleteAllNotificationsAsync(int userId);

        // Statistics
        Task<ApiResult<NotificationSummaryDto>> GetNotificationSummaryAsync(int userId);
        Task<ApiResult<int>> GetUnreadCountAsync(int userId);

        // Admin operations
        Task<ApiResult<bool>> SendNotificationToAllUsersAsync(string title, string message, string type);
        Task<ApiResult<bool>> SendNotificationToRoleAsync(string role, string title, string message, string type);
    }
}