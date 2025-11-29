using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/candidate/educations")]
    [ApiController]
    [Authorize(Roles = "Candidate")]
    public class EducationController : ControllerBase
    {
        private readonly IEducationRepository educationRepository;

        public EducationController(IEducationRepository educationRepository)
        {
            this.educationRepository = educationRepository;
        }

        // GET: api/candidate/educations
        [HttpGet]
        public async Task<IActionResult> GetEducations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<List<EducationDto>> result = await educationRepository.GetEducationsByCandidateAsync(userId);
            return StatusCode(result.Status, result);
        }

        // POST: api/candidate/educations
        [HttpPost]
        public async Task<IActionResult> AddEducation([FromBody] AddEducationRequest request)
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

            ApiResult<EducationDto> result = await educationRepository.AddEducationAsync(userId, request);
            return StatusCode(result.Status, result);
        }

        // PUT: api/candidate/educations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] AddEducationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            ApiResult<EducationDto> result = await educationRepository.UpdateEducationAsync(id, request);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/candidate/educations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await educationRepository.DeleteEducationAsync(id, userId);
            return StatusCode(result.Status, result);
        }
    }
}