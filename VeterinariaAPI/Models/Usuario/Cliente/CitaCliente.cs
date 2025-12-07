namespace VeterinariaAPI.Models.Usuario.Cliente;

public class CitaCliente
{
    public long ide_cit { get; set; }
    public DateTime cal_cit { get; set; }
    public int con_cit { get; set; }
    public string veterinario { get; set; } // Cambiado de "medico"
    public string especialidad { get; set; }
    public string mascota { get; set; }    // Añadido: nombre de la mascota
    public string especie { get; set; }    // Añadido: especie de la mascota
    public decimal mon_pag { get; set; }
}