using VeterinariaAPI.Models.Usuario.Veterinario;
using VeterinariaAPI.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace VeterinariaAPI.Repository.DAO;

public class VeterinarioDAO : IVeterinario
{
    private readonly string _connectionString;

    public VeterinarioDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("cn") ?? throw new NullReferenceException();
    }

    public IEnumerable<Veterinario> ListarVeterinariosFront()
    {
        var listaVeterinarios = new List<Veterinario>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarVeterinariosFront", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            listaVeterinarios.Add(new Veterinario()
            {
                IdVeterinario = Convert.ToInt64(dr["ide_vet"]),
                NombreUsuario = dr["nom_usr"].ToString(),
                ApellidoUsuario = dr["ape_usr"].ToString(),
                FechaNacimiento = Convert.ToDateTime(dr["fna_usr"]),
                TipoDocumento = dr["nom_doc"].ToString(),
                NumeroDocumento = dr["num_doc"].ToString(),
                Rol = dr["nom_rol"].ToString(),
                sueldo = Convert.ToDecimal(dr["sue_vet"]),
                especialidad = dr["nom_esp"].ToString()
            });
        }
        return listaVeterinarios;
    }

    public IEnumerable<VeterinarioO> ListarVeterinariosBack()
    {
        var listaVeterinarios = new List<VeterinarioO>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarVeterinariosBack", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            listaVeterinarios.Add(new VeterinarioO()
            {
                ide_usr = Convert.ToInt64(dr["ide_usr"]),
                ide_vet = Convert.ToInt64(dr["ide_vet"]),
                sue_vet = Convert.ToDecimal(dr["sue_vet"]),
                ide_esp = Convert.ToInt64(dr["ide_esp"]),
                cor_usr = dr["cor_usr"].ToString(),
                pwd_usr = dr["pwd_usr"].ToString(),
                nom_usr = dr["nom_usr"].ToString(),
                ape_usr = dr["ape_usr"].ToString(),
                fna_usr = Convert.ToDateTime(dr["fna_usr"]),
                num_doc = dr["num_doc"].ToString(),
                ide_doc = Convert.ToInt64(dr["ide_doc"]),
                ide_rol = Convert.ToInt64(dr["ide_rol"])
            });
        }
        return listaVeterinarios;
    }

    public string AgregarVeterinario(VeterinarioO veterinario)
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_agregarVeterinario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@cor", veterinario.cor_usr);
        cmd.Parameters.AddWithValue("@pwd", veterinario.pwd_usr);
        cmd.Parameters.AddWithValue("@nom", veterinario.nom_usr);
        cmd.Parameters.AddWithValue("@ape", veterinario.ape_usr);
        cmd.Parameters.AddWithValue("@ndo", veterinario.num_doc);
        cmd.Parameters.AddWithValue("@fna", veterinario.fna_usr);
        cmd.Parameters.AddWithValue("@doc", veterinario.ide_doc);
        cmd.Parameters.AddWithValue("@sue", veterinario.sue_vet);
        cmd.Parameters.AddWithValue("@esp", veterinario.ide_esp);

        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Veterinario guardado correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al guardar veterinario: " + ex.Message;
        }
        return mensaje;
    }

    public Veterinario BuscarVeterinarioPorID(long id)
    {
        Veterinario veterinario = new();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_buscarVeterinarioPorId", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", id);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            veterinario = new Veterinario()
            {
                IdVeterinario = Convert.ToInt64(dr["ide_vet"]),
                NombreUsuario = dr["nom_usr"].ToString(),
                ApellidoUsuario = dr["ape_usr"].ToString(),
                FechaNacimiento = Convert.ToDateTime(dr["fna_usr"]),
                TipoDocumento = dr["nom_doc"].ToString(),
                NumeroDocumento = dr["num_doc"].ToString(),
                Rol = dr["nom_rol"].ToString(),
                sueldo = Convert.ToDecimal(dr["sue_vet"]),
                especialidad = dr["nom_esp"].ToString()
            };
        }
        return veterinario;
    }

    public string ActualizarVeterinarioPorID(VeterinarioO veterinario)
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_actualizarVeterinario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", veterinario.ide_vet);
        cmd.Parameters.AddWithValue("@sue", veterinario.sue_vet);
        cmd.Parameters.AddWithValue("@esp", veterinario.ide_esp);
        cmd.Parameters.AddWithValue("@cor", veterinario.cor_usr);
        cmd.Parameters.AddWithValue("@pwd", veterinario.pwd_usr);
        cmd.Parameters.AddWithValue("@nom", veterinario.nom_usr);
        cmd.Parameters.AddWithValue("@ape", veterinario.ape_usr);
        cmd.Parameters.AddWithValue("@ndo", veterinario.num_doc);
        cmd.Parameters.AddWithValue("@fna", veterinario.fna_usr);
        cmd.Parameters.AddWithValue("@doc", veterinario.ide_doc);
        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Veterinario actualizado correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al actualizar veterinario: " + ex.Message;
        }
        return mensaje;
    }

    public string EliminarVeterinarioPorID(long id)
    {
        string mensaje = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_eliminarVeterinario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", id);
        try
        {
            cn.Open();
            cmd.ExecuteNonQuery();
            mensaje = "Veterinario eliminado correctamente";
        }
        catch (Exception ex)
        {
            mensaje = "Error al eliminar veterinario: " + ex.Message;
        }
        return mensaje;
    }

    public IEnumerable<CitaVeterinario> ListarCitasPorVeterinario(long ide_usr)
    {
        var listaCitas = new List<CitaVeterinario>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarCitasPorVeterinario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ide_usr", ide_usr);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            listaCitas.Add(new CitaVeterinario()
            {
                ide_cit = Convert.ToInt64(dr["ide_cit"]),
                cal_cit = Convert.ToDateTime(dr["cal_cit"]),
                con_cit = Convert.ToInt32(dr["con_cit"]),
                mascota = dr["mascota"].ToString(),
                especie = dr["especie"].ToString(),
                mon_pag = Convert.ToDecimal(dr["mon_pag"])
            });
        }
        return listaCitas;
    }

    public VeterinarioStats ObtenerEstadisticasVeterinario(long ide_usr)
    {
        VeterinarioStats stats = new();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_totalesPorVeterinario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ide_usr", ide_usr);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            stats.TotalCitas = Convert.ToInt32(dr["total_citas"]);
            stats.TotalMascotas = Convert.ToInt32(dr["total_mascotas"]);
        }
        return stats;
    }

    public IEnumerable<MascotaPorVeterinario> ListarMascotasPorVeterinario(long ide_usr)
    {
        var listaMascotas = new List<MascotaPorVeterinario>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarMascotasPorVeterinario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ide_usr", ide_usr);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            listaMascotas.Add(new MascotaPorVeterinario()
            {
                ide_mas = Convert.ToInt64(dr["ide_mas"]),
                Mascota = dr["Mascota"].ToString(),
                Especie = dr["Especie"].ToString(),
                Raza = dr["Raza"].ToString(),
                Doc_Dueño = dr["Doc_Dueño"].ToString(),
                Total_Citas = Convert.ToInt32(dr["Total_Citas"])
            });
        }
        return listaMascotas;
    }
}