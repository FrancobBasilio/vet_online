using VeterinariaAPI.Models.Cita;

namespace VeterinariaAPI.Repository.Interfaces;

public interface ICita
{
    IEnumerable<Cita> ListarCitas();
    IEnumerable<CitaO> ListarCitasO();
    string AgregarCita(CitaO obj);
    string ModificarCita(CitaO obj);
    string ActualizarEstadoCita(long idCita, string estado);
    CitaO BuscarCita(long id);
    Cita BuscarCitaFront(long id);
    void EliminarCita(long id);
    IEnumerable<Cita> ListarCitasPorFecha(int dia, int mes, int año);

    // Historial Médico
    string AgregarHistorialMedico(HistorialMedicoDTO dto);
    HistorialMedico? ObtenerHistorialPorCita(long idCita);
}