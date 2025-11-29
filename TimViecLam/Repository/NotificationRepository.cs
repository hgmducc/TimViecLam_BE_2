using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public NotificationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // ==========================================
        // GET NOTIFICATIONS
        // ==========================================

        public async Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.Notifications
                    .Where(n => n.UserID == userId)
                    .AsQueryable();

                // Filter by read status
                if (queryParams.IsRead.HasValue)
                {
                    query = query.Where(n => n.IsRead == queryParams.IsRead.Value);
                }

                // Filter by type
                if (!string.IsNullOrEmpty(queryParams.Type))
                {
                    query = query.Where(n => n.Type == queryParams.Type);
                }

                int totalRecords = await query.CountAsync();

                // Sorting
                query = queryParams.SortBy?.ToLower() switch
                {
                    "type" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(n => n.Type)
                        : query.OrderByDescending(n => n.Type),
                    "isread" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(n => n.IsRead)
                        : query.OrderByDescending(n => n.IsRead),
                    _ => queryParams.SortOrder == "asc"
                        ? query.OrderBy(n => n.CreatedAt)
                        : query.OrderByDescending(n => n.CreatedAt)
                };

                // Pagination
                var notifications = await query
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .Select(n => new NotificationDto
                    {
                        NotificationID = n.NotificationID,
                        UserID = n.UserID,
                        Title = n.Title,
                        Message = n.Message,
                        Type = n.Type,
                        IsRead = n.IsRead,
                        RelatedLink = n.RelatedLink,
                        CreatedAt = n.CreatedAt,
                        ReadAt = n.ReadAt
                    })
                    .ToListAsync();

                return new PagedResult<NotificationDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách thông báo thành công.",
                    Data = notifications,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<NotificationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<NotificationDto>> GetNotificationByIdAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await dbContext.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId && n.UserID == userId);

                if (notification == null)
                    return new ApiResult<NotificationDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "NOTIFICATION_NOT_FOUND",
                        Message = "Không tìm thấy thông báo."
                    };

                return new ApiResult<NotificationDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thông tin thông báo thành công.",
                    Data = new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.UserID,
                        Title = notification.Title,
                        Message = notification.Message,
                        Type = notification.Type,
                        IsRead = notification.IsRead,
                        RelatedLink = notification.RelatedLink,
                        CreatedAt = notification.CreatedAt,
                        ReadAt = notification.ReadAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<NotificationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // CREATE NOTIFICATIONS
        // ==========================================

        public async Task<ApiResult<NotificationDto>> CreateNotificationAsync(CreateNotificationRequest request)
        {
            try
            {
                var notification = new Notification
                {
                    UserID = request.UserID,
                    Title = request.Title,
                    Message = request.Message,
                    Type = request.Type,
                    RelatedLink = request.RelatedLink,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.Notifications.AddAsync(notification);
                await dbContext.SaveChangesAsync();

                return new ApiResult<NotificationDto>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Tạo thông báo thành công.",
                    Data = new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.UserID,
                        Title = notification.Title,
                        Message = notification.Message,
                        Type = notification.Type,
                        IsRead = notification.IsRead,
                        RelatedLink = notification.RelatedLink,
                        CreatedAt = notification.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<NotificationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<List<NotificationDto>>> CreateBulkNotificationsAsync(List<CreateNotificationRequest> requests)
        {
            try
            {
                var notifications = requests.Select(req => new Notification
                {
                    UserID = req.UserID,
                    Title = req.Title,
                    Message = req.Message,
                    Type = req.Type,
                    RelatedLink = req.RelatedLink,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await dbContext.Notifications.AddRangeAsync(notifications);
                await dbContext.SaveChangesAsync();

                var result = notifications.Select(n => new NotificationDto
                {
                    NotificationID = n.NotificationID,
                    UserID = n.UserID,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToList();

                return new ApiResult<List<NotificationDto>>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = $"Tạo {notifications.Count} thông báo thành công.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<NotificationDto>>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // MARK AS READ
        // ==========================================

        public async Task<ApiResult<bool>> MarkAsReadAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await dbContext.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId && n.UserID == userId);

                if (notification == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "NOTIFICATION_NOT_FOUND",
                        Message = "Không tìm thấy thông báo."
                    };

                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Đánh dấu đã đọc thành công.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var unreadNotifications = await dbContext.Notifications
                    .Where(n => n.UserID == userId && !n.IsRead)
                    .ToListAsync();

                if (unreadNotifications.Any())
                {
                    foreach (var notification in unreadNotifications)
                    {
                        notification.IsRead = true;
                        notification.ReadAt = DateTime.UtcNow;
                    }

                    await dbContext.SaveChangesAsync();
                }

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Đã đánh dấu {unreadNotifications.Count} thông báo là đã đọc.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> MarkMultipleAsReadAsync(List<int> notificationIds, int userId)
        {
            try
            {
                var notifications = await dbContext.Notifications
                    .Where(n => notificationIds.Contains(n.NotificationID) && n.UserID == userId && !n.IsRead)
                    .ToListAsync();

                if (notifications.Any())
                {
                    foreach (var notification in notifications)
                    {
                        notification.IsRead = true;
                        notification.ReadAt = DateTime.UtcNow;
                    }

                    await dbContext.SaveChangesAsync();
                }

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Đã đánh dấu {notifications.Count} thông báo là đã đọc.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // DELETE NOTIFICATIONS
        // ==========================================

        public async Task<ApiResult<bool>> DeleteNotificationAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await dbContext.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId && n.UserID == userId);

                if (notification == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "NOTIFICATION_NOT_FOUND",
                        Message = "Không tìm thấy thông báo."
                    };

                dbContext.Notifications.Remove(notification);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Xóa thông báo thành công.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đá xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> DeleteAllReadNotificationsAsync(int userId)
        {
            try
            {
                var readNotifications = await dbContext.Notifications
                    .Where(n => n.UserID == userId && n.IsRead)
                    .ToListAsync();

                if (readNotifications.Any())
                {
                    dbContext.Notifications.RemoveRange(readNotifications);
                    await dbContext.SaveChangesAsync();
                }

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Đã xóa {readNotifications.Count} thông báo đã đọc.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> DeleteAllNotificationsAsync(int userId)
        {
            try
            {
                var notifications = await dbContext.Notifications
                    .Where(n => n.UserID == userId)
                    .ToListAsync();

                if (notifications.Any())
                {
                    dbContext.Notifications.RemoveRange(notifications);
                    await dbContext.SaveChangesAsync();
                }

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Đã xóa {notifications.Count} thông báo.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // STATISTICS
        // ==========================================

        public async Task<ApiResult<NotificationSummaryDto>> GetNotificationSummaryAsync(int userId)
        {
            try
            {
                var notifications = await dbContext.Notifications
                    .Where(n => n.UserID == userId)
                    .ToListAsync();

                var summary = new NotificationSummaryDto
                {
                    TotalCount = notifications.Count,
                    UnreadCount = notifications.Count(n => !n.IsRead),
                    ReadCount = notifications.Count(n => n.IsRead),
                    TypeCounts = notifications
                        .GroupBy(n => n.Type)
                        .ToDictionary(g => g.Key, g => g.Count())
                };

                return new ApiResult<NotificationSummaryDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thống kê thông báo thành công.",
                    Data = summary
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<NotificationSummaryDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<int>> GetUnreadCountAsync(int userId)
        {
            try
            {
                int unreadCount = await dbContext.Notifications
                    .CountAsync(n => n.UserID == userId && !n.IsRead);

                return new ApiResult<int>
                {
                    IsSuccess = true,
                    Status = 200,
                    Data = unreadCount
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<int>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // ADMIN OPERATIONS
        // ==========================================

        public async Task<ApiResult<bool>> SendNotificationToAllUsersAsync(string title, string message, string type)
        {
            try
            {
                var allUserIds = await dbContext.Users
                    .Where(u => u.Status == "Active")
                    .Select(u => u.UserID)
                    .ToListAsync();

                var notifications = allUserIds.Select(userId => new Notification
                {
                    UserID = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await dbContext.Notifications.AddRangeAsync(notifications);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = $"Đã gửi thông báo đến {notifications.Count} người dùng.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> SendNotificationToRoleAsync(string role, string title, string message, string type)
        {
            try
            {
                var userIds = await dbContext.Users
                    .Where(u => u.Role == role && u.Status == "Active")
                    .Select(u => u.UserID)
                    .ToListAsync();

                if (!userIds.Any())
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "NO_USERS_FOUND",
                        Message = $"Không tìm thấy người dùng với vai trò '{role}'."
                    };

                var notifications = userIds.Select(userId => new Notification
                {
                    UserID = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await dbContext.Notifications.AddRangeAsync(notifications);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = $"Đã gửi thông báo đến {notifications.Count} người dùng có vai trò '{role}'.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }
    }
}