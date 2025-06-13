namespace WorkshopManager.DTOs;

public record UsedPartDto(
    int     Id,
    int     OrderId,
    int     PartId,
    int     Quantity,
    decimal UnitPrice,
    string  PartName);