using MailKit.Net.Smtp;
using MimeKit;

namespace WildShape_Sheets_API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public void SendPasswordResetEmail(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Wild Shape Sheets Password Services", "wildshapesheetsemail@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail)); // Use the recipient's email here
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                string sendGridApiKey = _configuration["APIKeys:SendGridAPIKey"] ?? throw new ArgumentNullException(nameof(sendGridApiKey));
                client.Connect("smtp.sendgrid.net", 465, true);
                client.Authenticate("apikey", sendGridApiKey);
                client.Send(message);
                client.Disconnect(true);
            }

            Console.WriteLine("Email sent successfully");
        }
    }
}
