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

            var mail = new System.Net.Mail.MailMessage(_config["Email:SmtpUser"], to, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }
    }
}
