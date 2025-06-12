// Data/WorkshopDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Models;


namespace WorkshopManager.Data
{
    public class WorkshopDbContext : IdentityDbContext
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

            // ServiceOrder: Vehicle relationship - restrict delete
            modelBuilder.Entity<ServiceOrder>()
                .HasOne(o => o.Vehicle)
                .WithMany(v => v.ServiceOrders)
                .HasForeignKey(o => o.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceOrder: Customer relationship - cascade delete
            modelBuilder.Entity<ServiceOrder>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.ServiceOrders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment: ServiceOrder relationship - restrict delete to avoid cascade path
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ServiceOrder)
                .WithMany(o => o.Comments)
                .HasForeignKey(c => c.ServiceOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment: Customer relationship - cascade delete
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Customer)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Part>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceTask>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);
        }
    }
}