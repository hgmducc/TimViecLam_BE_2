using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            this.profileRepository = profileRepository;
        }

        // GET: api/profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ProfileResult result = await profileRepository.GetProfileAsync(userId);
            return StatusCode(result.Status, result);
        }

        // PUT: api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ProfileResult result = await profileRepository.UpdateProfileAsync(userId, request);
            return StatusCode(result.Status, result);
        }

        // PUT: api/profile/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ProfileResult result = await profileRepository.ChangePasswordAsync(userId, request);
            return StatusCode(result.Status, result);
        }

        // PUT: api/profile/avatar
        [HttpPut("avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            // --- Validate file upload giống RegisterEmployer ---
            if (avatar == null)
            {
                return BadRequest(new ProfileResult
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "NO_FILE",
                    Message = "Vui lòng chọn file ảnh."
                });
            }

            if (avatar.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new ProfileResult
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "FILE_TOO_LARGE",
                    Message = "Kích thước file không được vượt quá 5MB."
                });
            }

            // Sửa phần check extension - giống như RegisterEmployer check PDF
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(avatar.FileName)?.ToLower();

            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new ProfileResult
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "INVALID_FILE_TYPE",
                    Message = "Chỉ chấp nhận file ảnh có định dạng: jpg, jpeg, png, gif."
                });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            // --- Gọi Repository ---
            ProfileResult result = await profileRepository.UpdateAvatarAsync(userId, avatar);
            return StatusCode(result.Status, result);
        }
    }
}