using Microsoft.EntityFrameworkCore;
using Tp3.Models;

namespace Tp3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Membership> Memberships { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer -> Membership (many-to-one)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Membership)
                .WithMany(m => m.Customers)
                .HasForeignKey(c => c.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Genre -> Movie (one-to-many)
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Genre)
                .WithMany(g => g.Movies)
                .HasForeignKey(m => m.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-many between Movie and Customer (EF Core will create the join table)
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Customers)
                .WithMany(c => c.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "CustomerMovie",
                    r => r.HasOne<Customer>().WithMany().HasForeignKey("CustomerId").OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<Movie>().WithMany().HasForeignKey("MovieId").OnDelete(DeleteBehavior.Cascade),
                    je =>
                    {
                        je.HasKey("CustomerId", "MovieId");
                        je.ToTable("CustomerMovies");
                    }
                );
        }
    }
}
