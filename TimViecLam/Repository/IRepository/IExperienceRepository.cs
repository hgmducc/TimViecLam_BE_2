using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IExperienceRepository
    {
        Task<ApiResult<List<ExperienceDto>>> GetExperiencesByCandidateAsync(int candidateId);
        Task<ApiResult<ExperienceDto>> AddExperienceAsync(int candidateId, AddExperienceRequest request);
        Task<ApiResult<ExperienceDto>> UpdateExperienceAsync(int experienceId, AddExperienceRequest request);
        Task<ApiResult<bool>> DeleteExperienceAsync(int experienceId, int candidateId);
    }
}