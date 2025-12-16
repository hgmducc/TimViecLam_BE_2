using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TimViecLam.Data;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Repository
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CandidateRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ProfileResult> GetCandidateProfileAsync(int candidateId)
        {
            try
            {
                var candidate = await dbContext.Candidates
                    .Include(c => c.User)
                    .Include(c => c.Educations)
                    .Include(c => c.Experiences)
                    .FirstOrDefaultAsync(c => c.CandidateID == candidateId);

                if (candidate == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "CANDIDATE_NOT_FOUND",
                        Message = "Không tìm thấy hồ sơ ứng viên."
                    };

                // Parse skills từ JSON
                List<string>? skills = null;
                if (!string.IsNullOrEmpty(candidate.Skills))
                {
                    try
                    {
                        skills = JsonSerializer.Deserialize<List<string>>(candidate.Skills);
                    }
                    catch
                    {
                        skills = candidate.Skills.Split(',').Select(s => s.Trim()).ToList();
                    }
                }

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Lấy hồ sơ ứng viên thành công.",
                    Data = new ProfileResponse
                    {
                        UserID = candidate.User.UserID,
                        FullName = candidate.User.FullName,
                        Email = candidate.User.Email,
                        Phone = candidate.User.Phone,
                        DateOfBirth = candidate.User.DateOfBirth,
                        Gender = candidate.User.Gender,
                        Address = candidate.User.Address,
                        Role = candidate.User.Role,
                        AvatarUrl = candidate.User.Avatar,
                        Status = candidate.User.Status,
                        CreatedAt = candidate.User.CreatedAt,
                        UpdatedAt = candidate.User.UpdatedAt,
                        CandidateProfile = new CandidateProfileDto
                        {
                            CandidateID = candidate.CandidateID,
                            DesiredPosition = candidate.DesiredPosition,
                            DesiredSalary = candidate.DesiredSalary,
                            YearsOfExperience = candidate.YearsOfExperience,
                            CVFileName = candidate.CVFileName,
                            CVFilePath = candidate.CVFilePath,
                            Skills = skills,
                            Educations = candidate.Educations.Select(e => new EducationDto
                            {
                                EducationID = e.EducationID,
                                InstitutionName = e.InstitutionName,
                                Degree = e.Degree,
                                Major = e.Major,
                                StartDate = e.StartDate,
                                EndDate = e.EndDate,
                                Description = e.Description
                            }).ToList(),
                            Experiences = candidate.Experiences.Select(ex => new ExperienceDto
                            {
                                ExperienceID = ex.ExperienceID,
                                CompanyName = ex.CompanyName,
                                Position = ex.Position,
                                StartDate = ex.StartDate,
                                EndDate = ex.EndDate,
                                Description = ex.Description,
                                IsCurrent = ex.IsCurrent
                            }).ToList()
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

        public async Task<ProfileResult> UpdateCandidateProfileAsync(int candidateId, UpdateCandidateProfileRequest request)
        {
            try
            {
                var candidate = await dbContext.Candidates.FindAsync(candidateId);

                if (candidate == null)
                    return new ProfileResult
                    {
                        IsSuccess = false,
                        Status = 404,
                        ErrorCode = "CANDIDATE_NOT_FOUND",
                        Message = "Không tìm thấy hồ sơ ứng viên."
                    };

                // Cập nhật thông tin
                candidate.DesiredPosition = request.DesiredPosition;
                candidate.DesiredSalary = request.DesiredSalary;
                candidate.YearsOfExperience = request.YearsOfExperience;

                // Cập nhật skills
                if (request.Skills != null && request.Skills.Any())
                {
                    candidate.Skills = JsonSerializer.Serialize(request.Skills);
                }

                candidate.LastUpdated = DateTime.UtcNow;

                // Tính lại profile completeness
                candidate.ProfileCompleteness = await CalculateProfileCompletenessAsync(candidateId);

                await dbContext.SaveChangesAsync();

                return new ProfileResult
                {
                    IsSuccess = true,
                    Status = 200,
                    Message = "Cập nhật hồ sơ ứng viên thành công."
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

        public async Task<int> CalculateProfileCompletenessAsync(int candidateId)
        {
            var candidate = await dbContext.Candidates
                .Include(c => c.User)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .FirstOrDefaultAsync(c => c.CandidateID == candidateId);

            if (candidate == null) return 0;

            int completeness = 0;

            // Basic info (30%)
            if (!string.IsNullOrEmpty(candidate.User.FullName)) completeness += 5;
            if (!string.IsNullOrEmpty(candidate.User.Email)) completeness += 5;
            if (!string.IsNullOrEmpty(candidate.User.Phone)) completeness += 5;
            if (!string.IsNullOrEmpty(candidate.User.Address)) completeness += 5;
            if (!string.IsNullOrEmpty(candidate.User.Avatar)) completeness += 10;

            // Candidate info (30%)
            if (!string.IsNullOrEmpty(candidate.DesiredPosition)) completeness += 10;
            if (candidate.DesiredSalary.HasValue) completeness += 5;
            if (candidate.YearsOfExperience.HasValue) completeness += 5;
            if (!string.IsNullOrEmpty(candidate.Skills)) completeness += 10;

            // CV (20%)
            if (!string.IsNullOrEmpty(candidate.CVFilePath)) completeness += 20;

            // Education (10%)
            if (candidate.Educations.Any()) completeness += 10;

            // Experience (10%)
            if (candidate.Experiences.Any()) completeness += 10;

            return completeness;
        }

        public async Task<bool> UpdateSkillsAsync(int candidateId, List<string> skills)
        {
            try
            {
                var candidate = await dbContext.Candidates.FindAsync(candidateId);
                if (candidate == null) return false;

                candidate.Skills = JsonSerializer.Serialize(skills);
                candidate.LastUpdated = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}