using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
    public class CitasDAL
    {
        public bool InsertarCita(
            int pacienteID,
            int doctorID,
            DateTime fechaCita,
            TimeSpan horaCita,
            string estado,
            string observaciones)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "InsertarCita",
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
                    "@FechaCita",
                    fechaCita);

                cmd.Parameters.AddWithValue(
                    "@HoraCita",
                    horaCita);

                cmd.Parameters.AddWithValue(
                    "@Estado",
                    estado);

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

        public DataTable MostrarCitas()
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
                        "MostrarCitas",
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

        public bool EditarCita(
            int citaID,
            int pacienteID,
            int doctorID,
            DateTime fechaCita,
            TimeSpan horaCita,
            string estado,
            string observaciones)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EditarCita",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@CitaID",
                    citaID);

                cmd.Parameters.AddWithValue(
                    "@PacienteID",
                    pacienteID);

                cmd.Parameters.AddWithValue(
                    "@DoctorID",
                    doctorID);

                cmd.Parameters.AddWithValue(
                    "@FechaCita",
                    fechaCita);

                cmd.Parameters.AddWithValue(
                    "@HoraCita",
                    horaCita);

                cmd.Parameters.AddWithValue(
                    "@Estado",
                    estado);

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

        // ELIMINAR
        public bool EliminarCita(
            int citaID)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EliminarCita",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@CitaID",
                    citaID);

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