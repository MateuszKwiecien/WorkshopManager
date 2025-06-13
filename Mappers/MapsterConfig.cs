// Mappers/MapsterConfig.cs

using System.Diagnostics;
using Mapster;
using WorkshopManager.DTOs;
using WorkshopManager.Models;

namespace WorkshopManager.Mappers
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig cfg)
        {
            /*──────────── Customer ↔ CustomerDto ────────────*/
            cfg.NewConfig<Customer,     CustomerDto>();
            cfg.NewConfig<CustomerDto,  Customer>();

            /*────────── Vehicle ↔ VehicleDto ──────────*/
            cfg.NewConfig<Vehicle, VehicleDto>()
                .ConstructUsing(s => new VehicleDto(
                    s.Id,
                    s.Make,
                    s.Model,
                    s.RegistrationNumber,
                    s.Year,
                    s.CustomerId,
                    s.Customer != null ? s.Customer.FullName : "gowno",// CustomerName
                    s.ImagePath ?? string.Empty
                ));
            cfg.NewConfig<VehicleDto, Vehicle>();    // odwrotny kierunek bez zmian

            /*──────────── ServiceOrder ↔ ServiceOrderDto ────*/
            cfg.NewConfig<ServiceOrder, ServiceOrderDto>()
                .Map(d => d.CustomerName, s => s.Customer.FullName)
                .Map(d => d.VehicleReg,   s => s.Vehicle.RegistrationNumber);

            cfg.NewConfig<ServiceOrderDto, ServiceOrder>();

            /*──────────── ServiceTask ↔ ServiceTaskDto ──────*/
            // ServiceTask ↔ ServiceTaskDto
            cfg.NewConfig<ServiceTask, ServiceTaskDto>()
                .Map(d => d.OrderId, s => s.ServiceOrderId);

            cfg.NewConfig<ServiceTaskDto, ServiceTask>()
                .Map(d => d.ServiceOrderId, s => s.OrderId);


            // UsedPart ↔ UsedPartDto
            cfg.NewConfig<UsedPart, UsedPartDto>()
                .Map(d => d.PartName,  s => s.Part.Name)
                .Map(d => d.UnitPrice, s => s.Part.UnitPrice)
                .Map(d => d.OrderId,   s => s.ServiceOrderId);

            cfg.NewConfig<UsedPartDto, UsedPart>()
                .Map(d => d.ServiceOrderId, s => s.OrderId);
            
            // Part ↔ PartDto
            cfg.NewConfig<Part,     PartDto>();
            cfg.NewConfig<PartDto,  Part>();
        }
    }
}