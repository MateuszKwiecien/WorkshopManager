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

            // kaskada z Vehicle → Customer zostaje
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ───── naprawa: blokujemy kaskadę Vehicle → ServiceOrder
            modelBuilder.Entity<ServiceOrder>()
                .HasOne(o => o.Vehicle)
                .WithMany(v => v.ServiceOrders)
                .HasForeignKey(o => o.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);   // albo NoAction

            // relacja Customer → ServiceOrder może pozostać kaskadowa
            modelBuilder.Entity<ServiceOrder>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.ServiceOrders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // (opcjonalnie) unikat na numer rejestracyjny
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.RegistrationNumber)
                .IsUnique();
        }
    }
}