namespace TimViecLam.Models.Dto.Response
{
    public class EducationDto
    {
        public int EducationID { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
        public string? Degree { get; set; }
        public string? Major { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
    }
}