using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/saved-jobs")]
    [ApiController]
    [Authorize(Roles = "Candidate")]
    public class SavedJobController : ControllerBase
    {
        private readonly ISavedJobRepository savedJobRepository;

        public SavedJobController(ISavedJobRepository savedJobRepository)
        {
            this.savedJobRepository = savedJobRepository;
        }

        // GET: api/saved-jobs
        [HttpGet]
        public async Task<IActionResult> GetSavedJobs([FromQuery] JobPostingQueryParameters queryParams)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            PagedResult<SavedJobDto> result = await savedJobRepository.GetSavedJobsAsync(userId, queryParams);
            return StatusCode(result.Status, result);
        }

        // POST: api/saved-jobs/{jobId}
        [HttpPost("{jobId}")]
        public async Task<IActionResult> SaveJob(int jobId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<SavedJobDto> result = await savedJobRepository.SaveJobAsync(userId, jobId);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/saved-jobs/{jobId}
        [HttpDelete("{jobId}")]
        public async Task<IActionResult> UnsaveJob(int jobId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await savedJobRepository.UnsaveJobAsync(userId, jobId);
            return StatusCode(result.Status, result);
        }

        // GET: api/saved-jobs/check/{jobId}
        [HttpGet("check/{jobId}")]
        public async Task<IActionResult> CheckIfSaved(int jobId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await savedJobRepository.CheckIfSavedAsync(userId, jobId);
            return StatusCode(result.Status, result);
        }

        // GET: api/saved-jobs/count
        [HttpGet("count")]
        public async Task<IActionResult> GetSavedJobsCount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<int> result = await savedJobRepository.GetSavedJobsCountAsync(userId);
            return StatusCode(result.Status, result);
        }
    }
}