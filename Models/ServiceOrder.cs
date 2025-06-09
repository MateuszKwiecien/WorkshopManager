// Models/ServiceOrder.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class ServiceOrder
    {
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        [StringLength(200)]
        public string Status { get; set; } = "Open";

        // Foreign keys
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        // Navigation
        public ICollection<ServiceTask> Tasks { get; set; } = new List<ServiceTask>();
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}