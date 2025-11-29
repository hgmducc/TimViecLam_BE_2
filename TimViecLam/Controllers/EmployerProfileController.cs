using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/employer")]
    [ApiController]
    [Authorize(Roles = "Employer")]
    public class EmployerProfileController : ControllerBase
    {
        private readonly IEmployerRepository employerRepository;
        private readonly IWebHostEnvironment env;

        public EmployerProfileController(IEmployerRepository employerRepository, IWebHostEnvironment env)
        {
            this.employerRepository = employerRepository;
            this.env = env;
        }

        // GET: api/employer/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetEmployerProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ProfileResult result = await employerRepository.GetEmployerProfileAsync(userId);
            return StatusCode(result.Status, result);
        }

        // PUT: api/employer/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateEmployerProfile([FromBody] UpdateEmployerProfileRequest request)
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

            ProfileResult result = await employerRepository.UpdateEmployerProfileAsync(userId, request);
            return StatusCode(result.Status, result);
        }

        // PUT: api/employer/license
        [HttpPut("license")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBusinessLicense(IFormFile licenseFile)
        {
            // Validate file upload
            if (licenseFile == null)
            {
                return BadRequest(new ApiResult<string>
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "NO_FILE",
                    Message = "Vui lòng chọn file giấy phép kinh doanh."
                });
            }

            if (licenseFile.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new ApiResult<string>
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "FILE_TOO_LARGE",
                    Message = "File quá lớn. Vui lòng tải lên file <= 5MB."
                });
            }

            if (!licenseFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ApiResult<string>
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "INVALID_FILE_TYPE",
                    Message = "File không hợp lệ. Chỉ nhận file PDF."
                });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<string> result = await employerRepository.UpdateBusinessLicenseAsync(userId, licenseFile, env);
            return StatusCode(result.Status, result);
        }

        // GET: api/employer/verification-status
        [HttpGet("verification-status")]
        public async Task<IActionResult> GetVerificationStatus()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ProfileResult result = await employerRepository.GetEmployerProfileAsync(userId);

            if (result.IsSuccess && result.Data?.EmployerProfile != null)
            {
                return Ok(new
                {
                    isSuccess = true,
                    message = "Lấy trạng thái xác minh thành công.",
                    data = new
                    {
                        verificationStatus = result.Data.EmployerProfile.VerificationStatus,
                        verifiedAt = result.Data.EmployerProfile.VerifiedAt
                    }
                });
            }

            return StatusCode(result.Status, result);
        }
    }
}