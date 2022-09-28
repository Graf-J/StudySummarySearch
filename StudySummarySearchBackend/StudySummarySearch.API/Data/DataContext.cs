using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudySummarySearch.API.Models;

namespace StudySummarySearch.API.Data
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _config;

        public DataContext(DbContextOptions<DataContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config.GetConnectionString("DefaultConnection"));
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Summary> Summaries => Set<Summary>();
        public DbSet<Semester> Semesters => Set<Semester>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Keyword> Keywords => Set<Keyword>();
    }
}