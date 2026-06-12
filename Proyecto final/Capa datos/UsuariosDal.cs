using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_datos

{
    public class UsuariosDAL


    {
        public DataTable Login(
            string usuario,
            string password)
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
                        "LoginUsuario",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@NombreUsuario",
                    usuario);

                cmd.Parameters.AddWithValue(
                    "@PasswordHash",
                    password);

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

        // INSERTAR
        public bool InsertarUsuario(
            string usuario,
            string password,
            int rolID)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "InsertarUsuario",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@NombreUsuario",
                    usuario);

                cmd.Parameters.AddWithValue(
                    "@PasswordHash",
                    password);

                cmd.Parameters.AddWithValue(
                    "@RolID",
                    rolID);

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
        public DataTable MostrarUsuarios()
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
                        "MostrarUsuarios",
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

        public bool EditarUsuario(
            int usuarioID,
            string usuario,
            string password,
            int rolID,
            bool estado)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EditarUsuario",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@UsuarioID",
                    usuarioID);

                cmd.Parameters.AddWithValue(
                    "@NombreUsuario",
                    usuario);

                cmd.Parameters.AddWithValue(
                    "@PasswordHash",
                    password);

                cmd.Parameters.AddWithValue(
                    "@RolID",
                    rolID);

                cmd.Parameters.AddWithValue(
                    "@Estado",
                    estado);

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
        public bool EliminarUsuario(
            int usuarioID)
        {
            SqlConnection con =
                Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "EliminarUsuario",
                        con);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@UsuarioID",
                    usuarioID);

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