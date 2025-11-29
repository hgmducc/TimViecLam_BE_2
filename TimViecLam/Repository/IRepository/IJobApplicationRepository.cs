using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IJobApplicationRepository
    {
        // Candidate operations
        Task<ApiResult<JobApplicationDto>> ApplyJobAsync(int candidateId, ApplyJobRequest request, IWebHostEnvironment env);
        Task<PagedResult<JobApplicationDto>> GetCandidateApplicationsAsync(int candidateId, ApplicationQueryParameters queryParams);
        Task<ApiResult<bool>> WithdrawApplicationAsync(int applicationId, int candidateId);
        Task<ApiResult<bool>> CheckIfAppliedAsync(int candidateId, int jobPostingId);

        // Employer operations
        Task<PagedResult<JobApplicationDto>> GetJobApplicationsAsync(int employerId, ApplicationQueryParameters queryParams);
        Task<PagedResult<JobApplicationDto>> GetApplicationsByJobAsync(int jobPostingId, int employerId, ApplicationQueryParameters queryParams);
        Task<ApiResult<JobApplicationDto>> GetApplicationDetailAsync(int applicationId, int employerId);
        Task<ApiResult<bool>> UpdateApplicationStatusAsync(int applicationId, int employerId, UpdateApplicationStatusRequest request);

        // Statistics
        Task<ApiResult<Dictionary<string, int>>> GetApplicationStatisticsAsync(int employerId);
    }
}