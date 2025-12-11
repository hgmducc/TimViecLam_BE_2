namespace TimViecLam.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new System.Net.Mail.SmtpClient(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]))
            {
                Credentials = new System.Net.NetworkCredential(_config["Email:SmtpUser"], _config["Email:SmtpPass"]),
                EnableSsl = true
            };

            // ⚡ Quan trọng: thêm Display Name ở đây
            var fromAddress = new System.Net.Mail.MailAddress(
                _config["Email:SmtpUser"],
                "Hệ Thống Tìm Kiếm Việc Làm" 
            );

            var mail = new System.Net.Mail.MailMessage()
            {
                From = fromAddress,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }
    }
}
