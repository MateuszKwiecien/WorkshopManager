using Riok.Mapperly.Abstractions;
using WorkshopManager.Models;
using WorkshopManager.DTOs;

namespace WorkshopManager.Mappers;

[Mapper]
public partial class EntityMapper
{
    public partial CustomerDto CustomerToDto(Customer customer);
    public partial Customer DtoToCustomer(CustomerDto dto);

    public partial VehicleDto VehicleToDto(Vehicle vehicle);
    public partial Vehicle DtoToVehicle(VehicleDto dto);

    public partial OrderDto ServiceOrderToDto(ServiceOrder order);
    public partial ServiceOrder DtoToServiceOrder(OrderDto dto);
}

//var mapper = new EntityMapper();
//CustomerDto dto = mapper.CustomerToDto(customer);
//Customer customer = mapper.DtoToCustomer(dto);