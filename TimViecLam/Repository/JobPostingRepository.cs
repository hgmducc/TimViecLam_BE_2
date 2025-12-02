using Microsoft.EntityFrameworkCore;
using System.Text.Json;  // ← THÊM DÒNG NÀY
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class JobPostingRepository : IJobPostingRepository
    {
        private readonly ApplicationDbContext dbContext;

        public JobPostingRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // ==========================================
        // HELPER METHOD: MAP TO DTO
        // ==========================================
        private JobPostingDto MapToDto(JobPosting job)
        {
            return new JobPostingDto
            {
                JobPostingID = job.JobPostingID,
                EmployerID = job.EmployerID,
                CompanyName = job.Employer.CompanyName,
                CompanyLogo = job.Employer.CompanyLogo,
                JobTitle = job.JobTitle,
                JobDescription = job.JobDescription,
                Requirements = job.Requirements,
                Benefits = job.Benefits,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                SalaryType = job.SalaryType,
                JobType = job.JobType,
                Location = job.Location,
                Industry = job.Industry,
                ExperienceLevel = job.ExperienceLevel,
                YearsOfExperienceRequired = job.YearsOfExperienceRequired,
                EducationLevel = job.EducationLevel,
                VacancyCount = job.VacancyCount,
                ApplicationDeadline = job.ApplicationDeadline,
                Status = job.Status,
                CreatedAt = job.CreatedAt,
                PublishedAt = job.PublishedAt,
                ViewCount = job.ViewCount,
                ApplicationCount = job.ApplicationCount,

                // ✅ NEW FIELDS:
                WorkingHours = job.WorkingHours,
                GenderRequirement = job.GenderRequirement,

                // Parse JSON to List<string>
                DetailedAddresses = string.IsNullOrWhiteSpace(job.DetailedLocations)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(job.DetailedLocations),

                RequiredSkills = string.IsNullOrWhiteSpace(job.RequiredSkills)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(job.RequiredSkills),

                Tags = string.IsNullOrWhiteSpace(job.Tags)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(job.Tags),

                CareerGrowth = job.CareerGrowth,
                ClosedAt = job.ClosedAt
            };
        }

        // ==========================================
        // GET ALL JOB POSTINGS
        // ==========================================
        public async Task<PagedResult<JobPostingDto>> GetAllJobPostingsAsync(JobPostingQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.JobPostings
                    .Include(j => j.Employer)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(j => j.Status == queryParams.Status);
                }

                // Filter by search term
                if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                {
                    query = query.Where(j =>
                        j.JobTitle.Contains(queryParams.SearchTerm) ||
                        j.Employer.CompanyName.Contains(queryParams.SearchTerm));
                }

                // Filter by job type
                if (!string.IsNullOrEmpty(queryParams.JobType))
                {
                    query = query.Where(j => j.JobType == queryParams.JobType);
                }

                // Filter by location
                if (!string.IsNullOrEmpty(queryParams.Location))
                {
                    query = query.Where(j => j.Location.Contains(queryParams.Location));
                }

                // Filter by industry
                if (!string.IsNullOrEmpty(queryParams.Industry))
                {
                    query = query.Where(j => j.Industry == queryParams.Industry);
                }

                // Filter by experience level
                if (!string.IsNullOrEmpty(queryParams.ExperienceLevel))
                {
                    query = query.Where(j => j.ExperienceLevel == queryParams.ExperienceLevel);
                }

                // Filter by salary
                if (queryParams.SalaryMin.HasValue)
                {
                    query = query.Where(j => j.SalaryMax >= queryParams.SalaryMin);
                }

                // Total count
                int totalRecords = await query.CountAsync();

                // Sorting
                query = queryParams.SortBy?.ToLower() switch
                {
                    "salary" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(j => j.SalaryMin)
                        : query.OrderByDescending(j => j.SalaryMax),
                    "deadline" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(j => j.ApplicationDeadline)
                        : query.OrderByDescending(j => j.ApplicationDeadline),
                    "viewcount" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(j => j.ViewCount)
                        : query.OrderByDescending(j => j.ViewCount),
                    _ => queryParams.SortOrder == "asc"
                        ? query.OrderBy(j => j.CreatedAt)
                        : query.OrderByDescending(j => j.CreatedAt)
                };

                // Pagination
                var jobPostings = await query
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .ToListAsync();

                var dtos = jobPostings.Select(j => MapToDto(j)).ToList();

                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách tin tuyển dụng thành công.",
                    Data = dtos,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // GET JOB BY ID
        // ==========================================
        public async Task<ApiResult<JobPostingDto>> GetJobPostingByIdAsync(int jobPostingId)
        {
            try
            {
                var jobPosting = await dbContext.JobPostings
                    .Include(j => j.Employer)
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobPostingId);

                if (jobPosting == null)
                    return new ApiResult<JobPostingDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                var dto = MapToDto(jobPosting);

                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thông tin tin tuyển dụng thành công.",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // CREATE JOB POSTING
        // ==========================================
        public async Task<ApiResult<JobPostingDto>> CreateJobPostingAsync(int employerId, CreateJobPostingRequest request)
        {
            try
            {
                var jobPosting = new JobPosting
                {
                    EmployerID = employerId,
                    JobTitle = request.JobTitle,
                    JobDescription = request.JobDescription,
                    Requirements = request.Requirements,
                    Benefits = request.Benefits,
                    SalaryMin = request.SalaryMin,
                    SalaryMax = request.SalaryMax,
                    SalaryType = request.SalaryType,
                    JobType = request.JobType,
                    Location = request.Location,
                    Industry = request.Industry,
                    ExperienceLevel = request.ExperienceLevel,
                    YearsOfExperienceRequired = request.YearsOfExperienceRequired,
                    EducationLevel = request.EducationLevel,
                    VacancyCount = request.VacancyCount,
                    ApplicationDeadline = request.ApplicationDeadline,

                    // ✅ NEW FIELDS:
                    WorkingHours = request.WorkingHours,
                    GenderRequirement = request.GenderRequirement,

                    // Serialize List<string> to JSON
                    DetailedLocations = request.DetailedAddresses != null && request.DetailedAddresses.Any()
                        ? JsonSerializer.Serialize(request.DetailedAddresses)
                        : null,

                    RequiredSkills = request.RequiredSkills != null && request.RequiredSkills.Any()
                        ? JsonSerializer.Serialize(request.RequiredSkills)
                        : null,

                    Tags = request.Tags != null && request.Tags.Any()
                        ? JsonSerializer.Serialize(request.Tags)
                        : null,

                    CareerGrowth = request.CareerGrowth,

                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.JobPostings.AddAsync(jobPosting);
                await dbContext.SaveChangesAsync();

                // Load employer for DTO
                await dbContext.Entry(jobPosting).Reference(j => j.Employer).LoadAsync();
                var dto = MapToDto(jobPosting);

                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Tạo tin tuyển dụng thành công.",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // UPDATE JOB POSTING
        // ==========================================
        public async Task<ApiResult<JobPostingDto>> UpdateJobPostingAsync(int jobPostingId, int employerId, CreateJobPostingRequest request)
        {
            try
            {
                var jobPosting = await dbContext.JobPostings
                    .Include(j => j.Employer)
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobPostingId && j.EmployerID == employerId);

                if (jobPosting == null)
                    return new ApiResult<JobPostingDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                // Update existing fields
                jobPosting.JobTitle = request.JobTitle;
                jobPosting.JobDescription = request.JobDescription;
                jobPosting.Requirements = request.Requirements;
                jobPosting.Benefits = request.Benefits;
                jobPosting.SalaryMin = request.SalaryMin;
                jobPosting.SalaryMax = request.SalaryMax;
                jobPosting.SalaryType = request.SalaryType;
                jobPosting.JobType = request.JobType;
                jobPosting.Location = request.Location;
                jobPosting.Industry = request.Industry;
                jobPosting.ExperienceLevel = request.ExperienceLevel;
                jobPosting.YearsOfExperienceRequired = request.YearsOfExperienceRequired;
                jobPosting.EducationLevel = request.EducationLevel;
                jobPosting.VacancyCount = request.VacancyCount;
                jobPosting.ApplicationDeadline = request.ApplicationDeadline;

                // ✅ UPDATE NEW FIELDS:
                jobPosting.WorkingHours = request.WorkingHours;
                jobPosting.GenderRequirement = request.GenderRequirement;

                jobPosting.DetailedLocations = request.DetailedAddresses != null && request.DetailedAddresses.Any()
                    ? JsonSerializer.Serialize(request.DetailedAddresses)
                    : null;

                jobPosting.RequiredSkills = request.RequiredSkills != null && request.RequiredSkills.Any()
                    ? JsonSerializer.Serialize(request.RequiredSkills)
                    : null;

                jobPosting.Tags = request.Tags != null && request.Tags.Any()
                    ? JsonSerializer.Serialize(request.Tags)
                    : null;

                jobPosting.CareerGrowth = request.CareerGrowth;

                jobPosting.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                var dto = MapToDto(jobPosting);

                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật tin tuyển dụng thành công.",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // DELETE JOB POSTING
        // ==========================================
        public async Task<ApiResult<bool>> DeleteJobPostingAsync(int jobPostingId, int employerId)
        {
            try
            {
                var jobPosting = await dbContext.JobPostings
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobPostingId && j.EmployerID == employerId);

                if (jobPosting == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                dbContext.JobPostings.Remove(jobPosting);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Xóa tin tuyển dụng thành công.",
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

        // ==========================================
        // UPDATE JOB STATUS
        // ==========================================
        public async Task<ApiResult<bool>> UpdateJobStatusAsync(int jobPostingId, int employerId, string status)
        {
            try
            {
                var jobPosting = await dbContext.JobPostings
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobPostingId && j.EmployerID == employerId);

                if (jobPosting == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                jobPosting.Status = status;

                if (status == "Active" && jobPosting.PublishedAt == null)
                {
                    jobPosting.PublishedAt = DateTime.UtcNow;
                }

                if (status == "Closed" && jobPosting.ClosedAt == null)
                {
                    jobPosting.ClosedAt = DateTime.UtcNow;
                }

                jobPosting.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Cập nhật trạng thái tin tuyển dụng thành '{status}' thành công.",
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

        // ==========================================
        // INCREMENT VIEW COUNT
        // ==========================================
        public async Task<ApiResult<bool>> IncrementViewCountAsync(int jobPostingId)
        {
            try
            {
                var jobPosting = await dbContext.JobPostings.FindAsync(jobPostingId);

                if (jobPosting == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                jobPosting.ViewCount++;
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Data = true
                };
            }
            catch
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    Data = false
                };
            }
        }

        // ==========================================
        // GET JOBS BY EMPLOYER
        // ==========================================
        public async Task<PagedResult<JobPostingDto>> GetJobPostingsByEmployerAsync(int employerId, JobPostingQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.JobPostings
                    .Include(j => j.Employer)
                    .Where(j => j.EmployerID == employerId)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(j => j.Status == queryParams.Status);
                }

                // Search
                if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                {
                    query = query.Where(j => j.JobTitle.Contains(queryParams.SearchTerm));
                }

                int totalRecords = await query.CountAsync();

                // Sorting & Pagination
                var jobPostings = await query
                    .OrderByDescending(j => j.CreatedAt)
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .ToListAsync();

                var dtos = jobPostings.Select(j => MapToDto(j)).ToList();

                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách tin tuyển dụng thành công.",
                    Data = dtos,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // GET RELATED JOBS (NEW)
        // ==========================================
        public async Task<ApiResult<List<JobPostingDto>>> GetRelatedJobsAsync(int jobId, int count)
        {
            try
            {
                var currentJob = await dbContext.JobPostings.FindAsync(jobId);

                if (currentJob == null)
                    return new ApiResult<List<JobPostingDto>>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                var relatedJobs = await dbContext.JobPostings
                    .Include(j => j.Employer)
                    .Where(j =>
                        j.JobPostingID != jobId &&
                        j.Status == "Active" &&
                        (j.Industry == currentJob.Industry ||
                         j.Location == currentJob.Location ||
                         j.JobType == currentJob.JobType))
                    .OrderByDescending(j => j.PublishedAt)
                    .Take(count)
                    .ToListAsync();

                var dtos = relatedJobs.Select(j => MapToDto(j)).ToList();

                return new ApiResult<List<JobPostingDto>>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách việc làm liên quan thành công.",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<JobPostingDto>>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // GET JOB COMPANY INFO (NEW)
        // ==========================================
        public async Task<ApiResult<EmployerProfileDto>> GetJobCompanyInfoAsync(int jobId)
        {
            try
            {
                var job = await dbContext.JobPostings
                    .Include(j => j.Employer)
                        .ThenInclude(e => e.JobPostings)
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobId);

                if (job == null)
                    return new ApiResult<EmployerProfileDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                var employerProfile = new EmployerProfileDto
                {
                    EmployerID = job.Employer.EmployerID,
                    CompanyName = job.Employer.CompanyName,
                    CompanyWebsite = job.Employer.CompanyWebsite,
                    CompanyDescription = job.Employer.CompanyDescription,
                    CompanyLogo = job.Employer.CompanyLogo,
                    CompanySize = job.Employer.CompanySize,
                    Industry = job.Employer.Industry,
                    CompanyAddress = job.Employer.CompanyAddress,
                    VerificationStatus = job.Employer.VerificationStatus,
                    ContactPerson = job.Employer.ContactPerson,
                    ContactEmail = job.Employer.ContactEmail,
                    ContactPhone = job.Employer.ContactPhone,
                    VerifiedAt = job.Employer.VerifiedAt,
                    TotalJobPostings = job.Employer.JobPostings.Count,
                    ActiveJobPostings = job.Employer.JobPostings.Count(j => j.Status == "Active")
                };

                return new ApiResult<EmployerProfileDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thông tin công ty thành công.",
                    Data = employerProfile
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<EmployerProfileDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        // ==========================================
        // GET JOBS BY TAG (NEW)
        // ==========================================
        public async Task<PagedResult<JobPostingDto>> GetJobsByTagAsync(string tagName, JobPostingQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.JobPostings
                    .Include(j => j.Employer)
                    .Where(j => j.Status == "Active");

                // Filter by tag (JSON contains)
                if (!string.IsNullOrWhiteSpace(tagName))
                {
                    query = query.Where(j =>
                        j.Tags != null &&
                        j.Tags.Contains($"\"{tagName}\""));
                }

                // Apply other filters
                if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
                {
                    query = query.Where(j =>
                        j.JobTitle.Contains(queryParams.SearchTerm) ||
                        j.JobDescription.Contains(queryParams.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(queryParams.JobType))
                    query = query.Where(j => j.JobType == queryParams.JobType);

                if (!string.IsNullOrWhiteSpace(queryParams.Location))
                    query = query.Where(j => j.Location.Contains(queryParams.Location));

                if (!string.IsNullOrWhiteSpace(queryParams.Industry))
                    query = query.Where(j => j.Industry == queryParams.Industry);

                // Sorting
                query = queryParams.SortBy?.ToLower() switch
                {
                    "salary" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(j => j.SalaryMin)
                        : query.OrderByDescending(j => j.SalaryMax),
                    _ => queryParams.SortOrder == "asc"
                        ? query.OrderBy(j => j.CreatedAt)
                        : query.OrderByDescending(j => j.CreatedAt)
                };

                var totalRecords = await query.CountAsync();

                var jobs = await query
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .ToListAsync();

                var dtos = jobs.Select(j => MapToDto(j)).ToList();

                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Lấy danh sách việc làm theo tag '{tagName}' thành công.",
                    Data = dtos,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }
    }
}