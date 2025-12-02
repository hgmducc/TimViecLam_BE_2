using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobPostingController : ControllerBase
    {
        private readonly IJobPostingRepository jobPostingRepository;

        public JobPostingController(IJobPostingRepository jobPostingRepository)
        {
            this.jobPostingRepository = jobPostingRepository;
        }

        // GET: api/jobs - Public (Không cần auth)
        [HttpGet]
        public async Task<IActionResult> GetAllJobPostings([FromQuery] JobPostingQueryParameters queryParams)
        {
            PagedResult<JobPostingDto> result = await jobPostingRepository.GetAllJobPostingsAsync(queryParams);
            return StatusCode(result.Status, result);
        }

        // GET: api/jobs/{id} - Public (Không cần auth)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobPostingById(int id)
        {
            // Tăng view count
            await jobPostingRepository.IncrementViewCountAsync(id);

            ApiResult<JobPostingDto> result = await jobPostingRepository.GetJobPostingByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        // GET: api/jobs/employer/my-jobs - Employer only
        [HttpGet("employer/my-jobs")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetMyJobPostings([FromQuery] JobPostingQueryParameters queryParams)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            PagedResult<JobPostingDto> result = await jobPostingRepository.GetJobPostingsByEmployerAsync(userId, queryParams);
            return StatusCode(result.Status, result);
        }

        // POST: api/jobs - Employer only
        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> CreateJobPosting([FromBody] CreateJobPostingRequest request)
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

            ApiResult<JobPostingDto> result = await jobPostingRepository.CreateJobPostingAsync(userId, request);
            return StatusCode(result.Status, result);
        }

        // PUT: api/jobs/{id} - Employer only
        [HttpPut("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> UpdateJobPosting(int id, [FromBody] CreateJobPostingRequest request)
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

            ApiResult<JobPostingDto> result = await jobPostingRepository.UpdateJobPostingAsync(id, userId, request);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/jobs/{id} - Employer only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> DeleteJobPosting(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await jobPostingRepository.DeleteJobPostingAsync(id, userId);
            return StatusCode(result.Status, result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromBody] UpdateJobStatusRequest request)
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

            ApiResult<bool> result = await jobPostingRepository.UpdateJobStatusAsync(id, userId, request.Status);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedJobs(int id, [FromQuery] int count = 5)
        {
            var result = await jobPostingRepository.GetRelatedJobsAsync(id, count);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}/company")]
        public async Task<IActionResult> GetJobCompanyInfo(int id)
        {
            var result = await jobPostingRepository.GetJobCompanyInfoAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpGet("tag/{tagName}")]
        public async Task<IActionResult> GetJobsByTag(
            string tagName,
            [FromQuery] JobPostingQueryParameters queryParams)
        {
            var result = await jobPostingRepository.GetJobsByTagAsync(tagName, queryParams);
            return StatusCode(result.Status, result);
        }
    }
}