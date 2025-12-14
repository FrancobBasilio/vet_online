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
            mensaje = "Error al registrar la cita: " + ex.Message;
        }
        return mensaje;
    }







    public CitaO BuscarCita(long id)
    {
        return ListarCitasO().FirstOrDefault(c => c.IdCita == id);
    }

    public Cita BuscarCitaFront(long id)
    {
        return ListarCitas().FirstOrDefault(c => c.IdCita == id);
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

    public IEnumerable<Cita> ListarCitasPorFecha(int dia, int mes, int año)
    {
        var lista = new List<Cita>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_obtenerCitasPorFecha", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@dia", dia);
        cmd.Parameters.AddWithValue("@mes", mes);
        cmd.Parameters.AddWithValue("@año", año);

        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            lista.Add(new Cita
            {
                IdCita = Convert.ToInt64(dr["ide_cit"]),
                CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                Consultorio = Convert.ToInt64(dr["con_cit"]),
                NombreVeterinario = dr["Veterinario"].ToString(),
                NombreMascota = dr["Mascota"].ToString(),
                MontoPago = Convert.ToDecimal(dr["mon_pag"])
            });
        }
        return lista;
    }

    public IEnumerable<Cita> ListarCitas()
    {
        var lista = new List<Cita>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasFront", cn);
        cmd.CommandType = CommandType.StoredProcedure;

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
                EstadoCita = dr["est_cit"].ToString() ?? "P"
            });
        }
        return lista;
    }

    public IEnumerable<CitaO> ListarCitasO()
    {
        var lista = new List<CitaO>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasBack", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            lista.Add(new CitaO
            {
                IdCita = Convert.ToInt64(dr["ide_cit"]),
                CalendarioCita = Convert.ToDateTime(dr["cal_cit"]),
                Consultorio = Convert.ToInt64(dr["con_cit"]),
                IdVeterinario = Convert.ToInt64(dr["ide_vet"]),
                IdMascota = Convert.ToInt64(dr["ide_mas"]),
                IdPago = Convert.ToInt64(dr["ide_pag"]),
                EstadoCita = dr["est_cit"].ToString() ?? "P"
            });
        }
        return lista;
    }





    public string ModificarCita(CitaO obj)
    {
        string mensaje = "";
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
            mensaje = "Error al actualizar la cita: " + ex.Message;
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