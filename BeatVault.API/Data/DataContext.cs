using BeatVault.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeatVault.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Beat> Beats { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Prevent deleting a User from wiping out auction history
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Configure Money Types (Postgres uses 'numeric' automatically for decimal)
            // But strict configuration is good practice.
            modelBuilder.Entity<Auction>().Property(a => a.CurrentPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Auction>().Property(a => a.StartingPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bid>().Property(b => b.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Beat>().Property(b => b.LeasePrice).HasColumnType("decimal(18,2)");
        }
    }
}