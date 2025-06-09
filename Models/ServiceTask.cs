// Models/ServiceTask.cs
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class ServiceTask
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        // Foreign key
        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; }
    }
}