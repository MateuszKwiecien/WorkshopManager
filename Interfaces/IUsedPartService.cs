using WorkshopManager.DTOs;

namespace WorkshopManager.Interfaces;

public interface IUsedPartService
{
    Task<IEnumerable<UsedPartDto>> ListAsync(int orderId);
    Task AddAsync(UsedPartDto dto);
    Task DeleteAsync(int id);
    
    Task<IEnumerable<PartDto>> GetManyAsync(IEnumerable<int> ids);

}