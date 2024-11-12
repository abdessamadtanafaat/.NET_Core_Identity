using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorisation.Services;

public class RoleSeederService : IRoleSeederService
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleSeederService(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager; 
    }

    public async Task SeedRoleAsync()
    {
        var adminRole = await _roleManager.FindByNameAsync("admin"); 
        if (!await _roleManager.RoleExistsAsync("admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("admin")); 
        }

        // Retrieve existing claims for the admin role
        var existingClaims = await _roleManager.GetClaimsAsync(adminRole);
       
        // Define the claims you want to add to the admin role
        var claimsToAdd = new List<Claim>
        {
            new Claim("Permission", "ManageUsers"),
            new Claim("Permission", "ManageRoles")
        };

        // Add each claim only if it doesn't already exist
        foreach (var claim in claimsToAdd)
        {
            if (!existingClaims.Any(c=> c.Type == claim.Type && c.Value == claim.Value))
            {
                await _roleManager.AddClaimAsync(adminRole, claim); 
            }
        }
        
        
        if (!await _roleManager.RoleExistsAsync("user"))
        {
            await _roleManager.CreateAsync(new IdentityRole("user")); 
        }
    }
}