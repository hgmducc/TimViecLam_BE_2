using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IEducationRepository
    {
        Task<ApiResult<List<EducationDto>>> GetEducationsByCandidateAsync(int candidateId);
        Task<ApiResult<EducationDto>> AddEducationAsync(int candidateId, AddEducationRequest request);
        Task<ApiResult<EducationDto>> UpdateEducationAsync(int educationId, AddEducationRequest request);
        Task<ApiResult<bool>> DeleteEducationAsync(int educationId, int candidateId);
    }
}