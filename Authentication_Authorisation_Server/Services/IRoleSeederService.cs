using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorisation.Services;

public interface IRoleSeederService
{
    public Task SeedRoleAsync(); 
}