using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.DTOs;

public record ServiceTaskDto
{
    public int Id { get; init; }

    [Required]
    public int OrderId { get; init; }

    [Required, StringLength(200)]
    public string Description { get; init; } = "";

    [Range(0, 100_000)]
    [DataType(DataType.Currency)]
    public decimal Price { get; init; }
}