using System.ComponentModel;

namespace VeterinariaWebApp.Models.Mascota;

public class Mascota
{
    [DisplayName("ID")]
    public long IdMascota { get; set; }

    [DisplayName("Nombre")]
    public string? Nombre { get; set; }

    [DisplayName("Especie")]
    public string? Especie { get; set; }

    [DisplayName("Raza")]
    public string? Raza { get; set; }

    [DisplayName("Fecha de Nacimiento")]
    public DateTime FechaNacimiento { get; set; }
}