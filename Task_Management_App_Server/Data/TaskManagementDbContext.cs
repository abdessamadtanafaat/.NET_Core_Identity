using Microsoft.EntityFrameworkCore;
using Task_Management_App.Models;

namespace Task_Management_App.Data;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options) { }

    //public DbSet<Task> Tasks { get; set; } // Map the tasks table.
    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}  