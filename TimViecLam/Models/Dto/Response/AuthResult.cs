namespace TimViecLam.Models.Dto.Response
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public int Status { get; set; } // Ví dụ: 200, 400, 409
        public string? ErrorCode { get; set; } // Ví dụ: "EMAIL_EXISTS"
        public string? Message { get; set; } // "Email đã tồn tại"
        public string? Token { get; set; } // Token nếu đăng nhập thành công
    }
}
