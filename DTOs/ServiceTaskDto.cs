namespace WorkshopManager.DTOs;

public record ServiceTaskDto(
    int    Id,
    string Description,
    decimal Price,
    int    OrderId);