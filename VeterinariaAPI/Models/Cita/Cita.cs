namespace VeterinariaAPI.Models.Cita;

public class Cita
{
    public long IdCita { get; set; }
    public DateTime CalendarioCita { get; set; }
    public long Consultorio { get; set; }
    public string? NombreVeterinario { get; set; } 
    public string? NombreMascota { get; set; }    
    public decimal MontoPago { get; set; }
}