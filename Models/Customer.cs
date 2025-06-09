// Models/Customer.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Phone, StringLength(20)]
        public string PhoneNumber { get; set; }

        [EmailAddress, StringLength(100)]
        public string Email { get; set; }

        // Navigation
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}