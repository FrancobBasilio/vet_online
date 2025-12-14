using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using VeterinariaAPI.Models.Usuario.Recepcionista;
using VeterinariaAPI.Repository.Interfaces;

namespace VeterinariaAPI.Repository.DAO;

public class RecepcionistaDAO : IRecepcionista 
{
    private readonly string _connectionString;

    public RecepcionistaDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("cn") ?? throw new NullReferenceException();
    }

    public IEnumerable<Recepcionista> ListarRecepcionistasFront()
    {
        var listaRecepcionistas = new List<Recepcionista>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarRecepcionistasFront", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            listaRecepcionistas.Add(new Recepcionista()
            {
                IdRecepcionista = Convert.ToInt64(dr[0]),
                NombreUsuario = dr[1].ToString(),
                ApellidoUsuario = dr[2].ToString(),
                FechaNacimiento = Convert.ToDateTime(dr[3]),
                TipoDocumento = dr[4].ToString(),
                NumeroDocumento = dr[5].ToString(),
                Rol = dr[6].ToString(),
                Sueldo = Convert.ToDecimal(dr[7])
            });
        }
        return listaRecepcionistas;
    }

    public IEnumerable<RecepcionistaO> ListarRecepcionistasBack()
    {
        var listaRecepcionistas = new List<RecepcionistaO>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarRecepcionistasBack", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            listaRecepcionistas.Add(new RecepcionistaO()
            {
                ide_usr = Convert.ToInt64(dr[0]),
                ide_rep = Convert.ToInt64(dr[1]),
                sue_rep = Convert.ToDecimal(dr[2]),
                cor_usr = dr[3].ToString(),
                pwd_usr = dr[4].ToString(),
                nom_usr = dr[5].ToString(),
                ape_usr = dr[6].ToString(),
                fna_usr = Convert.ToDateTime(dr[7]),
                num_doc = dr[8].ToString(),
                ide_doc = Convert.ToInt64(dr[9]),
                ide_rol = Convert.ToInt64(dr[10])
            });
        }
        return listaRecepcionistas;
    }

    public string AgregarRecepcionista(RecepcionistaO recepcionista) // Estilo PascalCase
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_agregarRecepcionista", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@cor", recepcionista.cor_usr);
        cmd.Parameters.AddWithValue("@pwd", recepcionista.pwd_usr);
        cmd.Parameters.AddWithValue("@nom", recepcionista.nom_usr);
        cmd.Parameters.AddWithValue("@ape", recepcionista.ape_usr);
        cmd.Parameters.AddWithValue("@ndo", recepcionista.num_doc);
        cmd.Parameters.AddWithValue("@fna", recepcionista.fna_usr);
        cmd.Parameters.AddWithValue("@doc", recepcionista.ide_doc);
        cmd.Parameters.AddWithValue("@sue", recepcionista.sue_rep);

        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Recepcionista guardado correctamente";
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
            mensaje = "Error al guardar recepcionista";
        }
        return mensaje;
    }

    public Recepcionista BuscarRecepcionistaPorID(long id)
    {
        Recepcionista recepcionista = new();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_buscarRecepcionistaPorId", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", id);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            recepcionista = new Recepcionista()
            {
                IdRecepcionista = Convert.ToInt64(dr[0]),
                NombreUsuario = dr[1].ToString(),
                ApellidoUsuario = dr[2].ToString(),
                FechaNacimiento = Convert.ToDateTime(dr[3]),
                TipoDocumento = dr[4].ToString(),
                NumeroDocumento = dr[5].ToString(),
                Rol = dr[6].ToString(),
                Sueldo = Convert.ToDecimal(dr[7])
            };
        }
        return recepcionista;
    }

    public string ActualizarRecepcionistaPorID(RecepcionistaO recepcionista)
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_actualizarRecepcionista", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", recepcionista.ide_rep);
        cmd.Parameters.AddWithValue("@sue", recepcionista.sue_rep);
        cmd.Parameters.AddWithValue("@cor", recepcionista.cor_usr);
        cmd.Parameters.AddWithValue("@pwd", recepcionista.pwd_usr);
        cmd.Parameters.AddWithValue("@nom", recepcionista.nom_usr);
        cmd.Parameters.AddWithValue("@ape", recepcionista.ape_usr);
        cmd.Parameters.AddWithValue("@ndo", recepcionista.num_doc);
        cmd.Parameters.AddWithValue("@fna", recepcionista.fna_usr);
        cmd.Parameters.AddWithValue("@doc", recepcionista.ide_doc);
        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Recepcionista actualizado correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al actualizar recepcionista: " + ex.Message;
        }
        return mensaje;
    }

    public string EliminarRecepcionistaPorID(long id)
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_eliminarRecepcionista", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", id);
        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Recepcionista eliminado correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al eliminar recepcionista: " + ex.Message;
        }
        return mensaje;
    }
}