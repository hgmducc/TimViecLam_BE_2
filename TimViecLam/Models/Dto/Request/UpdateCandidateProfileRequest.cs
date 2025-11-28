using System.ComponentModel.DataAnnotations;

namespace TimViecLam.Models.Dto.Request
{
    public class UpdateCandidateProfileRequest
    {
        [StringLength(100)]
        public string? DesiredPosition { get; set; }

        [Range(0, 999999999)]
        public decimal? DesiredSalary { get; set; }

        [Range(0, 50)]
        public int? YearsOfExperience { get; set; }

        public List<string>? Skills { get; set; }
    }
}