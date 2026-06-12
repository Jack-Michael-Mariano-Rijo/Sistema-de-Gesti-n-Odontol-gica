using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
    public class PagosDAL
    {
        public bool InsertarPago(
            int facturaID,
            decimal monto,
            string metodoPago)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "InsertarPago",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@FacturaID",
                    facturaID);

                cmd.Parameters.AddWithValue(
                    "@Monto",
                    monto);

                cmd.Parameters.AddWithValue(
                    "@MetodoPago",
                    metodoPago);

                cmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable MostrarPagos()
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            DataTable tabla =
                new DataTable();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "MostrarPagos",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                SqlDataAdapter da =
                    new SqlDataAdapter(cmd);

                da.Fill(tabla);
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return tabla;
        }

        // EDITAR
        public bool EditarPago(
            int pagoID,
            int facturaID,
            decimal monto,
            string metodoPago)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EditarPago",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@PagoID",
                    pagoID);

                cmd.Parameters.AddWithValue(
                    "@FacturaID",
                    facturaID);

                cmd.Parameters.AddWithValue(
                    "@Monto",
                    monto);

                cmd.Parameters.AddWithValue(
                    "@MetodoPago",
                    metodoPago);

                cmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public bool EliminarPago(
            int pagoID)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EliminarPago",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@PagoID",
                    pagoID);

                cmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }
    }
}