using System.ComponentModel;

namespace VeterinariaWebApp.Models.Usuario.Veterinario;

public class CitaVeterinario
{
    [DisplayName("CODIGO CITA")]
    public long ide_cit { get; set; }

    [DisplayName("FECHA")]
    public DateTime cal_cit { get; set; }

    [DisplayName("CONSULTORIO")]
    public int con_cit { get; set; }

    [DisplayName("MASCOTA")]
    public string mascota { get; set; }

    [DisplayName("ESPECIE")]
    public string especie { get; set; }

    [DisplayName("MONTO PAGAR")]
    public decimal mon_pag { get; set; }
}