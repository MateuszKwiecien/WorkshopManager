using MapsterMapper;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<ServiceTask> _repo;
    private readonly IMapper _map;
    public TaskService(IRepository<ServiceTask> r, IMapper m)
    { _repo = r; _map = m; }

    public async Task<IEnumerable<ServiceTaskDto>> ListAsync(int orderId) =>
        _map.Map<IEnumerable<ServiceTaskDto>>(
            await _repo.ListAsync(t => t.ServiceOrderId == orderId));


    public async Task AddAsync(ServiceTaskDto dto)
    {
        await _repo.AddAsync(_map.Map<ServiceTask>(dto));
        await _repo.SaveAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t is null) return;
        _repo.Delete(t);
        await _repo.SaveAsync();
    }
}