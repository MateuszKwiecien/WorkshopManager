using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using WorkshopManager.Models;

namespace WorkshopManager.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _repo;
        private readonly IMapper               _map;

        public CustomerService(IRepository<Customer> repo, IMapper map)
        {
            _repo = repo;
            _map  = map;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync(string? q)
        {
            var list = await _repo.ListAsync(c =>
                string.IsNullOrEmpty(q) ||
                EF.Functions.Like(c.FullName, $"%{q}%"));
            return _map.Map<IEnumerable<CustomerDto>>(list);
        }

        public async Task<CustomerDto?> GetAsync(int id) =>
            _map.Map<CustomerDto?>(await _repo.GetByIdAsync(id));

        public async Task<int> AddAsync(CustomerDto dto)
        {
            var entity = _map.Map<Customer>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, CustomerDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return false;
            _map.Map(dto, entity);
            _repo.Update(entity);
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
    }
}