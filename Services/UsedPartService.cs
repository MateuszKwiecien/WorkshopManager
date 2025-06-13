using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

public class UsedPartService : IUsedPartService
{
    private readonly IRepository<UsedPart> _repo;
    private readonly IMapper               _map;

    public UsedPartService(IRepository<UsedPart> repo, IMapper map)
    {
        _repo = repo;
        _map  = map;
    }

    public async Task<IEnumerable<UsedPartDto>> ListAsync(int orderId) =>
        _map.Map<IEnumerable<UsedPartDto>>(
            await _repo.ListAsync(p => p.ServiceOrderId == orderId));

    public async Task AddAsync(UsedPartDto dto)
    {
        var entity = _map.Map<UsedPart>(dto);
        await _repo.AddAsync(entity);
        await _repo.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return;
        _repo.Delete(entity);
        await _repo.SaveAsync();
    }
    
    public async Task<IEnumerable<PartDto>> GetManyAsync(IEnumerable<int> ids)
    {
        var list = await _repo.Query()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        return _map.Map<IEnumerable<PartDto>>(list);
    }

}