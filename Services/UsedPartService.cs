using Microsoft.EntityFrameworkCore;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

public class UsedPartService : IUsedPartService
{
    private readonly IRepository<UsedPart> _repo;

    public UsedPartService(IRepository<UsedPart> repo)
    {
        _repo = repo;
    }

    /*────────────────────── CRUD ──────────────────────*/

    public async Task AddAsync(UsedPartDto dto)
    {
        var entity = new UsedPart
        {
            ServiceOrderId = dto.OrderId,     // FK do zlecenia
            PartId         = dto.PartId,      // FK do katalogu Parts
            Quantity       = dto.Quantity,
            UnitPrice      = dto.UnitPrice,   // cena przekazana z kontrolera
            PartName       = dto.PartName     // nazwa przekazana z kontrolera
        };

        await _repo.AddAsync(entity);
        await _repo.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repo.GetAsync(id);
        if (entity is not null)
        {
            _repo.Delete(entity);
            await _repo.SaveAsync();
        }
    }

    public async Task<IEnumerable<UsedPartDto>> ListAsync(int orderId)
    {
        var list = await _repo.Query()
            .Where(u => u.ServiceOrderId == orderId)
            .ToListAsync();

        // ręczne mapowanie – tylko pola potrzebne w widoku
        return list.Select(u => new UsedPartDto(
            Id:        u.Id,
            OrderId:   u.ServiceOrderId,
            PartId:    u.PartId,
            Quantity:  u.Quantity,
            UnitPrice: u.UnitPrice,
            PartName:  u.PartName));
    }
}