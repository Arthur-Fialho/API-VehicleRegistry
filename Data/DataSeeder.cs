using VehicleRegistryAPI.Models;

namespace VehicleRegistryAPI.Data;

public static class DataSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // Se já existirem utilizadores, não faz nada
        if (context.Users.Any())
        {
            return;
        }

        // Adiciona utilizadores iniciais
        var users = new User[]
        {
            new User { Username = "editor", PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = "Editor" },
            new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("senhaforte"), Role = "Administrator" }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}