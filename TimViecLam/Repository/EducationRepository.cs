using Microsoft.EntityFrameworkCore;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class EducationRepository : IEducationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public EducationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ApiResult<List<EducationDto>>> GetEducationsByCandidateAsync(int candidateId)
        {
            try
            {
                var educations = await dbContext.Educations
                    .Where(e => e.CandidateID == candidateId)
                    .OrderByDescending(e => e.StartDate)
                    .Select(e => new EducationDto
                    {
                        EducationID = e.EducationID,
                        InstitutionName = e.InstitutionName,
                        Degree = e.Degree,
                        Major = e.Major,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description
                    })
                    .ToListAsync();

                return new ApiResult<List<EducationDto>>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy danh sách học vấn thành công.",
                    Data = educations
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<EducationDto>>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<EducationDto>> AddEducationAsync(int candidateId, AddEducationRequest request)
        {
            try
            {
                var education = new Education
                {
                    CandidateID = candidateId,
                    InstitutionName = request.InstitutionName,
                    Degree = request.Degree,
                    Major = request.Major,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.Educations.AddAsync(education);
                await dbContext.SaveChangesAsync();

                return new ApiResult<EducationDto>
                {
                    IsSuccess = true,
                    Status = 201,
                    Message = "Thêm học vấn thành công.",
                    Data = new EducationDto
                    {
                        EducationID = education.EducationID,
                        InstitutionName = education.InstitutionName,
                        Degree = education.Degree,
                        Major = education.Major,
                        StartDate = education.StartDate,
                        EndDate = education.EndDate,
                        Description = education.Description
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<EducationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<EducationDto>> UpdateEducationAsync(int educationId, AddEducationRequest request)
        {
            try
            {
                var education = await dbContext.Educations.FindAsync(educationId);

                if (education == null)
                    return new ApiResult<EducationDto>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EDUCATION_NOT_FOUND",
                        Message = "Không tìm thấy học vấn."
                    };

                education.InstitutionName = request.InstitutionName;
                education.Degree = request.Degree;
                education.Major = request.Major;
                education.StartDate = request.StartDate;
                education.EndDate = request.EndDate;
                education.Description = request.Description;
                education.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return new ApiResult<EducationDto>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật học vấn thành công.",
                    Data = new EducationDto
                    {
                        EducationID = education.EducationID,
                        InstitutionName = education.InstitutionName,
                        Degree = education.Degree,
                        Major = education.Major,
                        StartDate = education.StartDate,
                        EndDate = education.EndDate,
                        Description = education.Description
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<EducationDto>
                {
                    IsSuccess = false,
                    Status = 500,
                    ErrorCode = "SERVER_ERROR",
                    Message = "Đã xảy ra lỗi: " + ex.Message
                };
            }
        }

        public async Task<ApiResult<bool>> DeleteEducationAsync(int educationId, int candidateId)
        {
            try
            {
                var education = await dbContext.Educations
                    .FirstOrDefaultAsync(e => e.EducationID == educationId && e.CandidateID == candidateId);

                if (education == null)
                    return new ApiResult<bool>
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "EDUCATION_NOT_FOUND",
                        Message = "Không tìm thấy học vấn."
                    };

                dbContext.Educations.Remove(education);
                await dbContext.SaveChangesAsync();

                return new ApiResult<bool>
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Xóa học vấn thành công.",
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