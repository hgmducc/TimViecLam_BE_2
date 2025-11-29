using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public JobApplicationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // ==========================================
        // CANDIDATE OPERATIONS
        // ==========================================

        public async Task<ApiResult<JobApplicationDto>> ApplyJobAsync(int candidateId, ApplyJobRequest request, IWebHostEnvironment env)
        {
            try
            {
                // Kiểm tra job posting có tồn tại và đang active không
                var jobPosting = await dbContext.JobPostings
                    .Include(j => j.Employer)
                    .FirstOrDefaultAsync(j => j.JobPostingID == request.JobPostingID);

                if (jobPosting == null)
                    return new ApiResult<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "JOB_NOT_FOUND",
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                if (jobPosting.Status != "Active")
                    return new ApiResult<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Status = 400,
                        ErrorCode = "JOB_NOT_ACTIVE",
                        Message = "Tin tuyển dụng không còn nhận ứng tuyển."
                    };

                // Kiểm tra deadline
                if (jobPosting.ApplicationDeadline.HasValue && jobPosting.ApplicationDeadline.Value < DateOnly.FromDateTime(DateTime.UtcNow))
                    return new ApiResult<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Status = 400,
                        ErrorCode = "DEADLINE_PASSED",
                        Message = "Đã hết hạn ứng tuyển."
                    };

                // Kiểm tra đã ứng tuyển chưa
                var existingApplication = await dbContext.JobApplications
                    .FirstOrDefaultAsync(ja => ja.CandidateID == candidateId && ja.JobPostingID == request.JobPostingID);

                if (existingApplication != null)
                    return new ApiResult<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Status = 409,
                        ErrorCode = "ALREADY_APPLIED",
                        Message = "Bạn đã ứng tuyển vị trí này rồi."
                    };

                // Lấy CV từ profile nếu không upload CV riêng
                var candidate = await dbContext.Candidates
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CandidateID == candidateId);

                string? cvPath = candidate?.CVFilePath;

                // Upload CV riêng nếu có
                if (request.CVFile != null && request.CVFile.Length > 0)
                {
                    string rootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string folderPath = Path.Combine(rootPath, "Uploads", "ApplicationCVs");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fileName = $"{candidateId}_{Guid.NewGuid()}{Path.GetExtension(request.CVFile.FileName)}";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.CVFile.CopyToAsync(stream);
                    }

                    cvPath = $"/Uploads/ApplicationCVs/{fileName}";
                }

                // Tạo đơn ứng tuyển
                var application = new JobApplication
                {
                    JobPostingID = request.JobPostingID,
                    CandidateID = candidateId,
                    CVFilePath = cvPath,
                    CoverLetter = request.CoverLetter,
                    Status = "Submitted",
                    AppliedAt = DateTime.UtcNow
                };

                await dbContext.JobApplications.AddAsync(application);

                // Tăng ApplicationCount của JobPosting
                jobPosting.ApplicationCount++;

                await dbContext.SaveChangesAsync();

                return new ApiResult<JobApplicationDto>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Ứng tuyển thành công.",
                    Data = new JobApplicationDto
                    {
                        ApplicationID = application.ApplicationID,
                        JobPostingID = application.JobPostingID,
                        JobTitle = jobPosting.JobTitle,
                        CompanyName = jobPosting.Employer.CompanyName,
                        Status = application.Status,
                        AppliedAt = application.AppliedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<JobApplicationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<PagedResult<JobApplicationDto>> GetCandidateApplicationsAsync(int candidateId, ApplicationQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.JobApplications
                    .Include(ja => ja.JobPosting)
                        .ThenInclude(j => j.Employer)
                    .Where(ja => ja.CandidateID == candidateId)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(ja => ja.Status == queryParams.Status);
                }

                int totalRecords = await query.CountAsync();

                // Sorting
                query = queryParams.SortBy?.ToLower() switch
                {
                    "status" => queryParams.SortOrder == "asc"
                        ? query.OrderBy(ja => ja.Status)
                        : query.OrderByDescending(ja => ja.Status),
                    _ => queryParams.SortOrder == "asc"
                        ? query.OrderBy(ja => ja.AppliedAt)
                        : query.OrderByDescending(ja => ja.AppliedAt)
                };

                // Pagination
                var applications = await query
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .Select(ja => new JobApplicationDto
                    {
                        ApplicationID = ja.ApplicationID,
                        JobPostingID = ja.JobPostingID,
                        JobTitle = ja.JobPosting.JobTitle,
                        CompanyName = ja.JobPosting.Employer.CompanyName,
                        CompanyLogo = ja.JobPosting.Employer.CompanyLogo,
                        Location = ja.JobPosting.Location,
                        JobType = ja.JobPosting.JobType,
                        CVFilePath = ja.CVFilePath,
                        CoverLetter = ja.CoverLetter,
                        Status = ja.Status,
                        AppliedAt = ja.AppliedAt,
                        ReviewedAt = ja.ReviewedAt
                    })
                    .ToListAsync();

                return new PagedResult<JobApplicationDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách đơn ứng tuyển thành công.",
                    Data = applications,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<JobApplicationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> WithdrawApplicationAsync(int applicationId, int candidateId)
        {
            try
            {
                var application = await dbContext.JobApplications
                    .Include(ja => ja.JobPosting)
                    .FirstOrDefaultAsync(ja => ja.ApplicationID == applicationId && ja.CandidateID == candidateId);

                if (application == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "APPLICATION_NOT_FOUND",
                        Message = "Không tìm thấy đơn ứng tuyển."
                    };

                // Chỉ cho phép rút đơn khi status là Submitted hoặc Reviewing
                if (application.Status != "Submitted" && application.Status != "Reviewing")
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 400,
                        ErrorCode = "CANNOT_WITHDRAW",
                        Message = "Không thể rút đơn ứng tuyển ở trạng thái hiện tại."
                    };

                // Giảm ApplicationCount của JobPosting
                if (application.JobPosting != null)
                {
                    application.JobPosting.ApplicationCount--;
                }

                dbContext.JobApplications.Remove(application);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Rút đơn ứng tuyển thành công.",
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

        public async Task<ApiResult<bool>> CheckIfAppliedAsync(int candidateId, int jobPostingId)
        {
            try
            {
                bool hasApplied = await dbContext.JobApplications
                    .AnyAsync(ja => ja.CandidateID == candidateId && ja.JobPostingID == jobPostingId);

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Data = hasApplied
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

        // ==========================================
        // EMPLOYER OPERATIONS
        // ==========================================

        public async Task<PagedResult<JobApplicationDto>> GetJobApplicationsAsync(int employerId, ApplicationQueryParameters queryParams)
        {
            try
            {
                var query = dbContext.JobApplications
                    .Include(ja => ja.JobPosting)
                    .Include(ja => ja.Candidate)
                        .ThenInclude(c => c.User)
                    .Where(ja => ja.JobPosting.EmployerID == employerId)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(ja => ja.Status == queryParams.Status);
                }

                // Filter by job posting
                if (queryParams.JobPostingID.HasValue)
                {
                    query = query.Where(ja => ja.JobPostingID == queryParams.JobPostingID.Value);
                }

                int totalRecords = await query.CountAsync();

                // Sorting
                query = queryParams.SortOrder == "asc"
                    ? query.OrderBy(ja => ja.AppliedAt)
                    : query.OrderByDescending(ja => ja.AppliedAt);

                // Pagination
                var applications = await query
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .Select(ja => new JobApplicationDto
                    {
                        ApplicationID = ja.ApplicationID,
                        JobPostingID = ja.JobPostingID,
                        CandidateID = ja.CandidateID,
                        JobTitle = ja.JobPosting.JobTitle,
                        CandidateName = ja.Candidate.User.FullName,
                        CandidateEmail = ja.Candidate.User.Email,
                        CandidatePhone = ja.Candidate.User.Phone,
                        CandidateAvatar = ja.Candidate.User.Avatar,
                        CVFilePath = ja.CVFilePath,
                        Status = ja.Status,
                        AppliedAt = ja.AppliedAt,
                        ReviewedAt = ja.ReviewedAt,
                        EmployerNotes = ja.EmployerNotes
                    })
                    .ToListAsync();

                return new PagedResult<JobApplicationDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách đơn ứng tuyển thành công.",
                    Data = applications,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<JobApplicationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<PagedResult<JobApplicationDto>> GetApplicationsByJobAsync(int jobPostingId, int employerId, ApplicationQueryParameters queryParams)
        {
            try
            {
                // Kiểm tra job posting thuộc về employer
                var jobPosting = await dbContext.JobPostings
                    .FirstOrDefaultAsync(j => j.JobPostingID == jobPostingId && j.EmployerID == employerId);

                if (jobPosting == null)
                    return new PagedResult<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        Message = "Không tìm thấy tin tuyển dụng."
                    };

                var query = dbContext.JobApplications
                    .Include(ja => ja.Candidate)
                        .ThenInclude(c => c.User)
                    .Where(ja => ja.JobPostingID == jobPostingId)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(ja => ja.Status == queryParams.Status);
                }

                int totalRecords = await query.CountAsync();

                var applications = await query
                    .OrderByDescending(ja => ja.AppliedAt)
                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .Select(ja => new JobApplicationDto
                    {
                        ApplicationID = ja.ApplicationID,
                        CandidateID = ja.CandidateID,
                        CandidateName = ja.Candidate.User.FullName,
                        CandidateEmail = ja.Candidate.User.Email,
                        CandidatePhone = ja.Candidate.User.Phone,
                        CandidateAvatar = ja.Candidate.User.Avatar,
                        CVFilePath = ja.CVFilePath,
                        CoverLetter = ja.CoverLetter,
                        Status = ja.Status,
                        AppliedAt = ja.AppliedAt,
                        ReviewedAt = ja.ReviewedAt,
                        EmployerNotes = ja.EmployerNotes
                    })
                    .ToListAsync();

                return new PagedResult<JobApplicationDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách ứng viên thành công.",
                    Data = applications,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<JobApplicationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<JobApplicationDto>> GetApplicationDetailAsync(int applicationId, int employerId)
        {
            try
            {
                var application = await dbContext.JobApplications
                    .Include(ja => ja.JobPosting)
                    .Include(ja => ja.Candidate)
                        .ThenInclude(c => c.User)
                    .Include(ja => ja.Candidate)
                        .ThenInclude(c => c.Educations)
                    .Include(ja => ja.Candidate)
                        .ThenInclude(c => c.Experiences)
                    .FirstOrDefaultAsync(ja => ja.ApplicationID == applicationId && ja.JobPosting.EmployerID == employerId);

                if (application == null)
                    return new ApiResult<JobApplicationDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "APPLICATION_NOT_FOUND",
                        Message = "Không tìm thấy đơn ứng tuyển."
                    };

                return new ApiResult<JobApplicationDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy chi tiết đơn ứng tuyển thành công.",
                    Data = new JobApplicationDto
                    {
                        ApplicationID = application.ApplicationID,
                        JobPostingID = application.JobPostingID,
                        CandidateID = application.CandidateID,
                        JobTitle = application.JobPosting.JobTitle,
                        CandidateName = application.Candidate.User.FullName,
                        CandidateEmail = application.Candidate.User.Email,
                        CandidatePhone = application.Candidate.User.Phone,
                        CandidateAvatar = application.Candidate.User.Avatar,
                        CVFilePath = application.CVFilePath,
                        CoverLetter = application.CoverLetter,
                        Status = application.Status,
                        AppliedAt = application.AppliedAt,
                        ReviewedAt = application.ReviewedAt,
                        EmployerNotes = application.EmployerNotes
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<JobApplicationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> UpdateApplicationStatusAsync(int applicationId, int employerId, UpdateApplicationStatusRequest request)
        {
            try
            {
                var application = await dbContext.JobApplications
                    .Include(ja => ja.JobPosting)
                    .FirstOrDefaultAsync(ja => ja.ApplicationID == applicationId && ja.JobPosting.EmployerID == employerId);

                if (application == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "APPLICATION_NOT_FOUND",
                        Message = "Không tìm thấy đơn ứng tuyển."
                    };

                application.Status = request.Status;
                application.EmployerNotes = request.EmployerNotes;
                application.ReviewedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = $"Cập nhật trạng thái đơn ứng tuyển thành '{request.Status}' thành công.",
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

        public async Task<ApiResult<Dictionary<string, int>>> GetApplicationStatisticsAsync(int employerId)
        {
            try
            {
                var statistics = await dbContext.JobApplications
                    .Where(ja => ja.JobPosting.EmployerID == employerId)
                    .GroupBy(ja => ja.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);

                // Thêm các status còn thiếu với count = 0
                var allStatuses = new[] { "Submitted", "Reviewing", "Shortlisted", "Interviewed", "Accepted", "Rejected" };
                foreach (var status in allStatuses)
                {
                    if (!statistics.ContainsKey(status))
                        statistics[status] = 0;
                }

                return new ApiResult<Dictionary<string, int>>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy thống kê đơn ứng tuyển thành công.",
                    Data = statistics
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<Dictionary<string, int>>
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