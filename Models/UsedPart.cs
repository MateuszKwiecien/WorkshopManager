using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;          // ★ PrecisionAttribute

namespace WorkshopManager.Models
{
    public class UsedPart
    {
        public int Id { get; set; }

        // ─── Klucze obce ──────────────────────────
        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; } = null!;

        public int PartId { get; set; }
        public Part Part { get; set; } = null!;

        // ─── Dane ────────────────────────────────
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Precision(18, 2)]          // EF Core 7/8 atrybut precyzji
        [Range(0.01, 999999)]
        public decimal UnitPrice { get; set; }

        [Required, StringLength(100)]
        public string PartName { get; set; } = string.Empty;
    }
}