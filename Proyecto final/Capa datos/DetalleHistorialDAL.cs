using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
	public class DetalleHistorialDAL
	{
		public bool Insertar(int historialID, int tratamientoID, int cantidad)
		{
			SqlConnection con = Conexion.ObtenerConexion();

			try
			{
				con.Open();

				SqlCommand cmd = new SqlCommand("InsertarDetalleHistorial", con);
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@HistorialID", historialID);
				cmd.Parameters.AddWithValue("@TratamientoID", tratamientoID);
				cmd.Parameters.AddWithValue("@Cantidad", cantidad);

				cmd.ExecuteNonQuery();
				return true;
			}
			finally
			{
				con.Close();
			}
		}

		public DataTable Mostrar()
		{
			SqlConnection con = Conexion.ObtenerConexion();
			DataTable tabla = new DataTable();

			try
			{
				con.Open();

				SqlCommand cmd = new SqlCommand("MostrarDetalleHistorial", con);
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