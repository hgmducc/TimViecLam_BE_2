using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment env;

        public ProfileRepository(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            this.dbContext = dbContext;
            this.env = env;
        }

        // Lấy profile user
        public async Task<ProfileResult> GetProfileAsync(int userId)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(userId);

                if (user == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "USER_NOT_FOUND",
                        Message = "Không tìm thấy thông tin người dùng."
                    };

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thông tin profile thành công.",
                    Data = new ProfileResponse
                    {
                        UserID = user.UserID,
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Address = user.Address,
                        Role = user.Role,
                        AvatarUrl = user.Avatar,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ProfileResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // Cập nhật profile
        public async Task<ProfileResult> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(userId);

                if (user == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "USER_NOT_FOUND",
                        Message = "Không tìm thấy người dùng."
                    };

                // Kiểm tra số điện thoại trùng
                if (!string.IsNullOrEmpty(request.Phone) && request.Phone != user.Phone)
                {
                    bool phoneExists = await dbContext.Users.AnyAsync(u => u.Phone == request.Phone);
                    if (phoneExists)
                        return new ProfileResult
                        {
                            IsSuccess = false,
                            Status = 409,
                            ErrorCode = "PHONE_EXISTS",
                            Message = "Số điện thoại đã được sử dụng bởi tài khoản khác."
                        };
                }

                // Cập nhật thông tin
                user.FullName = request.FullName;
                user.Phone = request.Phone;
                user.DateOfBirth = request.DateOfBirth;
                user.Gender = request.Gender;
                user.Address = request.Address;
                user.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật profile thành công.",
                    Data = new ProfileResponse
                    {
                        UserID = user.UserID,
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Address = user.Address,
                        Role = user.Role,
                        AvatarUrl = user.Avatar,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ProfileResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // Đổi mật khẩu
        public async Task<ProfileResult> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(userId);

                if (user == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "USER_NOT_FOUND",
                        Message = "Không tìm thấy người dùng."
                    };

                // Kiểm tra mật khẩu hiện tại
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 400,
                        ErrorCode = "INVALID_PASSWORD",
                        Message = "Mật khẩu hiện tại không chính xác."
                    };

                // Cập nhật mật khẩu mới
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
                user.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Đổi mật khẩu thành công."
                };
            }
            catch (Exception ex)
            {
                return new ProfileResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // Cập nhật avatar
        public async Task<ProfileResult> UpdateAvatarAsync(int userId, IFormFile avatar)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(userId);

                if (user == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "USER_NOT_FOUND",
                        Message = "Không tìm thấy người dùng."
                    };

                // --- Upload file giống RegisterEmployer ---
                string? savedAvatarPath = null;

                if (avatar != null && avatar.Length > 0)
                {
                    string rootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string folderPath = Path.Combine(rootPath, "Uploads", "Avatar");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // Xóa avatar cũ nếu có
                    if (!string.IsNullOrEmpty(user.Avatar))
                    {
                        var oldAvatarFullPath = Path.Combine(rootPath, user.Avatar.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (File.Exists(oldAvatarFullPath))
                        {
                            File.Delete(oldAvatarFullPath);
                        }
                    }

                    string fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(stream);
                    }

                    savedAvatarPath = $"/Uploads/Avatar/{fileName}";
                }

                // Cập nhật database
                user.Avatar = savedAvatarPath;
                user.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật avatar thành công.",
                    Data = new ProfileResponse
                    {
                        UserID = user.UserID,
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Address = user.Address,
                        Role = user.Role,
                        AvatarUrl = savedAvatarPath,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ProfileResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }
    }
}