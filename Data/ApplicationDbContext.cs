using Microsoft.EntityFrameworkCore;
using VehicleRegistryAPI.Models;

namespace VehicleRegistryAPI.Data;

public class ApplicationDbContext : DbContext
{
    // Construtor que recebe as opções de configuração do DbContext
    // e as passa para a classe base DbContext
    // Essas opções geralmente incluem a string de conexão com o banco de dados
    // e outras configurações específicas do Entity Framework
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Cria uma tabela chamada 'Vehicles' do tipo 'Vehicle' no banco de dados
    public DbSet<Vehicle> Vehicles { get; set; }
}