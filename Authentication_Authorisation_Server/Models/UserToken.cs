using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorisation.Models;

public class UserToken : IdentityUserToken<string>
{
    public DateTime? ExpiryTime { get; set; }
}