using IlluviumTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IlluviumTest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<NFT> NFTs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasDiscriminator<string>("Type")
                .HasValue<MintTransaction>("Mint")
                .HasValue<BurnTransaction>("Burn")
                .HasValue<TransferTransaction>("Transfer");

            modelBuilder.Entity<MintTransaction>().Property(m => m.TokenId).IsRequired();
            modelBuilder.Entity<BurnTransaction>().Property(b => b.TokenId).IsRequired();
            modelBuilder.Entity<TransferTransaction>().Property(t => t.TokenId).IsRequired();
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Create options
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 23)));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

}
