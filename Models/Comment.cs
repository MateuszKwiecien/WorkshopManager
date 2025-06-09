// Models/Comment.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        [Required]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: komentarz może dotyczyć zlecenia lub klienta
        public int? ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; }

        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}