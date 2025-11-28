namespace TimViecLam.Models.Dto.Response
{
    public class CandidateProfileDto
    {
        public int CandidateID { get; set; }
        public string? DesiredPosition { get; set; }
        public decimal? DesiredSalary { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? CVFileName { get; set; }
        public string? CVFilePath { get; set; }

        // Nested collections
        public List<string>? Skills { get; set; }
        public List<EducationDto>? Educations { get; set; }
        public List<ExperienceDto>? Experiences { get; set; }
    }
}