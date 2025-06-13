using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

public class PartService : IPartService
{
    private readonly IRepository<Part> _repo;
    private readonly IMapper           _map;

    public PartService(IRepository<Part> repo, IMapper map)
    { _repo = repo; _map = map; }

    public async Task<IEnumerable<PartDto>> GetAllAsync(string? search)
        => _map.Map<IEnumerable<PartDto>>(await _repo.ListAsync(
            search is null ? null
                : p => p.Name.Contains(search)));

    public async Task<PartDto?> GetAsync(int id)
        => _map.Map<PartDto?>(await _repo.GetByIdAsync(id));

    public async Task AddAsync(PartDto dto)
    {
        await _repo.AddAsync(_map.Map<Part>(dto));
        await _repo.SaveAsync();
    }

    public async Task<bool> UpdateAsync(int id, PartDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return false;

        _map.Map(dto, entity);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return false;

        _repo.Delete(entity);
        await _repo.SaveAsync();
        return true;
    }
    
    public async Task<IEnumerable<PartDto>> GetManyAsync(IEnumerable<int> ids)
    {
        var list = await _repo.Query()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        return _map.Map<IEnumerable<PartDto>>(list);
    }


}