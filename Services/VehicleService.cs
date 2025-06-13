using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

public class VehicleService : IVehicleService
{
    private readonly IRepository<Vehicle> _repo;
    private readonly IMapper              _map;

    public VehicleService(IRepository<Vehicle> repo, IMapper map)
    {
        _repo = repo;
        _map  = map;
    }

    /*──────── LISTA ────────*/
    public async Task<IEnumerable<VehicleDto>> GetAllAsync(int customerId)
    {
        IQueryable<Vehicle> query = _repo.Query()
            .Include(v => v.Customer);

        if (customerId != 0)
            query = query.Where(v => v.CustomerId == customerId);

        var dtoList = await query
            .Select(v => new VehicleDto(
                v.Id,
                v.Make,
                v.Model,
                v.RegistrationNumber,
                v.Year,
                v.CustomerId,
                v.Customer.FullName                // ← imię trafia bez Mapstera
            ))
            .ToListAsync();

        return dtoList;
    }


    /*──────── SZCZEGÓŁ ────*/
    public async Task<VehicleDto?> GetAsync(int id)
    {
        var v = await _repo.Query()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);

        return v is null ? null
            : new VehicleDto(v.Id, v.Make, v.Model,
                v.RegistrationNumber, v.Year,
                v.CustomerId, v.Customer.FullName);
    }

    public async Task<int> AddAsync(VehicleDto dto)
    {
        var e = _map.Map<Vehicle>(dto);
        await _repo.AddAsync(e);
        await _repo.SaveAsync();
        return e.Id;
    }

    public async Task<bool> UpdateAsync(int id, VehicleDto dto)
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