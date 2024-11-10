namespace Authentication_Authorisation.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message)
    { }

}