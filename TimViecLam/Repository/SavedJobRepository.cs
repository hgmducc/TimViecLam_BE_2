using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class SavedJobRepository : ISavedJobRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SavedJobRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ApiResult<SavedJobDto>> SaveJobAsync(int candidateId, int jobPostingId)
        {
            try
            {
                // Kiểm tra job posting có tồn tại không
                var jobPosting = await dbContext.JobPostings
                    .Include(j => j.Employer)
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobPostingId);

                if (jobPosting == null)
                    return new ApiResult<SavedJobDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                // Kiểm tra đã lưu chưa
                var existingSavedJob = await dbContext.SavedJobs
                    .FirstOrDefaultAsync(sj => sj.CandidateID == candidateId && sj.JobPostingID == jobPostingId);

                if (existingSavedJob != null)
                    return new ApiResult<SavedJobDto>
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "ALREADY_SAVED",
                        Message = "Bạn đã lưu tin tuyển dụng này rồi."
                    };

                // Tạo saved job
                var savedJob = new SavedJob
                {
                    CandidateID = candidateId,
                    JobPostingID = jobPostingId,
                    SavedAt = DateTime.UtcNow
                };

                await dbContext.SavedJobs.AddAsync(savedJob);
                await dbContext.SaveChangesAsync();

                return new ApiResult<SavedJobDto>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Lưu tin tuyển dụng thành công.",
                    Data = new SavedJobDto
                    {
                        SavedJobID = savedJob.SavedJobID,
                        JobPostingID = jobPosting.JobPostingID,
                        JobTitle = jobPosting.JobTitle,
                        CompanyName = jobPosting.Employer.CompanyName,
                        SavedAt = savedJob.SavedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<SavedJobDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> UnsaveJobAsync(int candidateId, int jobPostingId)
        {
            try
            {
                var savedJob = await dbContext.SavedJobs
                    .FirstOrDefaultAsync(sj => sj.CandidateID == candidateId && sj.JobPostingID == jobPostingId);

                if (savedJob == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "SAVED_JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin đã lưu."
                    };

                dbContext.SavedJobs.Remove(savedJob);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Bỏ lưu tin tuyển dụng thành công.",
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

        public async Task<PagedResult<SavedJobDto>> GetSavedJobsAsync(int candidateId, JobPostingQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.SavedJobs
                    .Include(sj => sj.JobPosting)
                        .ThenInclude(j => j.Employer)
                    .Where(sj => sj.CandidateID == candidateId)
                    .AsQueryable();

                // Filter by job status (chỉ lấy Active jobs)
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(sj => sj.JobPosting.Status == queryParams.Status);
                }

                // Filter by search term
                if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                {
                    query = query.Where(sj =>
                        sj.JobPosting.JobTitle.Contains(queryParams.SearchTerm) ||
                        sj.JobPosting.Employer.CompanyName.Contains(queryParams.SearchTerm));
                }

                // Filter by job type
                if (!string.IsNullOrEmpty(queryParams.JobType))
                {
                    query = query.Where(sj => sj.JobPosting.JobType == queryParams.JobType);
                }

                // Filter by location
                if (!string.IsNullOrEmpty(queryParams.Location))
                {
                    query = query.Where(sj => sj.JobPosting.Location.Contains(queryParams.Location));
                }

                int totalRecords = await query.CountAsync();

                // Get job IDs để check HasApplied
                var savedJobs = await query
                    .OrderByDescending(sj => sj.SavedAt)
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .Select(sj => new
                    {
                        sj.SavedJobID,
                        sj.JobPostingID,
                        sj.SavedAt,
                        JobPosting = sj.JobPosting
                    })
                    .ToListAsync();

                var jobPostingIds = savedJobs.Select(sj => sj.JobPostingID).ToList();

                // Check which jobs candidate has applied to
                var appliedJobIds = await dbContext.JobApplications
                    .Where(ja => ja.CandidateID == candidateId && jobPostingIds.Contains(ja.JobPostingID))
                    .Select(ja => ja.JobPostingID)
                    .ToListAsync();

                var result = savedJobs.Select(sj => new SavedJobDto
                {
                    SavedJobID = sj.SavedJobID,
                    JobPostingID = sj.JobPostingID,
                    JobTitle = sj.JobPosting.JobTitle,
                    CompanyName = sj.JobPosting.Employer.CompanyName,
                    CompanyLogo = sj.JobPosting.Employer.CompanyLogo,
                    Location = sj.JobPosting.Location,
                    JobType = sj.JobPosting.JobType,
                    SalaryMin = sj.JobPosting.SalaryMin,
                    SalaryMax = sj.JobPosting.SalaryMax,
                    SalaryType = sj.JobPosting.SalaryType,
                    ApplicationDeadline = sj.JobPosting.ApplicationDeadline,
                    Status = sj.JobPosting.Status,
                    SavedAt = sj.SavedAt,
                    HasApplied = appliedJobIds.Contains(sj.JobPostingID)
                }).ToList();

                return new PagedResult<SavedJobDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách việc làm đã lưu thành công.",
                    Data = result,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<SavedJobDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> CheckIfSavedAsync(int candidateId, int jobPostingId)
        {
            try
            {
                bool isSaved = await dbContext.SavedJobs
                    .AnyAsync(sj => sj.CandidateID == candidateId && sj.JobPostingID == jobPostingId);

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Data = isSaved
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<int>> GetSavedJobsCountAsync(int candidateId)
        {
            try
            {
                int count = await dbContext.SavedJobs
                    .CountAsync(sj => sj.CandidateID == candidateId);

                return new ApiResult<int>
                {
                    IsSuccess = true,
                    Status = 200,
                    Data = count
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<int>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }
    }
}