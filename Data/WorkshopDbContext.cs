// Data/WorkshopDbContext.cs
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Models;

namespace WorkshopManager.Data
{
    public class WorkshopDbContext : DbContext
    {
        public WorkshopDbContext(DbContextOptions<WorkshopDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<ServiceTask> ServiceTasks { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<UsedPart> UsedParts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Przykład konfiguracji: unikaty na rejestracji pojazdu
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.RegistrationNumber)
                .IsUnique();

            // Relacje optional/required
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ServiceOrder)
                .WithMany(o => o.Comments)
                .HasForeignKey(c => c.ServiceOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Customer)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}