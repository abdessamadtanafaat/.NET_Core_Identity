using System.Text;
using Authentication_Authorisation.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using Org.BouncyCastle.Tsp;

namespace Authentication_Authorisation.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<User> _logger;

    public EmailSender(IConfiguration configuration,
        UserManager<User> userManager,
        ILogger<User> logger)
    {
        _configuration = configuration;
        _userManager = userManager;
        _logger = logger; 
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var emailConfig = _configuration.GetSection("EmailSettings");
        var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(emailConfig["Host"], int.Parse(emailConfig["Port"]), SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(emailConfig["Username"], emailConfig["Password"]);

        var message = new MimeMessage(); 
        message.From.Add(new MailboxAddress("DB_APP", emailConfig["From"]));
        message.To.Add(new MailboxAddress("" ,to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true); 
    }

    public async Task SendConfirmationEmail(string email, User user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        Console.WriteLine(token);
        
        /*if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Le token est null ou vide. Vérifiez la configuration.");
        }
        else
        {
            _logger.LogInformation($"Token généré : {token}");
        }*/
        /*
        _logger.LogWarning($"UserId : {user.Email} , Token : {token}"); 
        */
        // envoyer email : 
        var confirmationLink =
            $"https://your-domain.com/api/account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}"; 
        
        var emailBody = $@"
    <html>
        <body>
            <p>Click below to confirm your email:</p>
            <a href='{confirmationLink}' style='background-color:#007bff; padding:10px 20px; color:white; text-decoration:none;'>Confirm Email</a>
        </body>
    </html>";        
        await SendEmailAsync(email, "Email Confirmation",emailBody);
    }
}