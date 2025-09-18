using Microsoft.EntityFrameworkCore;
using VehicleRegistryAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework Core.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Adiciona serviços ao contêiner.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.Run();