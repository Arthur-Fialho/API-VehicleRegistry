using Microsoft.EntityFrameworkCore;
using VehicleRegistryAPI.Data;
using VehicleRegistryAPI.Dtos;
using VehicleRegistryAPI.Models;

namespace VehicleRegistryAPI.Services;

// Implementação do serviço de veículos
public class VehicleService : IVehicleService
{
    // Injeção do contexto do banco de dados
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto via injeção de dependência
    public VehicleService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Implementação dos métodos da interface IVehicleService
    // Obtém todos os veículos
    public async Task<List<Vehicle>> GetAll() => await _context.Vehicles.ToListAsync();

    // Obtém um veículo por ID
    public async Task<Vehicle> GetById(int id) => await _context.Vehicles.FindAsync(id);

    // Cria um novo veículo
    public async Task<Vehicle> Create(CreateVehicleDto vehicleDto)
    {
        var vehicle = new Vehicle
        {
            Make = vehicleDto.Make,
            Model = vehicleDto.Model,
            Year = vehicleDto.Year,
            LicensePlate = vehicleDto.LicensePlate
        };
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }

    // Atualiza um veículo existente
    public async Task<Vehicle> Update(int id, CreateVehicleDto vehicleDto)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return null;

        vehicle.Make = vehicleDto.Make;
        vehicle.Model = vehicleDto.Model;
        vehicle.Year = vehicleDto.Year;
        vehicle.LicensePlate = vehicleDto.LicensePlate;
        await _context.SaveChangesAsync();
        return vehicle;
    }

    // Deleta um veículo por ID
    public async Task<bool> Delete(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return false;

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }
}