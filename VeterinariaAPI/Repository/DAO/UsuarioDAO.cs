using System.Data;
using System.Data.SqlClient;
using VeterinariaAPI.Models.Usuario;
using VeterinariaAPI.Repository.Interfaces;

namespace VeterinariaAPI.Repository.DAO;

public class UsuarioDAO : IUsuario
{
    private readonly string _connectionString;

    public UsuarioDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("cn") ?? throw new NullReferenceException();
    }

    public string verificarLogin(string uid, string pwd)
    {
        string resultado = "denied";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_verificarLogin", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@correo", uid);
        cmd.Parameters.AddWithValue("@contraseña", pwd);
        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                resultado = dr.GetString(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en login: {ex.Message}");
        }
        return resultado;
    }

    public string obtenerIdUsuario(string correo)
    {
        string resultado = "denied";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_obtenerIdUsuario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@correo", correo);
        try
        {
            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                resultado = dr[0].ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener ID de usuario: {ex.Message}");
        }
        return resultado;
    }
}