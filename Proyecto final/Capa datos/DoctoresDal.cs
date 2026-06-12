using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos
{
    public class DoctoresDAL
    {
        public DataTable ObtenerDoctores()
        {
            SqlConnection con = Conexion.ObtenerConexion();
            DataTable tabla = new DataTable();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("ObtenerDoctores", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener doctores: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return tabla;
        }

        public DataTable MostrarDoctores()
        {
            SqlConnection con = Conexion.ObtenerConexion();
            DataTable tabla = new DataTable();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("MostrarDoctores", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);
            }
            catch
            {
                // Mantener como estaba
            }
            finally
            {
                con.Close();
            }
            return tabla;
        }

        // ÚNICA VERSIÓN DE InsertarDoctor
        public bool InsertarDoctor(string nombres, string especialidad, string telefono)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_InsertarDoctor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombres", nombres);
                cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                cmd.Parameters.AddWithValue("@Telefono", telefono);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar doctor: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // ÚNICA VERSIÓN DE EditarDoctor
        public bool EditarDoctor(int doctorID, string nombres, string especialidad, string telefono)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_EditarDoctor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                cmd.Parameters.AddWithValue("@Nombres", nombres);
                cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                cmd.Parameters.AddWithValue("@Telefono", telefono);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar doctor: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool EliminarDoctor(int doctorID)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("EliminarDoctor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
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

        public DataTable BuscarDoctores(string busqueda)
        {
            SqlConnection con = Conexion.ObtenerConexion();
            DataTable tabla = new DataTable();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("BuscarDoctores", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Busqueda", busqueda);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar doctores: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return tabla;
        }
    }
}