﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task_Management_App.Models;

namespace Task_Management_App.Data;

public class TaskManagementDbContext : IdentityDbContext<User>
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options) { }

    // Example DbSet for tasks
     // public DbSet<Task> Tasks { get; set; }
     public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}