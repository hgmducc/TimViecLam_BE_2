using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/applications")]
    [ApiController]
    [Authorize]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationRepository jobApplicationRepository;
        private readonly IWebHostEnvironment env;

        public JobApplicationController(
            IJobApplicationRepository jobApplicationRepository,
            IWebHostEnvironment env)
        {
            this.jobApplicationRepository = jobApplicationRepository;
            this.env = env;
        }

        // ==========================================
        // CANDIDATE ENDPOINTS
        // ==========================================

        // POST: api/applications - Candidate apply job
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ApplyJob([FromForm] ApplyJobRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            // Validate CV file nếu có
            if (request.CVFile != null)
            {
                if (request.CVFile.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new
                    {
                        isSuccess = false,
                        message = "File CV quá lớn. Vui lòng tải lên file <= 10MB."
                    });
                }

                var allowedExtensions = new[] { ". pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(request.CVFile.FileName)?.ToLower();
                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new
                    {
                        isSuccess = false,
                        message = "Chỉ chấp nhận file CV có định dạng: pdf, doc, docx."
                    });
                }
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<JobApplicationDto> result = await jobApplicationRepository.ApplyJobAsync(userId, request, env);
            return StatusCode(result.Status, result);
        }

        // GET: api/applications/my-applications - Candidate get own applications
        [HttpGet("my-applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetMyApplications([FromQuery] ApplicationQueryParameters queryParams)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            PagedResult<JobApplicationDto> result = await jobApplicationRepository.GetCandidateApplicationsAsync(userId, queryParams);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/applications/{id} - Candidate withdraw application
        [HttpDelete("{id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> WithdrawApplication(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await jobApplicationRepository.WithdrawApplicationAsync(id, userId);
            return StatusCode(result.Status, result);
        }

        // GET: api/applications/check/{jobId} - Check if candidate already applied
        [HttpGet("check/{jobId}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> CheckIfApplied(int jobId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<bool> result = await jobApplicationRepository.CheckIfAppliedAsync(userId, jobId);
            return StatusCode(result.Status, result);
        }

        // ==========================================
        // EMPLOYER ENDPOINTS
        // ==========================================

        // GET: api/applications/employer/all - Employer get all applications
        [HttpGet("employer/all")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetAllApplications([FromQuery] ApplicationQueryParameters queryParams)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            PagedResult<JobApplicationDto> result = await jobApplicationRepository.GetJobApplicationsAsync(userId, queryParams);
            return StatusCode(result.Status, result);
        }

        // GET: api/applications/employer/job/{jobId} - Employer get applications for specific job
        [HttpGet("employer/job/{jobId}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetApplicationsByJob(int jobId, [FromQuery] ApplicationQueryParameters queryParams)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            PagedResult<JobApplicationDto> result = await jobApplicationRepository.GetApplicationsByJobAsync(jobId, userId, queryParams);
            return StatusCode(result.Status, result);
        }

        // GET: api/applications/employer/{id} - Employer get application detail
        [HttpGet("employer/{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetApplicationDetail(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<JobApplicationDto> result = await jobApplicationRepository.GetApplicationDetailAsync(id, userId);
            return StatusCode(result.Status, result);
        }

        // PATCH: api/applications/employer/{id}/status - Employer update application status
        [HttpPatch("employer/{id}/status")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] UpdateApplicationStatusRequest request)
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

            ApiResult<bool> result = await jobApplicationRepository.UpdateApplicationStatusAsync(id, userId, request);
            return StatusCode(result.Status, result);
        }

        // GET: api/applications/employer/statistics - Employer get statistics
        [HttpGet("employer/statistics")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetStatistics()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác thực người dùng." });
            }

            ApiResult<Dictionary<string, int>> result = await jobApplicationRepository.GetApplicationStatisticsAsync(userId);
            return StatusCode(result.Status, result);
        }
    }
}