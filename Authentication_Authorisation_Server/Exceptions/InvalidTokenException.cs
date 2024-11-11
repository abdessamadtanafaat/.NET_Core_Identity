namespace Authentication_Authorisation.Exceptions;

public class InvalidTokenException : Exception 
{
    public InvalidTokenException(string message) : base(message) { }
}

public class ExpiredRefreshTokenException : Exception
{
    public ExpiredRefreshTokenException(string message) : base(message) { }
}