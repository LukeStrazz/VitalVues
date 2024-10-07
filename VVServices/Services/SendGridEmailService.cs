using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SendGridEmailService
    {
        private readonly string _apiKey;

        public SendGridEmailService(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task SendEmail(string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("whitj13@farmingdale.edu", "Vital Vues");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Email sent successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to send email: {response.StatusCode}");
            }
        }
    }
}



