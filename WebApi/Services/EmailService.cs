using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;
using Microsoft.Extensions.Options;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, int code);
        Task SendPasswordResetEmailAsync(string toEmail, string newPassword);
    }

    public class EmailService : IEmailService
    {
        private readonly MailjetClient _client;
        private readonly MailjetSettings _settings;

        public EmailService(IOptions<MailjetSettings> settings)
        {
            _settings = settings.Value;
            _client = new MailjetClient(_settings.ApiKey, _settings.ApiSecret);
        }


        public async Task SendVerificationEmailAsync(string toEmail, int code)
        {
            try
            {

                var emailBody = $@"
                    <h3>DEVELOPMENT TEST Email Verification Code</h3>
                    <p><strong>Disclaimer:</strong> This email is part of a <strong>development testing process</strong> for Online Clinic and is not intended for live or production use.</p>
                    <p>Thank you for registering with Online Clinic.</p>
                    <p>Your verification code is: <strong>{code}</strong></p>
                    <p>This code will expire in 2 minutes.</p>
                    <p>If you did not request this email, please contact us at <a href='mailto:support@[yourdomain].com'>support@[yourdomain].com</a>.</p>
                    <hr>
                    <p style='color: gray; font-size: small;'>This email is part of a development testing process for <strong>Online Clinic</strong>. If you have received this email in error, please disregard it.</p>
                    <p>For more information, please refer to our <a href='https://www.yourdomain.com/privacy-policy'>Privacy Policy</a>.</p>
                    <p><strong>Disclaimer:</strong> This email is part of a <strong>development testing process</strong> for Online Clinic and is not intended for live or production use.</p>";


                var email = new TransactionalEmailBuilder()
                    .WithFrom(new SendContact(_settings.SenderEmail, "Online Clinic Development"))
                    .WithSubject("Email Verification Code")
                    .WithHtmlPart(emailBody)
                    .WithTo(new SendContact(toEmail))
                    .WithHeader("X-Environment", "Development")
                    .Build();

                var response = await _client.SendTransactionalEmailAsync(email);

                if (!response.Messages.Any(m => m.Status == "success"))
                {
                    throw new Exception("Failed to send email");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send verification email", ex);
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string newPassword)
        {
            try
            {
                var email = new TransactionalEmailBuilder()
                    .WithFrom(new SendContact(_settings.SenderEmail, _settings.SenderName))
                    .WithSubject("Password Reset")
                    .WithHtmlPart($@"
                    <h3>Password Reset</h3>
                    <p>Your password has been reset. Your new password is:</p>
                    <p style='font-size: 18px; font-weight: bold; padding: 10px; background-color: black; color:white;'>{newPassword}</p>
                    <p>For security reasons, we recommend changing this password after logging in.</p>
                    <p>If you did not request this password reset, please contact support immediately.</p>")
                    .WithTo(new SendContact(toEmail))
                    .Build();

                var response = await _client.SendTransactionalEmailAsync(email);

                if (!response.Messages.Any(m => m.Status == "success"))
                {
                    throw new Exception("Failed to send password reset email");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send password reset email", ex);
            }
        }

    }
}
