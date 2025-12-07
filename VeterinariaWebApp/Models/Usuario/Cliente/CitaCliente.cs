using System.ComponentModel;

namespace VeterinariaWebApp.Models.Usuario.Cliente;

public class CitaCliente
{
    [DisplayName("ID CITA")]
    public long ide_cit { get; set; }

    [DisplayName("FECHA")]
    public DateTime cal_cit { get; set; }

    [DisplayName("CONSULTORIO")]
    public int con_cit { get; set; }

    [DisplayName("VETERINARIO")]
    public string veterinario { get; set; }

    [DisplayName("ESPECIALIDAD")]
    public string especialidad { get; set; }

    [DisplayName("MASCOTA")]
    public string mascota { get; set; }

    [DisplayName("ESPECIE")]
    public string especie { get; set; }

    [DisplayName("MONTO A PAGAR")]
    public decimal mon_pag { get; set; }
}