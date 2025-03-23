using Microsoft.EntityFrameworkCore;
using SharedModels.Models;

namespace ConfigBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Policy> Policies { get; set; }
        public DbSet<Condition> Conditions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Policy>()
                .HasMany(p => p.Conditions)
                .WithOne(c => c.Policy)
                .HasForeignKey(c => c.PolicyId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Condition>()
                .HasOne(c => c.TrueCondition)
                .WithMany()
                .HasForeignKey(c => c.TrueConditionId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Condition>()
                .HasOne(c => c.FalseCondition)
                .WithMany()
                .HasForeignKey(c => c.FalseConditionId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}