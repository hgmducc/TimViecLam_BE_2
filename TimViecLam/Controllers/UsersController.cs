using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;
using TimViecLam.Service;

namespace TimViecLam.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IEmployerRepository employerRepository;
        private readonly INotificationService notificationService;

        public UsersController(
            IUserRepository userRepository,
            IEmployerRepository employerRepository,
            INotificationService notificationService)
        {
            this.userRepository = userRepository;
            this.employerRepository = employerRepository;
            this.notificationService = notificationService;
        }

        // ==========================================
        // QUẢN LÝ TẤT CẢ USERS
        // ==========================================

        // GET: api/admin/users - Lấy tất cả users (có thể filter theo role)
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParameters queryParams)
        {
            try
            {
                // Nếu muốn lấy riêng Employers → dùng EmployerRepository
                if (queryParams.Role?.ToLower() == "employer")
                {
                    PagedResult<ProfileResponse> result = await employerRepository.GetAllEmployersAsync(queryParams);
                    return StatusCode(result.Status, result);
                }

                // Lấy tất cả users (chưa có phân trang trong UserRepository hiện tại)
                var users = await userRepository.GetAllUsersAsync();

                // Filter by role nếu có
                if (!string.IsNullOrEmpty(queryParams.Role))
                {
                    users = users.Where(u => u.Role.Equals(queryParams.Role, StringComparison.OrdinalIgnoreCase));
                }

                // Filter by status
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    users = users.Where(u => u.Status == queryParams.Status);
                }

                // Filter by search term
                if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                {
                    users = users.Where(u =>
                        u.FullName.Contains(queryParams.SearchTerm) ||
                        u.Email.Contains(queryParams.SearchTerm));
                }

                var userResponses = users.Select(u => new UserResponse
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    IsGoogleAccount = u.IsGoogleAccount,
                    Phone = u.Phone,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    Address = u.Address,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    Status = u.Status
                });

                return Ok(new
                {
                    isSuccess = true,
                    message = "Lấy danh sách user thành công.",
                    data = userResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Có lỗi xảy ra khi lấy danh sách user.",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/users/{id} - Lấy user theo ID (auto-include profile)
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = $"Không tìm thấy user với ID {id}."
                    });
                }

                // Nếu là Employer → lấy full profile từ EmployerRepository
                if (user.Role == "Employer")
                {
                    ProfileResult employerProfile = await employerRepository.GetEmployerProfileAsync(id);
                    return StatusCode(employerProfile.Status, employerProfile);
                }

                // Trả về User thông thường
                var userResponse = new UserResponse
                {
                    UserID = user.UserID,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsGoogleAccount = user.IsGoogleAccount,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Address = user.Address,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Status = user.Status
                };

                return Ok(new
                {
                    isSuccess = true,
                    message = "Lấy thông tin user thành công.",
                    data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Có lỗi xảy ra khi lấy thông tin user.",
                    error = ex.Message
                });
            }
        }

        // PUT: api/admin/users/{id} - Sửa thông tin user
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var userToUpdate = new User
                {
                    FullName = request.FullName,
                    Phone = request.Phone,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    Address = request.Address
                };

                var updatedUser = await userRepository.UpdateUserAsync(id, userToUpdate);

                if (updatedUser == null)
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = $"Không tìm thấy user với ID {id}."
                    });
                }

                var userResponse = new UserResponse
                {
                    UserID = updatedUser.UserID,
                    FullName = updatedUser.FullName,
                    Email = updatedUser.Email,
                    IsGoogleAccount = updatedUser.IsGoogleAccount,
                    Phone = updatedUser.Phone,
                    DateOfBirth = updatedUser.DateOfBirth,
                    Gender = updatedUser.Gender,
                    Address = updatedUser.Address,
                    Role = updatedUser.Role,
                    CreatedAt = updatedUser.CreatedAt,
                    UpdatedAt = updatedUser.UpdatedAt,
                    Status = updatedUser.Status
                };

                return Ok(new
                {
                    isSuccess = true,
                    message = "Cập nhật user thành công.",
                    data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Có lỗi xảy ra khi cập nhật user.",
                    error = ex.Message
                });
            }
        }

        // DELETE: api/admin/users/{id} - Xóa user
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await userRepository.DeleteUserAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = $"Không tìm thấy user với ID {id}."
                    });
                }

                return Ok(new
                {
                    isSuccess = true,
                    message = "Xoá user thành công."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Có lỗi xảy ra khi xoá user.",
                    error = ex.Message
                });
            }
        }

        // PATCH: api/admin/users/{id}/status - Cập nhật trạng thái Active/Locked
        [HttpPatch("users/{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var updatedUser = await userRepository.UpdateUserStatusAsync(id, request.Status);

                if (updatedUser == null)
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = $"Không tìm thấy user với ID {id}."
                    });
                }

                var userResponse = new UserResponse
                {
                    UserID = updatedUser.UserID,
                    FullName = updatedUser.FullName,
                    Email = updatedUser.Email,
                    IsGoogleAccount = updatedUser.IsGoogleAccount,
                    Phone = updatedUser.Phone,
                    DateOfBirth = updatedUser.DateOfBirth,
                    Gender = updatedUser.Gender,
                    Address = updatedUser.Address,
                    Role = updatedUser.Role,
                    CreatedAt = updatedUser.CreatedAt,
                    UpdatedAt = updatedUser.UpdatedAt,
                    Status = updatedUser.Status
                };

                return Ok(new
                {
                    isSuccess = true,
                    message = $"Cập nhật trạng thái user thành công thành '{request.Status}'.",
                    data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Có lỗi xảy ra khi cập nhật trạng thái user.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // QUẢN LÝ ĐẶC BIỆT CHO EMPLOYERS
        // ==========================================

        // GET: api/admin/employers/pending - Lấy employers chờ duyệt
        [HttpGet("employers/pending")]
        public async Task<IActionResult> GetPendingEmployers([FromQuery] UserQueryParameters queryParams)
        {
            var pendingParams = new UserQueryParameters
            {
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                SearchTerm = queryParams.SearchTerm,
                SortBy = queryParams.SortBy,
                SortOrder = queryParams.SortOrder
            };

            PagedResult<ProfileResponse> result = await employerRepository.GetAllEmployersAsync(pendingParams);

            // Filter by Pending status
            if (result.IsSuccess && result.Data != null)
            {
                result.Data = result.Data
                    .Where(p => p.EmployerProfile?.VerificationStatus == "Pending")
                    .ToList();
                result.TotalRecords = result.Data.Count;
            }

            return StatusCode(result.Status, result);
        }

        // PATCH: api/admin/employers/{id}/verify - Duyệt/Từ chối giấy phép
        [HttpPatch("employers/{id}/verify")]
        public async Task<IActionResult> VerifyEmployer(int id, [FromBody] VerifyEmployerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            if (request.Status != "Verified" && request.Status != "Rejected")
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Trạng thái chỉ có thể là 'Verified' hoặc 'Rejected'."
                });
            }

            var employerProfile = await employerRepository.GetEmployerProfileAsync(id);

            if (!employerProfile.IsSuccess || employerProfile.Data?.EmployerProfile == null)
            {
                return NotFound(new
                {
                    isSuccess = false,
                    message = "Không tìm thấy nhà tuyển dụng."
                });
            }

            ApiResult<bool> result = await employerRepository.UpdateVerificationStatusAsync(
                id,
                request.Status,
                request.Notes
            );

            if (result.IsSuccess)
            {
                bool isApproved = request.Status == "Verified";
                string companyName = employerProfile.Data.EmployerProfile.CompanyName;

                await notificationService.SendEmployerVerifiedNotificationAsync(
                    id,
                    companyName,
                    isApproved
                );
            }

            return StatusCode(result.Status, result);
        }

        // GET: api/admin/employers/statistics - Thống kê employers
        [HttpGet("employers/statistics")]
        public async Task<IActionResult> GetEmployerStatistics()
        {
            try
            {
                var allEmployers = await employerRepository.GetAllEmployersAsync(new UserQueryParameters
                {
                    Page = 1,
                    PageSize = int.MaxValue
                });

                if (!allEmployers.IsSuccess)
                {
                    return StatusCode(allEmployers.Status, allEmployers);
                }

                var statistics = new
                {
                    total = allEmployers.Data.Count,
                    verified = allEmployers.Data.Count(e => e.EmployerProfile?.VerificationStatus == "Verified"),
                    pending = allEmployers.Data.Count(e => e.EmployerProfile?.VerificationStatus == "Pending"),
                    rejected = allEmployers.Data.Count(e => e.EmployerProfile?.VerificationStatus == "Rejected")
                };

                return Ok(new
                {
                    isSuccess = true,
                    message = "Lấy thống kê thành công.",
                    data = statistics
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Đã xảy ra lỗi:  " + ex.Message
                });
            }
        }
    }
}