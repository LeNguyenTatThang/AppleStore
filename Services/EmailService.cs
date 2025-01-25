using System.Net.Mail;
using System.Net;

namespace AppleStore.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Thay bằng SMTP server của bạn
        private readonly int _smtpPort = 587; // Port cho SMTP (587 cho TLS)
        private readonly string _fromEmail = "lethang01674187728@gmail.com"; // Email gửi
        private readonly string _fromPassword = "jbas klkx eyoc wkyx"; // Mật khẩu ứng dụng hoặc email

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(_fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                Console.WriteLine($"Gửi email thất bại: {ex.Message}");
                throw;
            }
        }
    }
}
