using VeterinariaAPI.Models.Cita;
using VeterinariaAPI.Repository.Interfaces;
using System.Data.SqlClient;
using System.Data;

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
                MontoPago = Convert.ToDecimal(dr["mon_pag"])
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
                IdPago = Convert.ToInt64(dr["ide_pag"])
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







}