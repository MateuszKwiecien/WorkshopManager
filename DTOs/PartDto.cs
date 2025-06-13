// DTOs/PartDto.cs
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.DTOs;

public record PartDto
{
    public int Id { get; init; }

    [Required, StringLength(100)]
    public string Name { get; init; } = "";

    [Required, StringLength(100)]
    public string Manufacturer { get; init; } = "";

    [Range(0, 100_000)]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; init; }
}
