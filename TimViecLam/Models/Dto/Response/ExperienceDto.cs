namespace TimViecLam.Models.Dto.Response
{
    public class ExperienceDto
    {
        public int ExperienceID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public bool IsCurrent { get; set; }
    }
}