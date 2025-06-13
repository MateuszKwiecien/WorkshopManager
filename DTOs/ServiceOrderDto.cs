namespace WorkshopManager.DTOs;

public record ServiceOrderDto(
    int      Id,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string   Status,
    int      CustomerId,
    string   CustomerName,
    int      VehicleId,
    string   VehicleReg);