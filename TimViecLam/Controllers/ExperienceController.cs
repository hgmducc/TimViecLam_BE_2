using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/candidate/experiences")]
    [ApiController]
    [Authorize(Roles = "Candidate")]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceRepository experienceRepository;

        public ExperienceController(IExperienceRepository experienceRepository)
        {
            this.experienceRepository = experienceRepository;
        }

        // GET: api/candidate/experiences
        [HttpGet]
        public async Task<IActionResult> GetExperiences()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<List<ExperienceDto>> result = await experienceRepository.GetExperiencesByCandidateAsync(userId);
            return StatusCode(result.Status, result);
        }

        // POST: api/candidate/experiences
        [HttpPost]
        public async Task<IActionResult> AddExperience([FromBody] AddExperienceRequest request)
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

            ApiResult<ExperienceDto> result = await experienceRepository.AddExperienceAsync(userId, request);
            return StatusCode(result.Status, result);
        }

        // PUT: api/candidate/experiences/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExperience(int id, [FromBody] AddExperienceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            ApiResult<ExperienceDto> result = await experienceRepository.UpdateExperienceAsync(id, request);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/candidate/experiences/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await experienceRepository.DeleteExperienceAsync(id, userId);
            return StatusCode(result.Status, result);
        }
    }
}