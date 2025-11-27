using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> EmailExitAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneExitAsync(string phoneNumber)
        {
            return await _dbContext.Users.AnyAsync(u => u.Phone == phoneNumber);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Administrator)
                .Include(u => u.Employer)
                .Include(u => u.Candidate)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.Administrator)
                .Include(u => u.Employer)
                .Include(u => u.Candidate)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _dbContext.Users.FindAsync(id);
            if (existingUser == null)
                return null;

            existingUser.FullName = user.FullName;
            existingUser.Phone = user.Phone;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Gender = user.Gender;
            existingUser.Address = user.Address;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> UpdateUserStatusAsync(int id, string status)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return null;

            user.Status = status;
            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}