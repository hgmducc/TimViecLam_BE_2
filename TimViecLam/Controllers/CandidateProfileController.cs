using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/candidate")]
    [ApiController]
    [Authorize(Roles = "Candidate")]
    public class CandidateProfileController : ControllerBase
    {
        private readonly ICandidateRepository candidateRepository;

        public CandidateProfileController(ICandidateRepository candidateRepository)
        {
            this.candidateRepository = candidateRepository;
        }

        // GET: api/candidate/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetCandidateProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ProfileResult result = await candidateRepository.GetCandidateProfileAsync(userId);
            return StatusCode(result.Status, result);
        }

        // PUT: api/candidate/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateCandidateProfile([FromBody] UpdateCandidateProfileRequest request)
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

            ProfileResult result = await candidateRepository.UpdateCandidateProfileAsync(userId, request);
            return StatusCode(result.Status, result);
        }

        // PUT: api/candidate/skills
        [HttpPut("skills")]
        public async Task<IActionResult> UpdateSkills([FromBody] List<string> skills)
        {
            if (skills == null || !skills.Any())
            {
                return BadRequest(new { message = "Danh sách kỹ năng không được rỗng." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            bool success = await candidateRepository.UpdateSkillsAsync(userId, skills);

            if (success)
            {
                return Ok(new
                {
                    isSuccess = true,
                    message = "Cập nhật kỹ năng thành công."
                });
            }

            return StatusCode(500, new
            {
                isSuccess = false,
                message = "Có lỗi xảy ra khi cập nhật kỹ năng."
            });
        }
    }
}