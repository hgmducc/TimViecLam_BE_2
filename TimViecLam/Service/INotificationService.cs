namespace TimViecLam.Service
{
    public interface INotificationService
    {
        Task SendJobApplicationNotificationAsync(int employerId, string candidateName, string jobTitle);
        Task SendApplicationStatusUpdateAsync(int candidateId, string jobTitle, string status);
        Task SendJobPublishedNotificationAsync(int employerId, string jobTitle);
        Task SendEmployerVerifiedNotificationAsync(int employerId, string companyName, bool isApproved);
        Task SendWelcomeNotificationAsync(int userId, string userName);
    }
}