using WorkshopManager.DTOs;

namespace WorkshopManager.Interfaces;
public interface IPartService
{
    Task<IEnumerable<PartDto>> GetAllAsync(string? search);
    Task<PartDto?>             GetAsync(int id);
    Task AddAsync(PartDto dto);
    Task<bool> UpdateAsync(int id, PartDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<PartDto>> GetManyAsync(IEnumerable<int> ids);

}