// Models/Part.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Manufacturer { get; set; }

        public decimal UnitPrice { get; set; }

        // Navigation
        public ICollection<UsedPart> UsedInOrders { get; set; } = new List<UsedPart>();
    }
}