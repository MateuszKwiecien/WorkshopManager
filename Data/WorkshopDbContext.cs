// Data/WorkshopDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WorkshopManager.Models;
using WorkshopManager.DTOs;

namespace WorkshopManager.Data
{
    /// <summary>
    /// Główny kontekst EF Core – zawiera zarówno encje domenowe,
    /// jak i struktury ASP-NET Identity.
    /// </summary>
    public class WorkshopDbContext : IdentityDbContext<ApplicationUser>
    {
        public WorkshopDbContext(DbContextOptions<WorkshopDbContext> options)
            : base(options) { }

        public DbSet<Customer>     Customers     { get; set; }
        public DbSet<Vehicle>      Vehicles      { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<ServiceTask>  ServiceTasks  { get; set; }
        public DbSet<Part>         Parts         { get; set; }
        public DbSet<UsedPart>     UsedParts     { get; set; }
        public DbSet<Comment>      Comments      { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);   // © IdentityDbContext

            // kaskada Vehicle → Customer zostaje
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // naprawa: Vehicle → ServiceOrder bez kaskady
            modelBuilder.Entity<ServiceOrder>()
                .HasOne(o => o.Vehicle)
                .WithMany(v => v.ServiceOrders)
                .HasForeignKey(o => o.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);   // lub NoAction

            // kaskada Customer → ServiceOrder
            modelBuilder.Entity<ServiceOrder>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.ServiceOrders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // unikat na numer rejestracyjny pojazdu
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.RegistrationNumber)
                .IsUnique();
        }
    public DbSet<WorkshopManager.DTOs.ServiceOrderDto> ServiceOrderDto { get; set; } = default!;
    public DbSet<WorkshopManager.DTOs.VehicleDto> VehicleDto { get; set; } = default!;
    }
}
