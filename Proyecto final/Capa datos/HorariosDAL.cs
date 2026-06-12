using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
    public class HorariosDAL
    {
        // INSERTAR HORARIO
        public bool InsertarHorario(int doctorID, string dia, TimeSpan inicio, TimeSpan fin)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("InsertarHorario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                cmd.Parameters.AddWithValue("@DiaSemana", dia);
                cmd.Parameters.AddWithValue("@HoraInicio", inicio);
                cmd.Parameters.AddWithValue("@HoraFin", fin);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar horario: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // MOSTRAR HORARIOS
        public DataTable MostrarHorarios()
        {
            SqlConnection con = Conexion.ObtenerConexion();
            DataTable tabla = new DataTable();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("MostrarHorarios", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al mostrar horarios: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return tabla;
        }

        // EDITAR HORARIO
        public bool EditarHorario(int id, int doctorID, string dia, TimeSpan inicio, TimeSpan fin)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("EditarHorario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@HorarioID", id);
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                cmd.Parameters.AddWithValue("@DiaSemana", dia);
                cmd.Parameters.AddWithValue("@HoraInicio", inicio);
                cmd.Parameters.AddWithValue("@HoraFin", fin);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar horario: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // ELIMINAR HORARIO
        public bool EliminarHorario(int id)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("EliminarHorario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@HorarioID", id);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar horario: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}