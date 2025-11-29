using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface ISavedJobRepository
    {
        Task<ApiResult<SavedJobDto>> SaveJobAsync(int candidateId, int jobPostingId);
        Task<ApiResult<bool>> UnsaveJobAsync(int candidateId, int jobPostingId);
        Task<PagedResult<SavedJobDto>> GetSavedJobsAsync(int candidateId, JobPostingQueryParameters queryParams);
        Task<ApiResult<bool>> CheckIfSavedAsync(int candidateId, int jobPostingId);
        Task<ApiResult<int>> GetSavedJobsCountAsync(int candidateId);
    }
}