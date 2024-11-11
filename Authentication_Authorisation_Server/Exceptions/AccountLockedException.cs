namespace Authentication_Authorisation.Exceptions;

public class AccountLockedException: Exception
{
    public AccountLockedException(string message) : base(message)
    {
    }
}