using DatabaseLibrary.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DatabaseLibrary.ViewModels
{
    public class DatabaseContext : DbContext
    {

        public DbSet<CollectionAndTransportSampleModel> Plans { get; set; }

        public DatabaseContext()
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var dbPath = DependencyService.Get<IDatabaseService>().GetDbPath();
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}
