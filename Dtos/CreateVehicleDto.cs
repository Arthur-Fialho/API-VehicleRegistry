using System.ComponentModel.DataAnnotations;

namespace VehicleRegistryAPI.Dtos;

// Este 'record' define a estrutura de dados para criar ou atualizar um veículo.
public record CreateVehicleDto(
    [Required] string Make,
    [Required] string Model,
    int Year,
    [Required][StringLength(7)] string LicensePlate
);
