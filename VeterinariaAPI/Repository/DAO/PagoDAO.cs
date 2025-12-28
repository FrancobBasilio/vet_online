using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using VeterinariaAPI.Models.Pago;
using VeterinariaAPI.Repository.Interfaces;

namespace VeterinariaAPI.Repository.DAO;

public class PagoDAO : IPago
{
    private readonly string _connectionString;

    public PagoDAO()
    {
        _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("cn") ?? throw new NullReferenceException();
    }

    public IEnumerable<Pago> ListarPagos()
    {
        List<Pago> pagos = new List<Pago>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarPagos", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            pagos.Add(new Pago()
            {
                IdPago = Convert.ToInt64(dr[0]),
                HoraPago = Convert.ToDateTime(dr[1]),
                MontoPago = Convert.ToDecimal(dr[2]),
                TipoPago = dr[3].ToString(),
                CorreoCliente = dr[4].ToString(), 
                NombreCliente = dr[5].ToString()  
            });
        }
        return pagos;
    }



    // Listar PAGOS PENDIENTES para Recep
    public IEnumerable<Pago> ListarPagosPendientes()
    {
        List<Pago> pagos = new List<Pago>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarPagosPendientes", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            pagos.Add(new Pago()
            {
                IdPago = Convert.ToInt64(dr["ide_pag"]),
                HoraPago = Convert.ToDateTime(dr["hor_pag"]),
                MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                TipoPago = dr["nom_pay"].ToString(),
                CorreoCliente = dr["cor_usr"].ToString(),
                NombreCliente = dr["nombre_completo"].ToString(),
                EstadoPago = "Pendiente" 
            });
        }
        return pagos;
    }

    //  Listar PAGOS REALIZADOS Recep
    public IEnumerable<Pago> ListarPagosRealizados()
    {
        List<Pago> pagos = new List<Pago>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarPagosRealizados", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            pagos.Add(new Pago()
            {
                IdPago = Convert.ToInt64(dr["ide_pag"]),
                HoraPago = Convert.ToDateTime(dr["hor_pag"]),
                MontoPago = Convert.ToDecimal(dr["mon_pag"]),
                TipoPago = dr["nom_pay"].ToString(),
                CorreoCliente = dr["cor_usr"].ToString(),
                NombreCliente = dr["nombre_completo"].ToString(),
                EstadoPago = "Realizado" 
            });
        }
        return pagos;
    }



    public IEnumerable<Pago> ListarPagosPorCliente(long id)
    {
        List<Pago> pagos = new List<Pago>();
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_listarPagosPorCliente", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ide_usr", id);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            pagos.Add(new Pago()
            {
                IdPago = Convert.ToInt64(dr[0]),
                HoraPago = Convert.ToDateTime(dr[1]),
                MontoPago = Convert.ToDecimal(dr[2]),
                TipoPago = dr[3].ToString(),
                CorreoCliente = dr[4].ToString(),
                NombreCliente = dr[5].ToString(),
             
                EstadoPago = dr["EstadoPago"].ToString()
            });
        }
        return pagos;
    }

    public long AgregarPago(PagoO pago, long token)
    {
        long idGenerado = 0;
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_agregarPago", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@hora", pago.HoraPago);
        cmd.Parameters.AddWithValue("@monto", pago.MontoPago);
        cmd.Parameters.AddWithValue("@tipopago", pago.TipoPago);
        cmd.Parameters.AddWithValue("@usuario", token);
        cn.Open();
        var result = cmd.ExecuteScalar();
        if (result != null) idGenerado = Convert.ToInt64(result);
        return idGenerado;
    }

    public PagoO ObtenerPagoPorId(long id)
    {
        PagoO pago = null;
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_obtenerPagoPorId", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", id);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            pago = new PagoO()
            {
                IdPago = Convert.ToInt64(dr[0]),
                HoraPago = Convert.ToDateTime(dr[1]),
                MontoPago = Convert.ToDecimal(dr[2]),
                TipoPago = Convert.ToInt64(dr[3]),
                IdCliente = Convert.ToInt64(dr[4]) 
            };
        }
        return pago;
    }

    public Pago ObtenerPagoPorIdFront(long id)
    {
        Pago pago = null;
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_obtenerPagoPorIdFront", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", id);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            pago = new Pago()
            {
                IdPago = Convert.ToInt64(dr[0]),
                HoraPago = Convert.ToDateTime(dr[1]),
                MontoPago = Convert.ToDecimal(dr[2]),
                TipoPago = dr[3].ToString(),
                NombreCliente = dr[4].ToString(), 
                CorreoCliente = dr[5].ToString()  
            };
        }
        return pago;
    }

    public string ActualizarPago(PagoO pago)
    {
        string respuesta = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_actualizarPago", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@cliente", pago.IdCliente); 
        cmd.Parameters.AddWithValue("@ide_pag", pago.IdPago);
        cmd.Parameters.AddWithValue("@hor_pag", pago.HoraPago);
        cmd.Parameters.AddWithValue("@mon_pag", pago.MontoPago);
        cmd.Parameters.AddWithValue("@ide_pay", pago.TipoPago); 
        cn.Open();
        cmd.ExecuteNonQuery();
        respuesta = "Pago actualizado correctamente";
        return respuesta;
    }


    public string EliminarPago(long id)
    {
        string respuesta = "";
        using var cn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_eliminarPago", cn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@id", id);
        cn.Open();
        cmd.ExecuteNonQuery();
        respuesta = "Pago eliminado correctamente";
        return respuesta;
    }

}