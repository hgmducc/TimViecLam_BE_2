using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimViecLam.Models.Domain;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // lấy tất cả người dùng
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();

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


        // lấy theo id của người dùng
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = $"Không tìm thấy user với ID {id}."
                    });
                }

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


        // sửa thông tin người dùng
        [HttpPut("{id}")]
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

                var updatedUser = await _userRepository.UpdateUserAsync(id, userToUpdate);

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


        // xóa người dùng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userRepository.DeleteUserAsync(id);

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


        // cập nhật trạng thái tài khoản
        [HttpPatch("{id}/status")]
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
                var updatedUser = await _userRepository.UpdateUserStatusAsync(id, request.Status);

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
    }
}