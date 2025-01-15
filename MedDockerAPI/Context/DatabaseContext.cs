using Microsoft.EntityFrameworkCore;
using System;
using MedDockerAPI.Models;
using MedDockerAPI.Controllers;


namespace MedDockerAPI.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
