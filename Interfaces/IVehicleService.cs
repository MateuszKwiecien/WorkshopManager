using WorkshopManager.DTOs;

namespace WorkshopManager.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllAsync(int customerId);
    Task<VehicleDto?>             GetAsync(int id);
    Task<int>                     AddAsync(VehicleDto dto);
    Task<bool>                    UpdateAsync(int id, VehicleDto dto);
    Task<bool>                    DeleteAsync(int id);
}