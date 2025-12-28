using VeterinariaAPI.Models.Cita;
using VeterinariaAPI.Repository.Interfaces;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;

namespace VeterinariaAPI.Repository.DAO;

public class CitaDAO : ICita
{
    private readonly string _connectionString;

    public CitaDAO()
    {
        _connectionString = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .GetConnectionString("cn") ?? throw new NullReferenceException("Cadena de conexión no encontrada.");
    }




    public string AgregarCita(CitaO obj)
    {
        string mensaje = "";

       
        if (ExisteCitaEnHorario(0, obj.CalendarioCita, obj.IdVeterinario, obj.Consultorio))
        {
            return "Lo sentimos, ese horario ya está ocupado. Por favor, elija otra fecha u hora.";
        }

        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_agregarCita", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@calendario", obj.CalendarioCita);
        cmd.Parameters.AddWithValue("@consultorio", obj.Consultorio);
        cmd.Parameters.AddWithValue("@veterinario", obj.IdVeterinario);
        cmd.Parameters.AddWithValue("@mascota", obj.IdMascota);
        cmd.Parameters.AddWithValue("@pago", obj.IdPago);

        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Cita registrada correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al registrar la cita:" + ex.Message;
        }
        return mensaje;
    }




    public CitaO BuscarCita(long id)
    {
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_buscarCitaPorId", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@idCita", id);

        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new CitaO
                {
                    IdCita = Convert.ToInt64(dr["ide_cit"]),
                    CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                    Consultorio = Convert.ToInt64(dr["con_cit"]),
                    IdVeterinario = Convert.ToInt64(dr["ide_vet"]),
                    IdMascota = Convert.ToInt64(dr["ide_mas"]),
                    IdPago = Convert.ToInt64(dr["ide_pag"]),
                    EstadoCita = dr["est_cit"].ToString()
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al buscar cita por ID: {ex.Message}");
        }
        return null;
    }



    public Cita BuscarCitaFront(long id)
    {
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_buscarCitaPorId", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@idCita", id);

        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new Cita
                {
                    IdCita = Convert.ToInt64(dr["ide_cit"]),
                    CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                    Consultorio = Convert.ToInt64(dr["con_cit"]),
                    NombreVeterinario = dr["NombreVeterinario"].ToString(),
                    NombreMascota = dr["NombreMascota"].ToString(),
                    MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                    EstadoCita = dr["est_cit"].ToString()
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al buscar cita front por ID: {ex.Message}");
        }
        return null;
    }

    public void EliminarCita(long id)
    {
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_eliminarCita", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@idCita", id);

        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar la cita: {ex.Message}");
        }
    }


    //VALIDACION DE DISPONIBILIDAD DE CITA
    private bool ExisteCitaEnHorario(long idCita, DateTime fechaHora, long idVeterinario, long idConsultorio)
    {
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_verificarDisponibilidadCita", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@idCita", idCita);
        cmd.Parameters.AddWithValue("@fechaHora", fechaHora);
        cmd.Parameters.AddWithValue("@idVeterinario", idVeterinario);
        cmd.Parameters.AddWithValue("@idConsultorio", idConsultorio);

        try
        {
            cn.Open();
            var result = cmd.ExecuteScalar();
            return result != null && Convert.ToInt64(result) > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al verificar disponibilidad: {ex.Message}");
            return false;
        }
    }






    //LISTADO DE CITAS PENDIENTES, VENCIDAS, ATENDIDAS, CANCELADAS

    public List<Cita> ListarCitasPendientes()
    {
        var lista = new List<Cita>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasPendientes", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Cita
                {
                    IdCita = Convert.ToInt64(dr["ide_cit"]),
                    CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                    Consultorio = Convert.ToInt64(dr["con_cit"]),
                    NombreVeterinario = dr["NombreVeterinario"].ToString(),
                    NombreMascota = dr["NombreMascota"].ToString(),
                    MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                    EstadoCita = dr["est_cit"].ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al listar citas pendientes: " + ex.Message);
        }
        return lista;
    }

    //--------------------------------------------------
    public List<Cita> ListarCitasVencidas()
    {
        var lista = new List<Cita>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasVencidas", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Cita
                {
                    IdCita = Convert.ToInt64(dr["ide_cit"]),
                    CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                    Consultorio = Convert.ToInt64(dr["con_cit"]),
                    NombreVeterinario = dr["NombreVeterinario"].ToString(),
                    NombreMascota = dr["NombreMascota"].ToString(),
                    MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                    EstadoCita = dr["est_cit"].ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al listar citas vencidas: " + ex.Message);
        }
        return lista;
    }

    public string CancelarCitaPorInasistencia(long idCita)
    {
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_cancelarCitaPorInasistencia", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@idCita", idCita);

        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            return "Cita cancelada por inasistencia correctamente.";
        }
        catch (Exception ex)
        {
            return "Error al cancelar la cita: " + ex.Message;
        }
    }

    //------------------------------------

    public List<Cita> ListarCitasAtendidas()
    {
        var lista = new List<Cita>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasAtendidas", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Cita
                {
                    IdCita = Convert.ToInt64(dr["ide_cit"]),
                    CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                    Consultorio = Convert.ToInt64(dr["con_cit"]),
                    NombreVeterinario = dr["NombreVeterinario"].ToString(),
                    NombreMascota = dr["NombreMascota"].ToString(),
                    MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                    EstadoCita = dr["est_cit"].ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al listar citas atendidas: " + ex.Message);
        }
        return lista;
    }

    //Listado de citas Canceladas

    public List<Cita> ListarCitasCanceladas()
    {
        var lista = new List<Cita>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasCanceladas", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Cita
                {
                    IdCita = Convert.ToInt64(dr["ide_cit"]),
                    CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                    Consultorio = Convert.ToInt64(dr["con_cit"]),
                    NombreVeterinario = dr["NombreVeterinario"].ToString(),
                    NombreMascota = dr["NombreMascota"].ToString(),
                    MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                    EstadoCita = dr["est_cit"].ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al listar citas canceladas: " + ex.Message);
        }
        return lista;
    }



    // LISTAR CITAS POR ESTADO 

    public List<Cita> ListarCitasPorEstado(string estado)
    {
        var lista = new List<Cita>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasPorEstado", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@estado", estado);

        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Cita
                {
                    IdCita = Convert.ToInt64(dr["ide_cit"]),
                    CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                    Consultorio = Convert.ToInt64(dr["con_cit"]),
                    NombreVeterinario = dr["NombreVeterinario"].ToString(),
                    NombreMascota = dr["NombreMascota"].ToString(),
                    MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                    EstadoCita = dr["est_cit"].ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al listar citas por estado: {ex.Message}");
        }
        return lista;
    }






    public string ModificarCita(CitaO obj)
    {
        string mensaje = "";

        // Verificar disponibilidad ANTES de actualizar
        if (ExisteCitaEnHorario(obj.IdCita, obj.CalendarioCita, obj.IdVeterinario, obj.Consultorio))
        {
            return "Lo sentimos, ese horario ya está ocupado. Por favor, elija otra fecha u hora.";
        }

        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_actualizarCita", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@idCita", obj.IdCita);
        cmd.Parameters.AddWithValue("@calendario", obj.CalendarioCita);
        cmd.Parameters.AddWithValue("@consultorio", obj.Consultorio);
        cmd.Parameters.AddWithValue("@veterinario", obj.IdVeterinario);
        cmd.Parameters.AddWithValue("@mascota", obj.IdMascota);
        cmd.Parameters.AddWithValue("@pago", obj.IdPago);

        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Cita actualizada correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al actualizar la cita:" + ex.Message;
        }
        return mensaje;
    }




    // Nuevo método para actualizar solo el estado de la cita
    public string ActualizarEstadoCita(long idCita, string estado)
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_actualizarEstadoCita", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@idCita", idCita);
        cmd.Parameters.AddWithValue("@estado", estado);
        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = estado switch
            {
                "P" => "Cita marcada como Pendiente",
                "E" => "Cita en proceso de atención",
                "A" => "Cita atendida correctamente",
                "C" => "Cita cancelada correctamente",
                _ => "Estado actualizado"
            };
        }
        catch (Exception ex)
        {
            mensaje = "Error al actualizar el estado: " + ex.Message;
        }
        return mensaje;
    }

    // ==================== HISTORIAL MÉDICO ====================

    // Agregar o actualizar historial médico
    public string AgregarHistorialMedico(HistorialMedicoDTO dto)
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_agregarHistorialMedico", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ide_cit", dto.IdCita);
        cmd.Parameters.AddWithValue("@sintomas", (object?)dto.Sintomas ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@diagnostico", dto.Diagnostico);
        cmd.Parameters.AddWithValue("@tratamiento", dto.Tratamiento);
        cmd.Parameters.AddWithValue("@medicamentos", (object?)dto.Medicamentos ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@observaciones", (object?)dto.Observaciones ?? DBNull.Value);
        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Historial médico guardado correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al guardar historial médico: " + ex.Message;
        }
        return mensaje;
    }

    // Obtener historial médico por ID de cita
    public HistorialMedico? ObtenerHistorialPorCita(long idCita)
    {
        HistorialMedico? historial = null;
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_obtenerHistorialPorCita", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ide_cit", idCita);

        cn.Open();
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            historial = new HistorialMedico
            {
                ide_his = Convert.ToInt64(dr["ide_his"]),
                ide_cit = Convert.ToInt64(dr["ide_cit"]),
                sintomas = dr["sintomas"] == DBNull.Value ? null : dr["sintomas"].ToString(),
                diagnostico = dr["diagnostico"].ToString() ?? "",
                tratamiento = dr["tratamiento"].ToString() ?? "",
                medicamentos = dr["medicamentos"] == DBNull.Value ? null : dr["medicamentos"].ToString(),
                observaciones = dr["observaciones"] == DBNull.Value ? null : dr["observaciones"].ToString(),
                fecha_atencion = Convert.ToDateTime(dr["fecha_atencion"])
            };
        }
        return historial;
    }
}