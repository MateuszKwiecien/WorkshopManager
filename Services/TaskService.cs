using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

/// <inheritdoc />
public class TaskService : ITaskService
{
    private readonly IRepository<ServiceTask> _repo;
    private readonly IMapper                  _map;

    public TaskService(IRepository<ServiceTask> repo, IMapper map)
    {
        _repo = repo;
        _map  = map;
    }

    /*──── SZCZEGÓŁ ────────────────────────────────────────*/

    public async Task<ServiceTaskDto?> GetAsync(int id)
        => _map.Map<ServiceTaskDto?>(await _repo.GetByIdAsync(id));

    /*──── CREATE ─────────────────────────────────────────*/

    public async Task AddAsync(ServiceTaskDto dto)
    {
        var entity = _map.Map<ServiceTask>(dto);

        // awaryjne przypisanie FK (gdyby mapowanie nie skopiowało pola)
        entity.ServiceOrderId = dto.OrderId;

        await _repo.AddAsync(entity);
        await _repo.SaveAsync();
    }


    /*──── UPDATE ─────────────────────────────────────────*/

    public async Task<bool> UpdateAsync(int id, ServiceTaskDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return false;

        _map.Map(dto, entity);          // nadpisuje pola Description, Price, OrderId
        await _repo.SaveAsync();
        return true;
    }

    /*──── DELETE ─────────────────────────────────────────*/

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return false;

        _repo.Delete(entity);
        await _repo.SaveAsync();
        return true;
    }
    
    public async Task<IEnumerable<ServiceTaskDto>> GetManyAsync(IEnumerable<int> ids)
    {
        var list = await _repo.Query()
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
        return _map.Map<IEnumerable<ServiceTaskDto>>(list);
    }
    
    public async Task<IEnumerable<ServiceTaskDto>> ListAsync(int orderId)
    {
        var list = await _repo.Query()                     // IQueryable<ServiceTask>
            .Where(t => t.ServiceOrderId == orderId)
            .OrderBy(t => t.Id)
            .ToListAsync();

        return _map.Map<IEnumerable<ServiceTaskDto>>(list);
    }

}