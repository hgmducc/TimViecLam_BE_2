using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class LocationDetailRequest
    {
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Url(ErrorMessage = "Link bản đồ không hợp lệ")]
        [StringLength(500)]
        public string? MapLink { get; set; }
    }
}
