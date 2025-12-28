using VeterinariaAPI.Models.Cita;

namespace VeterinariaAPI.Repository.Interfaces;

public interface ICita
{

    string AgregarCita(CitaO obj);
    string ModificarCita(CitaO obj);
    string ActualizarEstadoCita(long idCita, string estado);
    CitaO BuscarCita(long id);
    Cita BuscarCitaFront(long id);
    void EliminarCita(long id);

    // Historial Médico
    string AgregarHistorialMedico(HistorialMedicoDTO dto);
    HistorialMedico? ObtenerHistorialPorCita(long idCita);
}