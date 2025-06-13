using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<ServiceOrder> _repo;
    private readonly IMapper                   _map;

    public OrderService(IRepository<ServiceOrder> repo, IMapper map)
    {
        _repo = repo; _map = map;
    }

    public async Task<IEnumerable<ServiceOrderDto>> GetAllAsync(string? status)
    {
        IQueryable<ServiceOrder> query = _repo.Query()
            .Include(o => o.Customer)
            .Include(o => o.Vehicle);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        var dtoList = await query
            .Select(o => new ServiceOrderDto(
                o.Id,
                o.CreatedAt,
                o.CompletedAt,
                o.Status,
                o.CustomerId,
                o.Customer.FullName,
                o.VehicleId,
                o.Vehicle.RegistrationNumber
            ))
            .ToListAsync();

        return dtoList;
    }

    public async Task<ServiceOrderDto?> GetAsync(int id)
        => _map.Map<ServiceOrderDto?>(await _repo.GetByIdAsync(id));

    public async Task<int> AddAsync(ServiceOrderDto dto)
    {
        var e = _map.Map<ServiceOrder>(dto);
        e.CreatedAt = DateTime.UtcNow;
        await _repo.AddAsync(e);
        await _repo.SaveAsync();
        return e.Id;
    }

    public async Task<bool> UpdateAsync(int id, ServiceOrderDto dto)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e is null) return false;
        _map.Map(dto, e);
        _repo.Update(e);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e is null) return false;
        _repo.Delete(e);
        await _repo.SaveAsync();
        return true;
    }
    
    
}