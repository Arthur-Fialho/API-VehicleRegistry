using Microsoft.EntityFrameworkCore;
using VehicleRegistryAPI.Data;
using VehicleRegistryAPI.Dtos;
using VehicleRegistryAPI.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework Core.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configuração da autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!)),
            ValidateIssuer = false, // Em produção, valide o emissor
            ValidateAudience = false // Em produção, valide a audiência
        };
    });

// Adiciona serviços ao contêiner.
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Endpoint para buscar todos os veículos
app.MapGet("/vehicles", async (ApplicationDbContext context) =>
{
    var vehicles = await context.Vehicles.ToListAsync();
    return Results.Ok(vehicles);
});

// Endpoint para buscar um veículo por ID
app.MapGet("/vehicles/{id}", async (int id, ApplicationDbContext context) =>
{
    // FindAsync é otimizado para buscar pela chave primária
    var vehicle = await context.Vehicles.FindAsync(id);

    if (vehicle is null)
    {
        // Retorna um 404 Not Found se o veículo não for encontrado
        return Results.NotFound(new { Message = $"Veículo com ID {id}, não foi encontrado." });
    }

    // Retorna um 200 OK com o veículo encontrado
    return Results.Ok(vehicle);
});

// Endpoint para criar um novo veículo
app.MapPost("/vehicles", async (CreateVehicleDto vehicleDto, ApplicationDbContext context) =>
{
    // Mapeamento manual do DTO para a Entidade Vehicle
    var vehicle = new Vehicle
    {
        Make = vehicleDto.Make,
        Model = vehicleDto.Model,
        Year = vehicleDto.Year,
        LicensePlate = vehicleDto.LicensePlate
    };

    await context.Vehicles.AddAsync(vehicle); // Adiciona o novo veículo ao contexto do EF
    await context.SaveChangesAsync(); // Salva as alterações na base de dados (executa o INSERT)

    // Retorna um status 201 Created com a localização do novo recurso e o objeto criado
    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
});

// Endpoint para atualizar um veículo existente
app.MapPut("/vehicles/{id}", async (int id, CreateVehicleDto vehicleDto, ApplicationDbContext context) =>
{
    var vehicle = await context.Vehicles.FindAsync(id);
    if (vehicle is null)
    {
        return Results.NotFound(new { Message = $"Veículo com ID {id}, não foi encontrado." });
    }

    // Atualiza as propriedades da entidade encontrada com os dados do DTO
    vehicle.Make = vehicleDto.Make;
    vehicle.Model = vehicleDto.Model;
    vehicle.Year = vehicleDto.Year;
    vehicle.LicensePlate = vehicleDto.LicensePlate;

    await context.SaveChangesAsync(); // Salva as alterações (executa o UPDATE)

    return Results.Ok(vehicle);
});

// Endpoint para deletar um veículo
app.MapDelete("/vehicles/{id}", async (int id, ApplicationDbContext context) =>
{
    var vehicle = await context.Vehicles.FindAsync(id);
    if (vehicle is null)
    {
        return Results.NotFound(new { Message = $"Veículo com ID {id}, não foi encontrado." });
    }

    context.Vehicles.Remove(vehicle); // Marca a entidade para deleção
    await context.SaveChangesAsync(); // Salva as alterações (executa o DELETE)

    // Retorna um status 204 No Content, o padrão para um delete bem-sucedido
    return Results.NoContent();
});

app.Run();