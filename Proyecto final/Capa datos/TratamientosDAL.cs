using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
    public class TratamientosDAL
    {
        public bool InsertarTratamiento(
            string nombre,
            string descripcion,
            decimal costo)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "InsertarTratamiento",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@NombreTratamiento",
                    nombre);

                cmd.Parameters.AddWithValue(
                    "@Descripcion",
                    descripcion);

                cmd.Parameters.AddWithValue(
                    "@Costo",
                    costo);

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

        public DataTable MostrarTratamientos()
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
                        "MostrarTratamientos",
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

        public bool EditarTratamiento(
            int tratamientoID,
            string nombre,
            string descripcion,
            decimal costo)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EditarTratamiento",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@TratamientoID",
                    tratamientoID);

                cmd.Parameters.AddWithValue(
                    "@NombreTratamiento",
                    nombre);

                cmd.Parameters.AddWithValue(
                    "@Descripcion",
                    descripcion);

                cmd.Parameters.AddWithValue(
                    "@Costo",
                    costo);

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

        public bool EliminarTratamiento(
            int tratamientoID)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EliminarTratamiento",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@TratamientoID",
                    tratamientoID);

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