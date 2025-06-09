// Models/Vehicle.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Make { get; set; }

        [Required, StringLength(50)]
        public string Model { get; set; }

        [StringLength(20)]
        public string RegistrationNumber { get; set; }

        public int Year { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        // Navigation
        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    }
}