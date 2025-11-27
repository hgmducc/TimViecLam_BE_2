namespace TimViecLam.Models.Dto.Response
{
    public class ProfileResult
    {
        public bool IsSuccess { get; set; }
        public int Status { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public ProfileResponse? Data { get; set; }
    }
}