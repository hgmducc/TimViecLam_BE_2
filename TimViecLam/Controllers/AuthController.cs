using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpPost("register/candidate")]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            AuthResult result = await authRepository.RegisterCandidateAsync(requestDto);

            return StatusCode(result.Status, result);
        }

        [HttpPost("register/employer")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterEmployer([FromForm] RegisterEmployerRequest request)
        {
            // --- Validate file upload ---
            if (request.BusinessLicenseFile == null)
            {
                return BadRequest(new AuthResult
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "NO_FILE",
                    Message = "Vui lòng tải lên giấy phép kinh doanh."
                });
            }

            if (request.BusinessLicenseFile.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new AuthResult
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "FILE_TOO_LARGE",
                    Message = "File quá lớn. Vui lòng tải lên file <= 5MB."
                });
            }

            if (!request.BusinessLicenseFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new AuthResult
                {
                    IsSuccess = false,
                    Status = 400,
                    ErrorCode = "INVALID_FILE_TYPE",
                    Message = "File không hợp lệ. Chỉ nhận file PDF."
                });
            }

            // --- Gọi Repository ---
            AuthResult result = await authRepository.RegisterEmployerAsync(request);

            return StatusCode(result.Status, result);
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });

            var result = await authRepository.RegisterAdminAsync(request);
            return StatusCode(result.Status, result);
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            AuthResult result = await authRepository.LoginAsync(requestDto);

            return StatusCode(result.Status, result);
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });

            var result = await authRepository.ForgotPasswordAsync(requestDto);
            return StatusCode(result.Status, result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });

            var result = await authRepository.ResetPasswordAsync(requestDto);
            return StatusCode(result.Status, result);
        }

    }
}
