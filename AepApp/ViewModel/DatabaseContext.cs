using System;
using AepApp.Services;
using AepApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace AepApp.ViewModel
{
    public class DatabaseContext : DbContext
    {
        public DbSet<TestPersonViewModel.Person> Persons { get; set; }

        public DbSet<TestPersonViewModel.Plan> Plans { get; set; }
        public DatabaseContext()
        {
            this.Database.EnsureCreated();
            this.Database.MigrateAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = DependencyService.Get<IDatabaseService>().GetDbPath();
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

    }
}
