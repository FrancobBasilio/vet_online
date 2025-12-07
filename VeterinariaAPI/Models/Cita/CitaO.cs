namespace VeterinariaAPI.Models.Cita;

public class CitaO
{
    public long IdCita { get; set; }
    public DateTime CalendarioCita { get; set; }
    public long Consultorio { get; set; }
    public long IdVeterinario { get; set; } 
    public long IdMascota { get; set; }    
    public long IdPago { get; set; }
}