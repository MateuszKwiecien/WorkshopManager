namespace WorkshopManager.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public string Vin { get; set; } = default!;
    public string Registration { get; set; } = default!;
    public int CustomerId { get; set; }
}