namespace Authentication_Authorisation.Services;

public interface IAccountService
{
    Task<string> ConfirmEmail(string userId, string token);
}