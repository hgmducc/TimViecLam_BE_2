using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class GoogleLoginRequest
    {
        [Required(ErrorMessage = "Google token là bắt buộc. ")]
        public string GoogleToken { get; set; } = string.Empty;
    }
}