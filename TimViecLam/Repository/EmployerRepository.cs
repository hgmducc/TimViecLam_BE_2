using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class EmployerRepository : IEmployerRepository
    {
        private readonly ApplicationDbContext dbContext;

        public EmployerRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ProfileResult> GetEmployerProfileAsync(int employerId)
        {
            try
            {
                var employer = await dbContext.Employers
                    .Include(e => e.User)
                    .Include(e => e.JobPostings)
                    .FirstOrDefaultAsync(e => e.EmployerID == employerId);

                if (employer == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EMPLOYER_NOT_FOUND",
                        Message = "Không tìm thấy nhà tuyển dụng."
                    };

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thông tin nhà tuyển dụng thành công.",
                    Data = new ProfileResponse
                    {
                        UserID = employer.User.UserID,
                        FullName = employer.User.FullName,
                        Email = employer.User.Email,
                        Phone = employer.User.Phone,
                        DateOfBirth = employer.User.DateOfBirth,
                        Gender = employer.User.Gender,
                        Address = employer.User.Address,
                        Role = employer.User.Role,
                        AvatarUrl = employer.User.Avatar,
                        IsGoogleAccount = employer.User.IsGoogleAccount,
                        Status = employer.User.Status,
                        CreatedAt = employer.User.CreatedAt,
                        UpdatedAt = employer.User.UpdatedAt,
                        EmployerProfile = new EmployerProfileDto
                        {
                            EmployerID = employer.EmployerID,
                            CompanyName = employer.CompanyName,
                            CompanyWebsite = employer.CompanyWebsite,
                            CompanyDescription = employer.CompanyDescription,
                            TaxCode = employer.TaxCode,
                            BusinessLicenseNumber = employer.BusinessLicenseNumber,
                            BusinessLicenseFile = employer.BusinessLicenseFile,
                            VerificationStatus = employer.VerificationStatus,
                            ContactPerson = employer.ContactPerson,
                            ContactEmail = employer.ContactEmail,
                            ContactPhone = employer.ContactPhone,
                            VerifiedAt = employer.VerifiedAt,
                            CompanyAddress = employer.CompanyAddress,
                            TotalJobPostings = employer.JobPostings.Count,
                            ActiveJobPostings = employer.JobPostings.Count(j => j.Status == "Active")
                        }
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

        public async Task<ProfileResult> UpdateEmployerProfileAsync(int employerId, UpdateEmployerProfileRequest request)
        {
            try
            {
                var employer = await dbContext.Employers.FindAsync(employerId);

                if (employer == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EMPLOYER_NOT_FOUND",
                        Message = "Không tìm thấy nhà tuyển dụng."
                    };

                employer.CompanyName = request.CompanyName;
                employer.CompanyWebsite = request.CompanyWebsite;
                employer.CompanyDescription = request.CompanyDescription;
                employer.TaxCode = request.TaxCode;
                employer.ContactPerson = request.ContactPerson;
                employer.ContactEmail = request.ContactEmail;
                employer.ContactPhone = request.ContactPhone;
                employer.CompanyAddress = request.CompanyAddress;
                employer.LastUpdated = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật thông tin công ty thành công."
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

        public async Task<ApiResult<string>> UpdateBusinessLicenseAsync(int employerId, IFormFile licenseFile, IWebHostEnvironment env)
        {
            try
            {
                var employer = await dbContext.Employers.FindAsync(employerId);

                if (employer == null)
                    return new ApiResult<string>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EMPLOYER_NOT_FOUND",
                        Message = "Không tìm thấy nhà tuyển dụng."
                    };

                // Upload file
                string rootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string folderPath = Path.Combine(rootPath, "Uploads", "BusinessLicenses");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Xóa file cũ nếu có
                if (!string.IsNullOrEmpty(employer.BusinessLicenseFile))
                {
                    var oldFilePath = Path.Combine(rootPath, employer.BusinessLicenseFile.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                string fileName = $"{employerId}_{Guid.NewGuid()}{Path.GetExtension(licenseFile.FileName)}";
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await licenseFile.CopyToAsync(stream);
                }

                string savedPath = $"/Uploads/BusinessLicenses/{fileName}";
                employer.BusinessLicenseFile = savedPath;
                employer.VerificationStatus = "Pending"; // Reset về Pending khi upload lại
                employer.LastUpdated = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ApiResult<string>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật giấy phép kinh doanh thành công.",
                    Data = savedPath
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<string>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> UpdateVerificationStatusAsync(int employerId, string status, string? notes)
        {
            try
            {
                var employer = await dbContext.Employers.FindAsync(employerId);

                if (employer == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EMPLOYER_NOT_FOUND",
                        Message = "Không tìm thấy nhà tuyển dụng."
                    };

                employer.VerificationStatus = status;

                if (status == "Verified")
                {
                    employer.VerifiedAt = DateTime.UtcNow;
                }

                employer.LastUpdated = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Cập nhật trạng thái xác minh thành '{status}' thành công.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<PagedResult<ProfileResponse>> GetAllEmployersAsync(UserQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.Employers
                    .Include(e => e.User)
                    .Include(e => e.JobPostings)
                    .AsQueryable();

                // Filter by search term
                if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                {
                    query = query.Where(e =>
                        e.CompanyName.Contains(queryParams.SearchTerm) ||
                        e.User.FullName.Contains(queryParams.SearchTerm) ||
                        e.User.Email.Contains(queryParams.SearchTerm));
                }

                // Filter by status
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(e => e.User.Status == queryParams.Status);
                }

                // Total count
                int totalRecords = await query.CountAsync();

                // Sorting
                query = queryParams.SortBy?.ToLower() switch
                {
                    "companyname" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(e => e.CompanyName)
                        : query.OrderByDescending(e => e.CompanyName),
                    "verificationstatus" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(e => e.VerificationStatus)
                        : query.OrderByDescending(e => e.VerificationStatus),
                    _ => queryParams.SortOrder == "asc"
                        ? query.OrderBy(e => e.User.CreatedAt)
                        : query.OrderByDescending(e => e.User.CreatedAt)
                };

                // Pagination
                var employers = await query
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .Select(e => new ProfileResponse
                    {
                        UserID = e.User.UserID,
                        FullName = e.User.FullName,
                        Email = e.User.Email,
                        Phone = e.User.Phone,

                        // ✅ THÊM 3 FIELDS NÀY (đã có từ fix trước):
                        DateOfBirth = e.User.DateOfBirth,
                        Gender = e.User.Gender,
                        Address = e.User.Address,

                        Role = e.User.Role,
                        Status = e.User.Status,
                        CreatedAt = e.User.CreatedAt,
                        EmployerProfile = new EmployerProfileDto
                        {
                            EmployerID = e.EmployerID,
                            CompanyName = e.CompanyName,
                            CompanyWebsite = e.CompanyWebsite,

                            // ✅ THÊM TẤT CẢ FIELDS CÒN THIẾU:
                            CompanyDescription = e.CompanyDescription,
                            CompanyLogo = e.CompanyLogo,
                            CompanySize = e.CompanySize,
                            Industry = e.Industry,
                            TaxCode = e.TaxCode,
                            BusinessLicenseNumber = e.BusinessLicenseNumber,
                            BusinessLicenseFile = e.BusinessLicenseFile,

                            VerificationStatus = e.VerificationStatus,
                            VerifiedAt = e.VerifiedAt,

                            ContactPerson = e.ContactPerson,
                            ContactEmail = e.ContactEmail,
                            ContactPhone = e.ContactPhone,
                            CompanyAddress = e.CompanyAddress,

                            TotalJobPostings = e.JobPostings.Count,
                            ActiveJobPostings = e.JobPostings.Count(j => j.Status == "Active")
                        }
                    })
                    .ToListAsync();

                return new PagedResult<ProfileResponse>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách nhà tuyển dụng thành công.",
                    Data = employers,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<ProfileResponse>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi:  " + ex.Message
                };
            }
        }
    }
}