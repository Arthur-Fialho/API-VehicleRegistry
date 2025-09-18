using System.ComponentModel.DataAnnotations;

namespace VehicleRegistryAPI.Models
{
    public class Vehicle
    {
    public int Id { get; set; }

    [Required]  
    [StringLength(100)]
    public string Make { get; set; } = string.Empty; // Ex: "Toyota"

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty; // Ex: "Corolla"

    public int Year { get; set; }

    [Required]
    [StringLength(7)]
    public string LicensePlate { get; set; } = string.Empty; // Placa (ex: BRA2E19)
    }
}