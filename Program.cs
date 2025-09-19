using Microsoft.EntityFrameworkCore;
using VehicleRegistryAPI.Data;
using VehicleRegistryAPI.Dtos;
using VehicleRegistryAPI.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VehicleRegistryAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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
builder.Services.AddAuthorization(options =>
{
    // Define uma política chamada "Administrator"
    // que exige que o utilizador tenha o perfil ("Role") "Administrator".
    options.AddPolicy("Administrator", policy =>
        policy.RequireRole("Administrator"));
});

// Adiciona controladores (Controllers) ao projeto
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger para suportar autenticação JWT
builder.Services.AddSwaggerGen(options =>
{
    // Adiciona a definição de segurança para JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer ' seguido do seu token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Garante que o Swagger envie o token em todas as requisições protegidas
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Regista o TokenService para injeção de dependência
builder.Services.AddScoped<TokenService>();

// Regista o VehicleService para injeção de dependência
builder.Services.AddScoped<IVehicleService, VehicleService>();

var app = builder.Build();

// Seed da base de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DataSeeder.Seed(context);
}

// Configuração do pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Grupo para todos os endpoints de "/vehicles"
// onde é exigida autorização para TODO o grupo.
var vehicleEndpoints = app.MapGroup("/vehicles").RequireAuthorization();

// GET (Leitura) - Acessível a qualquer utilizador autenticado (Editor ou Administrator)
vehicleEndpoints.MapGet("/", async (IVehicleService service) =>
{
    var vehicles = await service.GetAll();
    return Results.Ok(vehicles);
});

// GET by ID (Leitura) - Acessível a qualquer utilizador autenticado (Editor ou Administrator)
vehicleEndpoints.MapGet("/{id}", async (int id, IVehicleService service) =>
{
    var vehicle = await service.GetById(id);
    return vehicle is not null ? Results.Ok(vehicle) : Results.NotFound();
});

// POST (Criação) - Acessível a qualquer utilizador autenticado (Editor ou Administrator)
vehicleEndpoints.MapPost("/", async (CreateVehicleDto vehicleDto, IVehicleService service) =>
{
    var vehicle = await service.Create(vehicleDto);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
});


// PUT (Atualização) - RESTRITO A ADMINISTRADORES
vehicleEndpoints.MapPut("/{id}", async (int id, CreateVehicleDto vehicleDto, IVehicleService service) =>
{
    var vehicle = await service.Update(id, vehicleDto);
    if (vehicle is null) return Results.NotFound($"Veículo não encontrado com o id {id}.");

    return Results.Ok(vehicle);
})
.RequireAuthorization("Administrator"); // Somente administradores podem atualizar

// DELETE (Remoção) - RESTRITO A ADMINISTRADORES
vehicleEndpoints.MapDelete("/{id}", async (int id, IVehicleService service) =>
{
    var vehicle = await service.Delete(id);
    if (!vehicle) return Results.NotFound($"Veículo não encontrado com o id {id}.");

    return Results.NoContent();
})
.RequireAuthorization("Administrator"); // Somente administradores podem deletar

app.MapPost("/login", async ([FromBody] LoginRequestDto loginDto, ApplicationDbContext context, TokenService tokenService) =>
{
    var user = await context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

    if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    var token = tokenService.GenerateToken(user);

    return Results.Ok(new LoginResponseDto(token));
});

app.Run();