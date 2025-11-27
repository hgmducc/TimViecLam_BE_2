using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IUserRepository 
    {
        Task<bool> EmailExitAsync(string email);
        Task<bool> PhoneExitAsync(string phoneNumber);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> UpdateUserStatusAsync(int id, string status);
    }
}
