using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Authentication_Authorisation.Models;
using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorisation.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, UserToken>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    
     public DbSet<User> Users { get; set; }
     public DbSet<UserToken> UserTokens { get; set; }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}