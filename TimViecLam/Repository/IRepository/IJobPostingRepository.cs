using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IJobPostingRepository
    {
        Task<PagedResult<JobPostingDto>> GetAllJobPostingsAsync(JobPostingQueryParameters queryParams);
        Task<ApiResult<JobPostingDto>> GetJobPostingByIdAsync(int jobPostingId);
        Task<ApiResult<JobPostingDto>> CreateJobPostingAsync(int employerId, CreateJobPostingRequest request);
        Task<ApiResult<JobPostingDto>> UpdateJobPostingAsync(int jobPostingId, int employerId, CreateJobPostingRequest request);
        Task<ApiResult<bool>> DeleteJobPostingAsync(int jobPostingId, int employerId);
        Task<ApiResult<bool>> UpdateJobStatusAsync(int jobPostingId, int employerId, string status);
        Task<ApiResult<bool>> IncrementViewCountAsync(int jobPostingId);
        Task<PagedResult<JobPostingDto>> GetJobPostingsByEmployerAsync(int employerId, JobPostingQueryParameters queryParams);
        Task<ApiResult<List<JobPostingDto>>> GetRelatedJobsAsync(int jobId, int count);
        Task<ApiResult<EmployerProfileDto>> GetJobCompanyInfoAsync(int jobId);
        Task<PagedResult<JobPostingDto>> GetJobsByTagAsync(string tagName, JobPostingQueryParameters queryParams);

    }
}