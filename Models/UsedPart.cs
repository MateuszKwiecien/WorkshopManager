// Models/UsedPart.cs
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class UsedPart
    {
        public int Id { get; set; }

        // Foreign keys
        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int Quantity { get; set; }
    }
}