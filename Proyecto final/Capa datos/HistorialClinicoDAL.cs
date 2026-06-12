using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
    public class HistorialClinicoDAL
    {
        public bool InsertarHistorial(
            int pacienteID,
            int doctorID,
            string diagnostico,
            string observaciones)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "InsertarHistorialClinico",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@PacienteID",
                    pacienteID);

                cmd.Parameters.AddWithValue(
                    "@DoctorID",
                    doctorID);

                cmd.Parameters.AddWithValue(
                    "@Diagnostico",
                    diagnostico);

                cmd.Parameters.AddWithValue(
                    "@Observaciones",
                    observaciones);

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

        // MOSTRAR
        public DataTable MostrarHistorial()
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
                        "MostrarHistorialClinico",
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
        public bool EditarHistorial(
            int historialID,
            int pacienteID,
            int doctorID,
            string diagnostico,
            string observaciones)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EditarHistorialClinico",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@HistorialID",
                    historialID);

                cmd.Parameters.AddWithValue(
                    "@PacienteID",
                    pacienteID);

                cmd.Parameters.AddWithValue(
                    "@DoctorID",
                    doctorID);

                cmd.Parameters.AddWithValue(
                    "@Diagnostico",
                    diagnostico);

                cmd.Parameters.AddWithValue(
                    "@Observaciones",
                    observaciones);

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

        public bool EliminarHistorial(
            int historialID)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EliminarHistorialClinico",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@HistorialID",
                    historialID);

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