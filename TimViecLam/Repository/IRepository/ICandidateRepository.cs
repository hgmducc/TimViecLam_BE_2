using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface ICandidateRepository
    {
        Task<ProfileResult> GetCandidateProfileAsync(int candidateId);
        Task<ProfileResult> UpdateCandidateProfileAsync(int candidateId, UpdateCandidateProfileRequest request);
        Task<int> CalculateProfileCompletenessAsync(int candidateId);
        Task<bool> UpdateSkillsAsync(int candidateId, List<string> skills);
    }
}