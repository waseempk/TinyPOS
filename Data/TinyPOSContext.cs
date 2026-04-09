using Microsoft.EntityFrameworkCore;
using TinyPOSApp.Models;

namespace TinyPOSApp.Data
{
    public class TinyPOSContext : DbContext
    {
        public TinyPOSContext(DbContextOptions<TinyPOSContext> options) : base(options)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>().ToTable("profiles");
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Transaction>().ToTable("transactions");
            modelBuilder.Entity<TransactionItem>().ToTable("transaction_items");
        }
    }
}
