namespace WorkshopManager.DTOs;

public record VehicleDto(
    int    Id,
    string Make,
    string Model,
    string RegistrationNumber,
    int    Year,
    int    CustomerId,
    string CustomerName);         // tylko do podglądu w tabeli