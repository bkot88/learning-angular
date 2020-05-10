using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using WorldCities.Data.Models;

namespace WorldCities.Data
{
    public class ApplicationDbContext : DbContext
    {
        private const string PricesSchemaName = "prices";

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Quote>().HasKey(q => new { q.Symbol, q.Date });
            modelBuilder.Entity<Quote>().HasIndex(q => q.Symbol);
            modelBuilder.Entity<Quote>().HasIndex(q => q.Date);
            modelBuilder.Entity<Quote>().HasIndex(q => new { q.Date, q.Symbol });
        }

        public void EnesureCreated()
        {
            Database.EnsureCreated();
            //var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            //if (!databaseCreator.HasTables()) databaseCreator.CreateTables();

            //Database.ExecuteSqlRaw($"create schema if not exists {PricesSchemaName}");
            //Database.ExecuteSqlRaw($"create table if not exists {PricesSchemaName}.audusd (vector int[]);");
        }
    }
}
