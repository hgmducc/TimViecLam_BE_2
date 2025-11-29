using Microsoft.EntityFrameworkCore;
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

                // Filter by search term (job title, company name)
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
                    .Select(j => new JobPostingDto
                    {
                        JobPostingID = j.JobPostingID,
                        EmployerID = j.EmployerID,
                        CompanyName = j.Employer.CompanyName,
                        CompanyLogo = j.Employer.CompanyLogo,
                        JobTitle = j.JobTitle,
                        JobDescription = j.JobDescription,
                        Requirements = j.Requirements,
                        Benefits = j.Benefits,
                        SalaryMin = j.SalaryMin,
                        SalaryMax = j.SalaryMax,
                        SalaryType = j.SalaryType,
                        JobType = j.JobType,
                        Location = j.Location,
                        Industry = j.Industry,
                        ExperienceLevel = j.ExperienceLevel,
                        YearsOfExperienceRequired = j.YearsOfExperienceRequired,
                        EducationLevel = j.EducationLevel,
                        VacancyCount = j.VacancyCount,
                        ApplicationDeadline = j.ApplicationDeadline,
                        Status = j.Status,
                        CreatedAt = j.CreatedAt,
                        PublishedAt = j.PublishedAt,
                        ViewCount = j.ViewCount,
                        ApplicationCount = j.ApplicationCount
                    })
                    .ToListAsync();

                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách tin tuyển dụng thành công.",
                    Data = jobPostings,
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

                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thông tin tin tuyển dụng thành công.",
                    Data = new JobPostingDto
                    {
                        JobPostingID = jobPosting.JobPostingID,
                        EmployerID = jobPosting.EmployerID,
                        CompanyName = jobPosting.Employer.CompanyName,
                        CompanyLogo = jobPosting.Employer.CompanyLogo,
                        JobTitle = jobPosting.JobTitle,
                        JobDescription = jobPosting.JobDescription,
                        Requirements = jobPosting.Requirements,
                        Benefits = jobPosting.Benefits,
                        SalaryMin = jobPosting.SalaryMin,
                        SalaryMax = jobPosting.SalaryMax,
                        SalaryType = jobPosting.SalaryType,
                        JobType = jobPosting.JobType,
                        Location = jobPosting.Location,
                        Industry = jobPosting.Industry,
                        ExperienceLevel = jobPosting.ExperienceLevel,
                        YearsOfExperienceRequired = jobPosting.YearsOfExperienceRequired,
                        EducationLevel = jobPosting.EducationLevel,
                        VacancyCount = jobPosting.VacancyCount,
                        ApplicationDeadline = jobPosting.ApplicationDeadline,
                        Status = jobPosting.Status,
                        CreatedAt = jobPosting.CreatedAt,
                        PublishedAt = jobPosting.PublishedAt,
                        ViewCount = jobPosting.ViewCount,
                        ApplicationCount = jobPosting.ApplicationCount
                    }
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
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.JobPostings.AddAsync(jobPosting);
                await dbContext.SaveChangesAsync();

                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Tạo tin tuyển dụng thành công.",
                    Data = new JobPostingDto
                    {
                        JobPostingID = jobPosting.JobPostingID,
                        EmployerID = jobPosting.EmployerID,
                        JobTitle = jobPosting.JobTitle,
                        Status = jobPosting.Status,
                        CreatedAt = jobPosting.CreatedAt
                    }
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

        public async Task<ApiResult<JobPostingDto>> UpdateJobPostingAsync(int jobPostingId, int employerId, CreateJobPostingRequest request)
        {
            try
            {
                var jobPosting = await dbContext.JobPostings
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobPostingId && j.EmployerID == employerId);

                if (jobPosting == null)
                    return new ApiResult<JobPostingDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

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
                jobPosting.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ApiResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật tin tuyển dụng thành công."
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
                    .Select(j => new JobPostingDto
                    {
                        JobPostingID = j.JobPostingID,
                        EmployerID = j.EmployerID,
                        CompanyName = j.Employer.CompanyName,
                        JobTitle = j.JobTitle,
                        JobType = j.JobType,
                        Location = j.Location,
                        Status = j.Status,
                        CreatedAt = j.CreatedAt,
                        PublishedAt = j.PublishedAt,
                        ViewCount = j.ViewCount,
                        ApplicationCount = j.ApplicationCount
                    })
                    .ToListAsync();

                return new PagedResult<JobPostingDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách tin tuyển dụng thành công.",
                    Data = jobPostings,
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