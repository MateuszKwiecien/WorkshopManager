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
            // Vehicle ↔ VehicleDto
            c.NewConfig<Vehicle,     VehicleDto>()
                .Map(d => d.CustomerName, s => s.Customer.FullName);

            c.NewConfig<VehicleDto,  Vehicle>();

            // ServiceOrder ↔ ServiceOrderDto
            c.NewConfig<ServiceOrder, ServiceOrderDto>()
                .Map(d => d.CustomerName, s => s.Customer.FullName)
                .Map(d => d.VehicleReg,   s => s.Vehicle.RegistrationNumber);

            c.NewConfig<ServiceOrderDto, ServiceOrder>();
        }
    }
}