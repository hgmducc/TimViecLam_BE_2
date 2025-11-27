using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IProfileRepository
    {
        Task<ProfileResult> GetProfileAsync(int userId);
        Task<ProfileResult> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<ProfileResult> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<ProfileResult> UpdateAvatarAsync(int userId, IFormFile avatar);
    }
}