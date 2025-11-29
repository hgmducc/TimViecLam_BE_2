using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IEmployerRepository
    {
        Task<ProfileResult> GetEmployerProfileAsync(int employerId);
        Task<ProfileResult> UpdateEmployerProfileAsync(int employerId, UpdateEmployerProfileRequest request);
        Task<ApiResult<string>> UpdateBusinessLicenseAsync(int employerId, IFormFile licenseFile, IWebHostEnvironment env);
        Task<ApiResult<bool>> UpdateVerificationStatusAsync(int employerId, string status, string? notes);
        Task<PagedResult<ProfileResponse>> GetAllEmployersAsync(UserQueryParameters queryParams);
    }
}