using Authentication_Authorisation.Models;

namespace Authentication_Authorisation.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendConfirmationEmail(string email, User user);
}