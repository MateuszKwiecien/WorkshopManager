namespace WorkshopManager.DTOs;

public record UsedPartDto(
    int    Id,
    int    PartId,
    string PartName,
    int    Quantity,
    decimal UnitPrice,
    int    OrderId);