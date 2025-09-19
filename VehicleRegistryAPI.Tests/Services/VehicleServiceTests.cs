using Microsoft.EntityFrameworkCore;
using Moq;
using VehicleRegistryAPI.Data;
using VehicleRegistryAPI.Models;
using VehicleRegistryAPI.Services;
using VehicleRegistryAPI.Dtos;

public class VehicleServiceTests
{
    [Fact]
    public async Task Delete_WhenVehicleExists_ShouldReturnTrue()
    {
        // Arrange (Organizar)
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Delete_Success")
            .Options;

        var vehicleId = 1;
        var vehicle = new Vehicle { Id = vehicleId, Make = "Test", Model = "Car", Year = 2025, LicensePlate = "ABC1234" };

        // Uma base de dados em memória para simular a base de dados real
        using (var context = new ApplicationDbContext(options))
        {
            context.Vehicles.Add(vehicle);
            context.SaveChanges();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var service = new VehicleService(context);

            // Act (Agir)
            var result = await service.Delete(vehicleId);

            // Assert (Verificar)
            Assert.True(result);
            Assert.Equal(0, await context.Vehicles.CountAsync()); // Garante que o veículo foi realmente apagado
        }
    }

    [Fact]
    public async Task Create_WithValidData_ShouldAddVehicleToDatabase()
    {
        // Arrange (Organizar)
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Create_Success")
            .Options;

        var vehicleDto = new CreateVehicleDto("Ford", "Ranger", 2024, "XYZ9876");

        // Usando um 'using' para garantir que o contexto da base de dados é descartado corretamente
        await using (var context = new ApplicationDbContext(options))
        {
            var service = new VehicleService(context);

            // Act (Agir)
            var createdVehicle = await service.Create(vehicleDto);

            // Assert (Verificar)
            Assert.NotNull(createdVehicle);
            Assert.Equal("Ford", createdVehicle.Make);
            // A verificação mais importante: confirma que há 1 item na base de dados
            Assert.Equal(1, await context.Vehicles.CountAsync());
        }
    }

    [Fact]
    public async Task Update_WhenVehicleExists_ShouldUpdateData()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Update_Success")
            .Options;

        var initialVehicle = new Vehicle { Id = 1, Make = "VW", Model = "Nivus", Year = 2023, LicensePlate = "ABC1234" };
        await using (var context = new ApplicationDbContext(options))
        {
            context.Vehicles.Add(initialVehicle);
            await context.SaveChangesAsync();
        }

        var updateDto = new CreateVehicleDto("Volkswagen", "Nivus Highline", 2023, "ABC1234");

        await using (var context = new ApplicationDbContext(options))
        {
            var service = new VehicleService(context);

            // Act
            var updatedVehicle = await service.Update(1, updateDto);

            // Assert
            Assert.NotNull(updatedVehicle);
            Assert.Equal("Volkswagen", updatedVehicle.Make); // Verifica se o "Make" foi atualizado
            Assert.Equal("Nivus Highline", updatedVehicle.Model); // Verifica se o "Model" foi atualizado
        }
    }

    [Fact]
    public async Task GetById_WhenVehicleExists_ShouldReturnVehicle()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetById_Success")
            .Options;

        var vehicle = new Vehicle { Id = 1, Make = "Fiat", Model = "Toro", Year = 2022, LicensePlate = "DEF5678" };
        await using (var context = new ApplicationDbContext(options))
        {
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
        }

        await using (var context = new ApplicationDbContext(options))
        {
            var service = new VehicleService(context);

            // Act
            var foundVehicle = await service.GetById(1);

            // Assert
            Assert.NotNull(foundVehicle);
            Assert.Equal(1, foundVehicle.Id);
            Assert.Equal("Fiat", foundVehicle.Make);
        }
    }
}