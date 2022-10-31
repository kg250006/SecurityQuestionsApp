using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using DBService.DB.Models;

namespace DBService.DB
{
    public class IContextRepository : DbContext
    {
        public DbSet<Config> Configs { get; set; }
      
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //IConfiguration _appsettings;

            var dbName = AppContext.BaseDirectory + "secSQLite.db";
            optionsBuilder.UseSqlite( "Filename=" + dbName, option => 
            {
                option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Config>().ToTable("Configs", "test");
            modelBuilder.Entity<Config>(entity => {
                entity.HasKey(k => k.Id);
                //entity.HasIndex(i => i.Name).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
