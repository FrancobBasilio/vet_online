namespace VeterinariaAPI.Models.Mascota;

public class MascotaO
{
    public long ide_mas { get; set; } // ID de la mascota
    public string nom_mas { get; set; } // Nombre de la mascota
    public string esp_mas { get; set; } // Especie
    public string raz_mas { get; set; } // Raza
    public DateTime fna_mas { get; set; } // Fecha de nacimiento
    public long ide_cli { get; set; } // ID del cliente (dueño)
}