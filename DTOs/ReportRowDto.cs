namespace WorkshopManager.DTOs;

public record ReportRowDto(
    DateTime Date,
    string   Customer,
    string   Vehicle,
    decimal  Labor,
    decimal  Parts,
    decimal  Total);