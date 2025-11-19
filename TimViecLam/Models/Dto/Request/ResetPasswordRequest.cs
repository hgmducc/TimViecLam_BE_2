using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimViecLam.Models.Dto.Request
{
    public class ResetPasswordRequest
    {
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [MinLength(8, ErrorMessage = "Mật khẩu phải ít nhất 8 ký tự.")]
    public string NewPassword { get; set; } = null!;
    }
}
