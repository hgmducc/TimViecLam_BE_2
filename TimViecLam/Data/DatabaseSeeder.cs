using Microsoft.EntityFrameworkCore;
using TimViecLam.Models.Domain;

namespace TimViecLam.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            if (await context.Users.AnyAsync()) return;

            var users = SeedUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            var admins = SeedAdministrators(users);
            await context.Administrators.AddRangeAsync(admins);
            await context.SaveChangesAsync();

            var employers = SeedEmployers(users);
            await context.Employers.AddRangeAsync(employers);
            await context.SaveChangesAsync();

            var candidates = SeedCandidates(users);
            await context.Candidates.AddRangeAsync(candidates);
            await context.SaveChangesAsync();

            var educations = SeedEducations(candidates);
            await context.Educations.AddRangeAsync(educations);
            await context.SaveChangesAsync();

            var experiences = SeedExperiences(candidates);
            await context.Experiences.AddRangeAsync(experiences);
            await context.SaveChangesAsync();

            var jobs = SeedJobPostings(employers);
            await context.JobPostings.AddRangeAsync(jobs);
            await context.SaveChangesAsync();

            var applications = SeedJobApplications(candidates, jobs);
            await context.JobApplications.AddRangeAsync(applications);
            await context.SaveChangesAsync();
        }

        private static List<User> SeedUsers()
        {
            var now = DateTime.UtcNow;
            return new List<User>
            {
                new User { FullName = "Nguyen Van Admin", Email = "superadmin@timvieclam.vn", Phone = "0901000001", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), DateOfBirth = new DateOnly(1985, 5, 15), Gender = "Nam", Address = "123 Nguyen Hue, Quan 1, TP.HCM", Role = "Admin", Status = "Active", CreatedAt = now.AddMonths(-12) },
                new User { FullName = "Tran Thi Moderator", Email = "moderator@timvieclam.vn", Phone = "0901000002", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), DateOfBirth = new DateOnly(1990, 8, 20), Gender = "Nu", Address = "456 Le Loi, Quan 1, TP.HCM", Role = "Admin", Status = "Active", CreatedAt = now.AddMonths(-10) },
                new User { FullName = "Pham Van Support", Email = "support@timvieclam.vn", Phone = "0901000003", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), DateOfBirth = new DateOnly(1992, 3, 10), Gender = "Nam", Address = "789 Tran Hung Dao, Quan 5, TP.HCM", Role = "Admin", Status = "Active", CreatedAt = now.AddMonths(-8) },
                new User { FullName = "Le Van FPT", Email = "hr@fpt.com.vn", Phone = "0902000001", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employer@123"), DateOfBirth = new DateOnly(1988, 7, 25), Gender = "Nam", Address = "17 Duy Tan, Cau Giay, Ha Noi", Role = "Employer", Status = "Active", CreatedAt = now.AddMonths(-9) },
                new User { FullName = "Nguyen Thi VNG", Email = "tuyendung@vng.com.vn", Phone = "0902000002", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employer@123"), DateOfBirth = new DateOnly(1991, 11, 5), Gender = "Nu", Address = "182 Le Dai Hanh, Quan 11, TP.HCM", Role = "Employer", Status = "Active", CreatedAt = now.AddMonths(-8) },
                new User { FullName = "Tran Hoang Vietcombank", Email = "hr@vietcombank.com.vn", Phone = "0902000003", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employer@123"), DateOfBirth = new DateOnly(1985, 4, 12), Gender = "Nam", Address = "198 Tran Quang Khai, Quan 1, TP.HCM", Role = "Employer", Status = "Active", CreatedAt = now.AddMonths(-7) },
                new User { FullName = "Pham Thi TGDD", Email = "hr@thegioididong.com", Phone = "0902000004", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employer@123"), DateOfBirth = new DateOnly(1989, 9, 8), Gender = "Nu", Address = "128 Tran Quang Khai, Quan 1, TP.HCM", Role = "Employer", Status = "Active", CreatedAt = now.AddMonths(-6) },
                new User { FullName = "Vo Van Vingroup", Email = "careers@vingroup.net", Phone = "0902000005", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employer@123"), DateOfBirth = new DateOnly(1987, 12, 20), Gender = "Nam", Address = "7 Bang Lang, Vinhomes, Ha Noi", Role = "Employer", Status = "Active", CreatedAt = now.AddMonths(-5) },
                new User { FullName = "Nguyen Minh Tuan", Email = "nguyenminhtuan@gmail.com", Phone = "0903000001", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1995, 3, 15), Gender = "Nam", Address = "12 Le Van Sy, Quan 3, TP.HCM", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-4) },
                new User { FullName = "Tran Thi Huong", Email = "tranthihuong@gmail.com", Phone = "0903000002", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1997, 7, 22), Gender = "Nu", Address = "45 NTMK, Quan 1, TP.HCM", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-4) },
                new User { FullName = "Le Van Duc", Email = "levanduc.dev@gmail.com", Phone = "0903000003", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1994, 11, 8), Gender = "Nam", Address = "78 Cau Giay, Ha Noi", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-3) },
                new User { FullName = "Pham Thi Mai", Email = "phamthimai@outlook.com", Phone = "0903000004", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1996, 5, 30), Gender = "Nu", Address = "234 Bach Dang, Da Nang", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-3) },
                new User { FullName = "Hoang Van Phong", Email = "hoangvanphong.it@gmail.com", Phone = "0903000005", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1993, 9, 14), Gender = "Nam", Address = "56 Hai Ba Trung, Ha Noi", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-2) },
                new User { FullName = "Vu Thi Lan", Email = "vuthilan.acc@gmail.com", Phone = "0903000006", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1998, 1, 25), Gender = "Nu", Address = "89 Nguyen Van Linh, Da Nang", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-2) },
                new User { FullName = "Do Van Hung", Email = "dovanhung.java@gmail.com", Phone = "0903000007", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1992, 4, 18), Gender = "Nam", Address = "123 Tran Phu, Quan 5, TP.HCM", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-1) },
                new User { FullName = "Ngo Thi Thao", Email = "ngothithao.hr@gmail.com", Phone = "0903000008", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1999, 6, 12), Gender = "Nu", Address = "45 Pham Van Dong, Ha Noi", Role = "Candidate", Status = "Active", CreatedAt = now.AddMonths(-1) },
                new User { FullName = "Bui Quang Huy", Email = "buiquanghuy@gmail.com", Phone = "0903000009", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1995, 10, 5), Gender = "Nam", Address = "67 Nguyen Trai, Ha Noi", Role = "Candidate", Status = "Active", CreatedAt = now.AddDays(-20) },
                new User { FullName = "Dang Thi Ngoc", Email = "dangthingoc@gmail.com", Phone = "0903000010", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Candidate@123"), DateOfBirth = new DateOnly(1997, 2, 28), Gender = "Nu", Address = "90 Le Duan, TP.HCM", Role = "Candidate", Status = "Active", CreatedAt = now.AddDays(-10) }
            };
        }

        private static List<Administrator> SeedAdministrators(List<User> users)
        {
            var admins = users.Where(u => u.Role == "Admin").ToList();
            return new List<Administrator>
            {
                new Administrator { AdminID = admins[0].UserID, AdminRole = "SuperAdmin", Department = "IT", InternalTitle = "System Administrator", CreatedAt = DateTime.UtcNow },
                new Administrator { AdminID = admins[1].UserID, AdminRole = "Moderator", Department = "Content", InternalTitle = "Content Moderator", CreatedAt = DateTime.UtcNow },
                new Administrator { AdminID = admins[2].UserID, AdminRole = "Support", Department = "Customer Service", InternalTitle = "Support Specialist", CreatedAt = DateTime.UtcNow }
            };
        }

        private static List<Employer> SeedEmployers(List<User> users)
        {
            var empUsers = users.Where(u => u.Role == "Employer").ToList();
            return new List<Employer>
            {
                new Employer { EmployerID = empUsers[0].UserID, CompanyName = "FPT Software", CompanyWebsite = "https://fpt-software.com", CompanyDescription = "FPT Software la cong ty phan mem hang dau Viet Nam", TaxCode = "0101234567", VerificationStatus = "Verified", ContactPerson = "Le Van FPT", ContactEmail = "hr@fpt.com.vn", ContactPhone = "0902000001", CompanySize = "500+", Industry = "Cong nghe thong tin", LastUpdated = DateTime.UtcNow },
                new Employer { EmployerID = empUsers[1].UserID, CompanyName = "VNG Corporation", CompanyWebsite = "https://vng.com.vn", CompanyDescription = "VNG la cong ty cong nghe hang dau Viet Nam", TaxCode = "0301234568", VerificationStatus = "Verified", ContactPerson = "Nguyen Thi VNG", ContactEmail = "tuyendung@vng.com.vn", ContactPhone = "0902000002", CompanySize = "201-500", Industry = "Cong nghe thong tin", LastUpdated = DateTime.UtcNow },
                new Employer { EmployerID = empUsers[2].UserID, CompanyName = "Vietcombank", CompanyWebsite = "https://vietcombank.com.vn", CompanyDescription = "Ngan hang TMCP Ngoai Thuong Viet Nam", TaxCode = "0100112233", VerificationStatus = "Verified", ContactPerson = "Tran Hoang", ContactEmail = "hr@vietcombank.com.vn", ContactPhone = "0902000003", CompanySize = "500+", Industry = "Tai chinh - Ngan hang", LastUpdated = DateTime.UtcNow },
                new Employer { EmployerID = empUsers[3].UserID, CompanyName = "The Gioi Di Dong", CompanyWebsite = "https://thegioididong.com", CompanyDescription = "Chuoi ban le dien thoai lon nhat Viet Nam", TaxCode = "0309876543", VerificationStatus = "Verified", ContactPerson = "Pham Thi", ContactEmail = "hr@thegioididong.com", ContactPhone = "0902000004", CompanySize = "500+", Industry = "Ban le", LastUpdated = DateTime.UtcNow },
                new Employer { EmployerID = empUsers[4].UserID, CompanyName = "Vingroup", CompanyWebsite = "https://vingroup.net", CompanyDescription = "Tap doan da nganh lon nhat Viet Nam", TaxCode = "0105432198", VerificationStatus = "Pending", ContactPerson = "Vo Van", ContactEmail = "careers@vingroup.net", ContactPhone = "0902000005", CompanySize = "500+", Industry = "Bat dong san", LastUpdated = DateTime.UtcNow }
            };
        }

        private static List<Candidate> SeedCandidates(List<User> users)
        {
            var candUsers = users.Where(u => u.Role == "Candidate").ToList();
            var skills = new[] { "[\"C#\",\"ASP.NET\",\"SQL Server\"]", "[\"Java\",\"Spring Boot\",\"MySQL\"]", "[\"Python\",\"Django\",\"PostgreSQL\"]", "[\"JavaScript\",\"React\",\"Node.js\"]", "[\"PHP\",\"Laravel\",\"MongoDB\"]" };
            var candidates = new List<Candidate>();
            for (int i = 0; i < candUsers.Count; i++)
            {
                candidates.Add(new Candidate
                {
                    CandidateID = candUsers[i].UserID,
                    DesiredPosition = i % 2 == 0 ? "Software Developer" : "Business Analyst",
                    DesiredSalary = 15000000 + (i * 2000000),
                    YearsOfExperience = i % 5 + 1,
                    JobType = i % 3 == 0 ? "Full-time" : "Remote",
                    DesiredLocation = i % 3 == 0 ? "Ha Noi" : i % 3 == 1 ? "TP.HCM" : "Da Nang",
                    Skills = skills[i % 5],
                    ProfileCompleteness = 60 + (i * 4),
                    LastUpdated = DateTime.UtcNow
                });
            }
            return candidates;
        }

        private static List<Education> SeedEducations(List<Candidate> candidates)
        {
            var schools = new[] { "Dai hoc Bach Khoa Ha Noi", "Dai hoc Cong nghe - DHQGHN", "Dai hoc FPT", "Dai hoc Kinh te Quoc dan", "Dai hoc Bach Khoa TP.HCM" };
            var educations = new List<Education>();
            for (int i = 0; i < candidates.Count; i++)
            {
                educations.Add(new Education
                {
                    CandidateID = candidates[i].CandidateID,
                    InstitutionName = schools[i % 5],
                    Degree = "Cu nhan",
                    Major = i % 2 == 0 ? "Cong nghe thong tin" : "Quan tri kinh doanh",
                    StartDate = new DateOnly(2013 + (i % 3), 9, 1),
                    EndDate = new DateOnly(2017 + (i % 3), 6, 30),
                    Description = "Tot nghiep loai Kha",
                    CreatedAt = DateTime.UtcNow
                });
            }
            return educations;
        }

        private static List<Experience> SeedExperiences(List<Candidate> candidates)
        {
            var companies = new[] { "FPT Software", "VNG", "Viettel", "VNPT", "CMC" };
            var experiences = new List<Experience>();
            for (int i = 0; i < candidates.Count; i++)
            {
                experiences.Add(new Experience
                {
                    CandidateID = candidates[i].CandidateID,
                    CompanyName = companies[i % 5],
                    Position = i % 2 == 0 ? "Software Developer" : "Business Analyst",
                    StartDate = new DateOnly(2018 + (i % 3), 1, 1),
                    EndDate = i % 3 == 0 ? null : new DateOnly(2023, 12, 31),
                    IsCurrent = i % 3 == 0,
                    Description = "Phat trien va bao tri he thong",
                    CreatedAt = DateTime.UtcNow
                });
            }
            return experiences;
        }

        private static List<JobPosting> SeedJobPostings(List<Employer> employers)
        {
            var jobs = new List<JobPosting>();
            var titles = new[] { "Senior .NET Developer", "Java Developer", "Frontend Developer", "DevOps Engineer", "Business Analyst", "Project Manager", "QA Engineer", "Data Analyst", "Mobile Developer", "Full Stack Developer", "Cloud Architect", "UI/UX Designer", "Scrum Master", "Technical Lead", "HR Specialist" };
            var locations = new[] { "Ha Noi", "TP.HCM", "Da Nang" };
            var statuses = new[] { "Active", "Active", "Active", "Draft", "Closed" };
            for (int i = 0; i < 15; i++)
            {
                jobs.Add(new JobPosting
                {
                    EmployerID = employers[i % 5].EmployerID,
                    JobTitle = titles[i],
                    JobDescription = $"Mo ta cong viec cho vi tri {titles[i]}. Chung toi dang tim kiem ung vien co ky nang va kinh nghiem phu hop.",
                    Requirements = "Yeu cau: Tot nghiep dai hoc chuyen nganh CNTT hoac lien quan. Co kinh nghiem lam viec tu 1-3 nam.",
                    Benefits = "Luong canh tranh, BHXH, BHYT, 13 thang luong, du lich hang nam",
                    SalaryMin = 15000000 + (i * 2000000),
                    SalaryMax = 25000000 + (i * 3000000),
                    SalaryType = "Range",
                    JobType = i % 3 == 0 ? "Full-time" : i % 3 == 1 ? "Remote" : "Part-time",
                    Location = locations[i % 3],
                    Industry = i % 2 == 0 ? "Cong nghe thong tin" : "Tai chinh - Ngan hang",
                    ExperienceLevel = i % 4 == 0 ? "Junior" : i % 4 == 1 ? "Mid" : i % 4 == 2 ? "Senior" : "Lead",
                    YearsOfExperienceRequired = i % 5 + 1,
                    EducationLevel = "Bachelor",
                    VacancyCount = (i % 3) + 1,
                    ApplicationDeadline = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1 + (i % 3))),
                    Status = statuses[i % 5],
                    CreatedAt = DateTime.UtcNow.AddDays(-i * 2),
                    PublishedAt = statuses[i % 5] == "Active" ? DateTime.UtcNow.AddDays(-i * 2) : null,
                    ViewCount = i * 50,
                    ApplicationCount = i * 3
                });
            }
            return jobs;
        }

        private static List<JobApplication> SeedJobApplications(List<Candidate> candidates, List<JobPosting> jobs)
        {
            var applications = new List<JobApplication>();
            var statuses = new[] { "Submitted", "Reviewing", "Shortlisted", "Interviewed", "Accepted", "Rejected" };
            int appId = 0;
            for (int i = 0; i < candidates.Count; i++)
            {
                for (int j = 0; j < 3 && appId < 30; j++)
                {
                    var jobIndex = (i + j) % jobs.Count;
                    applications.Add(new JobApplication
                    {
                        JobPostingID = jobs[jobIndex].JobPostingID,
                        CandidateID = candidates[i].CandidateID,
                        CoverLetter = $"Toi rat quan tam den vi tri nay va mong muon duoc lam viec tai quy cong ty.",
                        Status = statuses[appId % 6],
                        AppliedAt = DateTime.UtcNow.AddDays(-appId),
                        ReviewedAt = appId % 3 != 0 ? DateTime.UtcNow.AddDays(-appId + 1) : null,
                        EmployerNotes = appId % 4 == 0 ? "Ung vien tiem nang" : null
                    });
                    appId++;
                }
            }
            return applications;
        }
    }
}
