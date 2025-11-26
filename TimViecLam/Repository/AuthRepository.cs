using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;
using TimViecLam.Service;

namespace TimViecLam.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfiguration configuration;
        private readonly string secretKey;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService emailService;

        public AuthRepository(ApplicationDbContext dbContext, IConfiguration configuration, IWebHostEnvironment env, IEmailService emailService)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this._env = env;
            this.emailService = emailService;
            secretKey = configuration.GetValue<string>("ApiSetting:Secret");
        }


        //đăng nhập
        public async Task<AuthResult> LoginAsync(LoginRequest requestDto)
        {
            try
            {
                var user = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == requestDto.Username || u.Phone == requestDto.Username);

                if (user == null)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 401,
                        ErrorCode = "INVALID_CREDENTIALS",
                        Message = "Tài khoản hoặc mật khẩu không chính xác."
                    };

                // --- Chỉ cho phép đăng nhập nếu trạng thái Active ---
                if (user.Status != "Active")
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 403,
                        ErrorCode = "ACCOUNT_NOT_ACTIVE",
                        Message = "Tài khoản của bạn hiện không ở trạng thái hoạt động."
                    };
                }

                // --- Đăng nhập bằng Google ---
                if (user.IsGoogleAccount)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 400,
                        ErrorCode = "GOOGLE_LOGIN_REQUIRED",
                        Message = "Tài khoản này đăng ký bằng Google. Vui lòng đăng nhập bằng Google."
                    };
                }

                // --- Kiểm tra mật khẩu ---
                if (!BCrypt.Net.BCrypt.Verify(requestDto.Password, user.PasswordHash))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 401,
                        ErrorCode = "INVALID_CREDENTIALS",
                        Message = "Tài khoản hoặc mật khẩu không chính xác."
                    };
                }

                // --- Sinh token ---
                var key = Encoding.UTF8.GetBytes(secretKey);
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(configuration["ApiSetting:ExpiresInMinutes"])),
                    Issuer = configuration["ApiSetting:Issuer"],
                    Audience = configuration["ApiSetting:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                string jwtToken = tokenHandler.WriteToken(token);

                return new AuthResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Đăng nhập thành công.",
                    Token = jwtToken
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi máy chủ: " + ex.Message
                };
            }
        }


        public async Task<AuthResult> RegisterAdminAsync(RegisterAdminRequest requestDto)
        {
            // Kiểm tra email đã tồn tại chưa
            if (await dbContext.Users.AnyAsync(u => u.Email == requestDto.Email))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 400,
                    Message = "Email đã được đăng ký."
                };
            }

            // Tạo User
            var user = new User
            {
                FullName = requestDto.FullName,
                Email = requestDto.Email,
                Phone = requestDto.Phone,
                Gender = requestDto.Gender,
                DateOfBirth = requestDto.DateOfBirth,
                Address = requestDto.Address,
                Role = "Admin",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(requestDto.Password)
            };

            // Tạo Administrator liên kết
            var admin = new Administrator
            {
                AdminRole = requestDto.AdminRole,
                User = user
            };

            dbContext.Users.Add(user);
            dbContext.Administrators.Add(admin);
            await dbContext.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Status = 201,
                Message = "Đăng ký Admin thành công."
            };
        }



        //đăng kí ứng viên
        public async Task<AuthResult> RegisterCandidateAsync(RegisterCandidateRequest requestDto)
        {
            try
            {
                bool emailExists = await dbContext.Users.AnyAsync(u => u.Email == requestDto.Email);
                if (emailExists)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "EMAIL_EXISTS",
                        Message = "Địa chỉ email này đã được đăng ký."
                    };

                bool phoneExists = await dbContext.Users.AnyAsync(u => u.Phone == requestDto.PhoneNumber);
                if (phoneExists)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "PHONE_EXISTS",
                        Message = "Số điện thoại này đã được đăng ký."
                    };

                string hashed = BCrypt.Net.BCrypt.HashPassword(requestDto.Password, workFactor: 12);

                var newUser = new User
                {
                    FullName = requestDto.FullName,
                    Email = requestDto.Email,
                    Phone = requestDto.PhoneNumber,
                    PasswordHash = hashed,
                    DateOfBirth = requestDto.DateOfBirth,
                    Gender = requestDto.Gender,
                    Address = requestDto.address,
                    Role = "Candidate",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    IsGoogleAccount = false
                };

                var newCandidate = new Candidate();
                newUser.Candidate = newCandidate;

                await dbContext.Users.AddAsync(newUser);
                await dbContext.SaveChangesAsync();

                return new AuthResult
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Đăng ký thành công."
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi máy chủ: " + ex.Message
                };
            }
        }

        // đăng kí nhà tuyển dụng
        public async Task<AuthResult> RegisterEmployerAsync(RegisterEmployerRequest requestDto)
        {
            try
            {
                // --- Kiểm tra email ---
                bool emailExists = await dbContext.Users.AnyAsync(u => u.Email == requestDto.Email);
                if (emailExists)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "EMAIL_EXISTS",
                        Message = "Email này đã được đăng ký."
                    };

                // --- Kiểm tra số điện thoại ---
                bool phoneExists = await dbContext.Users.AnyAsync(u => u.Phone == requestDto.Phone);
                if (phoneExists)
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "PHONE_EXISTS",
                        Message = "Số điện thoại này đã được đăng ký."
                    };

                // --- Hash password ---
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDto.Password, workFactor: 12);

                // --- Upload file ---
                string? savedLicenseFilePath = null;

                if (requestDto.BusinessLicenseFile != null && requestDto.BusinessLicenseFile.Length > 0)
                {
                    // fallback nếu WebRootPath null
                    string rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string folderPath = Path.Combine(rootPath, "Uploads", "BusinessLicenses");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fileName = $"{Guid.NewGuid()}_{requestDto.BusinessLicenseFile.FileName}";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await requestDto.BusinessLicenseFile.CopyToAsync(stream);
                    }

                    // đường dẫn trả về frontend
                    savedLicenseFilePath = $"/Uploads/BusinessLicenses/{fileName}";
                }

                // --- Tạo User ---
                var newUser = new User
                {
                    FullName = requestDto.FullName,
                    Email = requestDto.Email,
                    Phone = requestDto.Phone,
                    PasswordHash = hashedPassword,
                    Role = "Employer",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    IsGoogleAccount = false
                };

                // --- Tạo Employer ---
                var newEmployer = new Employer
                {
                    CompanyName = requestDto.CompanyName,
                    CompanyWebsite = requestDto.CompanyWebsite,
                    CompanyDescription = "",
                    TaxCode = requestDto.TaxCode,
                    BusinessLicenseFile = savedLicenseFilePath,
                    BusinessLicenseNumber = null,
                    VerificationStatus = "Pending",
                    ContactPerson = requestDto.ContactPerson ?? requestDto.FullName,
                    ContactEmail = requestDto.ContactEmail ?? requestDto.Email,
                    ContactPhone = requestDto.ContactPhone ?? requestDto.Phone
                };

                newUser.Employer = newEmployer;

                await dbContext.Users.AddAsync(newUser);
                await dbContext.SaveChangesAsync();

                return new AuthResult
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Đăng ký nhà tuyển dụng thành công, vui lòng chờ xét duyệt giấy phép kinh doanh."
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }


        // --- Forgot Password ---
        public async Task<AuthResult> ForgotPasswordAsync(ForgotPasswordRequest requestDto)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == requestDto.Email);

            if (user == null)
            {
                // Email chưa đăng ký
                return new AuthResult
                {
                    IsSuccess = false,
                    Status = 404,
                    Message = "Email chưa đăng ký tài khoản."
                };
            }

            // Email tồn tại -  tạo token reset
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            await dbContext.SaveChangesAsync();

            var resetLink = $"https://trangdoimak.com/reset-password?token={user.PasswordResetToken}";

            string emailBody = $@"
            <div style='font-family: Arial, sans-serif; line-height: 1.5;'>
                <h2>Đặt lại mật khẩu</h2>
                <p>Chào {user.FullName},</p>
                <p>Bạn hoặc ai đó đã yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                <p>Nhấn nút bên dưới để đặt lại mật khẩu. Link này sẽ hết hạn sau 15 phút.</p>
                <a href='{resetLink}' 
                   style='display: inline-block; padding: 12px 24px; margin: 10px 0; 
                          font-size: 16px; color: white; background-color: #007BFF; 
                          text-decoration: none; border-radius: 6px;'>Đặt lại mật khẩu</a>
                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
                <p>Trân trọng,<br/>TimViecLam Team</p>
            </div>";

            await emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu TimViecLam", emailBody);

            return new AuthResult
            {
                IsSuccess = true,
                Status = 200,
                Message = "Link đặt lại mật khẩu đã được gửi đến email của bạn."
            };
        }


        // --- Reset Password ---
        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordRequest requestDto)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == requestDto.Token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return new AuthResult { IsSuccess = false, Status = 400, Message = "Token không hợp lệ hoặc đã hết hạn." };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(requestDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await dbContext.SaveChangesAsync();
            return new AuthResult { IsSuccess = true, Status = 200, Message = "Đặt lại mật khẩu thành công." };
        }
    }
}
