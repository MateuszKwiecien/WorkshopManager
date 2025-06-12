using Mapster;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig c)
        {
            c.NewConfig<Customer,     CustomerDto>();
            c.NewConfig<CustomerDto, Customer>();
        }
    }
}