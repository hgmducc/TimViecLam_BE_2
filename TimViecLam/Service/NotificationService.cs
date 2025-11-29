using TimViecLam.Models.Dto.Request;
using TimViecLam.Repository.IRepository;

namespace TimViecLam.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        public async Task SendJobApplicationNotificationAsync(int employerId, string candidateName, string jobTitle)
        {
            var request = new CreateNotificationRequest
            {
                UserID = employerId,
                Title = "Đơn ứng tuyển mới",
                Message = $"{candidateName} đã ứng tuyển vào vị trí {jobTitle}.",
                Type = "Info",
                RelatedLink = "/employer/applications"
            };

            await notificationRepository.CreateNotificationAsync(request);
        }

        public async Task SendApplicationStatusUpdateAsync(int candidateId, string jobTitle, string status)
        {
            string message = status switch
            {
                "Reviewing" => $"Đơn ứng tuyển '{jobTitle}' của bạn đang được xem xét.",
                "Shortlisted" => $"Chúc mừng!  Bạn đã được chọn vào vòng tiếp theo cho vị trí '{jobTitle}'.",
                "Interviewed" => $"Bạn đã được mời phỏng vấn cho vị trí '{jobTitle}'.",
                "Accepted" => $"Chúc mừng! Đơn ứng tuyển '{jobTitle}' của bạn đã được chấp nhận.",
                "Rejected" => $"Rất tiếc, đơn ứng tuyển '{jobTitle}' của bạn chưa phù hợp lần này.",
                _ => $"Trạng thái đơn ứng tuyển '{jobTitle}' đã được cập nhật."
            };

            var type = status == "Accepted" ? "Success" : status == "Rejected" ? "Warning" : "Info";

            var request = new CreateNotificationRequest
            {
                UserID = candidateId,
                Title = "Cập nhật đơn ứng tuyển",
                Message = message,
                Type = type,
                RelatedLink = "/candidate/applications"
            };

            await notificationRepository.CreateNotificationAsync(request);
        }

        public async Task SendJobPublishedNotificationAsync(int employerId, string jobTitle)
        {
            var request = new CreateNotificationRequest
            {
                UserID = employerId,
                Title = "Tin tuyển dụng đã được đăng",
                Message = $"Tin tuyển dụng '{jobTitle}' đã được đăng thành công và đang hiển thị công khai.",
                Type = "Success",
                RelatedLink = "/employer/jobs"
            };

            await notificationRepository.CreateNotificationAsync(request);
        }

        public async Task SendEmployerVerifiedNotificationAsync(int employerId, string companyName, bool isApproved)
        {
            var request = new CreateNotificationRequest
            {
                UserID = employerId,
                Title = isApproved ? "Tài khoản đã được xác minh" : "Tài khoản chưa được xác minh",
                Message = isApproved
                    ? $"Chúc mừng! Tài khoản công ty '{companyName}' đã được xác minh.  Bạn có thể bắt đầu đăng tin tuyển dụng."
                    : $"Tài khoản công ty '{companyName}' chưa được xác minh.  Vui lòng kiểm tra lại thông tin và giấy phép kinh doanh.",
                Type = isApproved ? "Success" : "Warning",
                RelatedLink = "/employer/profile"
            };

            await notificationRepository.CreateNotificationAsync(request);
        }

        public async Task SendWelcomeNotificationAsync(int userId, string userName)
        {
            var request = new CreateNotificationRequest
            {
                UserID = userId,
                Title = "Chào mừng đến với TimViecLam!",
                Message = $"Xin chào {userName}! Chúc bạn tìm được công việc phù hợp trên nền tảng của chúng tôi.",
                Type = "Info"
            };

            await notificationRepository.CreateNotificationAsync(request);
        }
    }
}