using VehicleRegistryAPI.Dtos;
using VehicleRegistryAPI.Models;

namespace VehicleRegistryAPI.Services;

public interface IVehicleService
{
    Task<List<Vehicle>> GetAll();
    Task<Vehicle> GetById(int id);
    Task<Vehicle> Create(CreateVehicleDto vehicleDto);
    Task<Vehicle> Update(int id, CreateVehicleDto vehicleDto);
    Task<bool> Delete(int id);
}