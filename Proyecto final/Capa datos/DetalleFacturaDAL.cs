using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
    public class DetalleFacturaDAL
    {
        // INSERTAR
        public bool Insertar(int facturaID, int tratamientoID, int cantidad, decimal precio)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("InsertarDetalleFactura", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FacturaID", facturaID);
                cmd.Parameters.AddWithValue("@TratamientoID", tratamientoID);
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", precio);

                cmd.ExecuteNonQuery();
                return true;
            }
            finally
            {
                con.Close();
            }
        }

        // MOSTRAR
        public DataTable Mostrar()
        {
            SqlConnection con = Conexion.ObtenerConexion();
            DataTable tabla = new DataTable();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("MostrarDetalleFactura", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);
            }
            finally
            {
                con.Close();
            }

            return tabla;
        }
    }
}