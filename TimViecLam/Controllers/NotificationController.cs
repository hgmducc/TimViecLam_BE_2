using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        // GET: api/notifications
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] NotificationQueryParameters queryParams)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            PagedResult<NotificationDto> result = await notificationRepository.GetUserNotificationsAsync(userId, queryParams);
            return StatusCode(result.Status, result);
        }

        // GET: api/notifications/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<NotificationDto> result = await notificationRepository.GetNotificationByIdAsync(id, userId);
            return StatusCode(result.Status, result);
        }

        // GET: api/notifications/unread-count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<int> result = await notificationRepository.GetUnreadCountAsync(userId);
            return StatusCode(result.Status, result);
        }

        // GET: api/notifications/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<NotificationSummaryDto> result = await notificationRepository.GetNotificationSummaryAsync(userId);
            return StatusCode(result.Status, result);
        }

        // PATCH: api/notifications/{id}/read
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await notificationRepository.MarkAsReadAsync(id, userId);
            return StatusCode(result.Status, result);
        }

        // PATCH: api/notifications/read-all
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await notificationRepository.MarkAllAsReadAsync(userId);
            return StatusCode(result.Status, result);
        }

        // PATCH: api/notifications/read-multiple
        [HttpPatch("read-multiple")]
        public async Task<IActionResult> MarkMultipleAsRead([FromBody] List<int> notificationIds)
        {
            if (notificationIds == null || !notificationIds.Any())
            {
                return BadRequest(new { message = "Danh sách thông báo không được rỗng." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await notificationRepository.MarkMultipleAsReadAsync(notificationIds, userId);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/notifications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await notificationRepository.DeleteNotificationAsync(id, userId);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/notifications/read
        [HttpDelete("read")]
        public async Task<IActionResult> DeleteAllReadNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await notificationRepository.DeleteAllReadNotificationsAsync(userId);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/notifications
        [HttpDelete]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await notificationRepository.DeleteAllNotificationsAsync(userId);
            return StatusCode(result.Status, result);
        }

        // ==========================================
        // ADMIN ENDPOINTS
        // ==========================================

        // POST: api/notifications/admin/broadcast
        [HttpPost("admin/broadcast")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BroadcastToAll([FromBody] BroadcastNotificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            ApiResult<bool> result = await notificationRepository.SendNotificationToAllUsersAsync(
                request.Title,
                request.Message,
                request.Type
            );

            return StatusCode(result.Status, result);
        }

        // POST: api/notifications/admin/role/{role}
        [HttpPost("admin/role/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BroadcastToRole(string role, [FromBody] BroadcastNotificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            ApiResult<bool> result = await notificationRepository.SendNotificationToRoleAsync(
                role,
                request.Title,
                request.Message,
                request.Type
            );

            return StatusCode(result.Status, result);
        }
    }
}