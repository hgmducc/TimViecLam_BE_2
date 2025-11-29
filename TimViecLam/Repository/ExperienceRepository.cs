using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ExperienceRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ApiResult<List<ExperienceDto>>> GetExperiencesByCandidateAsync(int candidateId)
        {
            try
            {
                var experiences = await dbContext.Experiences
                    .Where(e => e.CandidateID == candidateId)
                    .OrderByDescending(e => e.StartDate)
                    .Select(e => new ExperienceDto
                    {
                        ExperienceID = e.ExperienceID,
                        CompanyName = e.CompanyName,
                        Position = e.Position,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description,
                        IsCurrent = e.IsCurrent
                    })
                    .ToListAsync();

                return new ApiResult<List<ExperienceDto>>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách kinh nghiệm thành công.",
                    Data = experiences
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<ExperienceDto>>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<ExperienceDto>> AddExperienceAsync(int candidateId, AddExperienceRequest request)
        {
            try
            {
                // Nếu IsCurrent = true, set tất cả experience khác của candidate thành false
                if (request.IsCurrent)
                {
                    var currentExperiences = await dbContext.Experiences
                        .Where(e => e.CandidateID == candidateId && e.IsCurrent)
                        .ToListAsync();

                    foreach (var exp in currentExperiences)
                    {
                        exp.IsCurrent = false;
                    }
                }

                var experience = new Experience
                {
                    CandidateID = candidateId,
                    CompanyName = request.CompanyName,
                    Position = request.Position,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Description = request.Description,
                    IsCurrent = request.IsCurrent,
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.Experiences.AddAsync(experience);
                await dbContext.SaveChangesAsync();

                return new ApiResult<ExperienceDto>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Thêm kinh nghiệm thành công.",
                    Data = new ExperienceDto
                    {
                        ExperienceID = experience.ExperienceID,
                        CompanyName = experience.CompanyName,
                        Position = experience.Position,
                        StartDate = experience.StartDate,
                        EndDate = experience.EndDate,
                        Description = experience.Description,
                        IsCurrent = experience.IsCurrent
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<ExperienceDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<ExperienceDto>> UpdateExperienceAsync(int experienceId, AddExperienceRequest request)
        {
            try
            {
                var experience = await dbContext.Experiences.FindAsync(experienceId);

                if (experience == null)
                    return new ApiResult<ExperienceDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EXPERIENCE_NOT_FOUND",
                        Message = "Không tìm thấy kinh nghiệm."
                    };

                // Nếu IsCurrent = true, set tất cả experience khác thành false
                if (request.IsCurrent && !experience.IsCurrent)
                {
                    var currentExperiences = await dbContext.Experiences
                        .Where(e => e.CandidateID == experience.CandidateID && e.IsCurrent && e.ExperienceID != experienceId)
                        .ToListAsync();

                    foreach (var exp in currentExperiences)
                    {
                        exp.IsCurrent = false;
                    }
                }

                experience.CompanyName = request.CompanyName;
                experience.Position = request.Position;
                experience.StartDate = request.StartDate;
                experience.EndDate = request.EndDate;
                experience.Description = request.Description;
                experience.IsCurrent = request.IsCurrent;
                experience.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ApiResult<ExperienceDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật kinh nghiệm thành công.",
                    Data = new ExperienceDto
                    {
                        ExperienceID = experience.ExperienceID,
                        CompanyName = experience.CompanyName,
                        Position = experience.Position,
                        StartDate = experience.StartDate,
                        EndDate = experience.EndDate,
                        Description = experience.Description,
                        IsCurrent = experience.IsCurrent
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<ExperienceDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> DeleteExperienceAsync(int experienceId, int candidateId)
        {
            try
            {
                var experience = await dbContext.Experiences
                    .FirstOrDefaultAsync(e => e.ExperienceID == experienceId && e.CandidateID == candidateId);

                if (experience == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EXPERIENCE_NOT_FOUND",
                        Message = "Không tìm thấy kinh nghiệm."
                    };

                dbContext.Experiences.Remove(experience);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Xóa kinh nghiệm thành công.",
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
    }
}