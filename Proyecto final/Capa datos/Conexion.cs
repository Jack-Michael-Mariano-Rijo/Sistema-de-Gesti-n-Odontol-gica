using System;
using System.Data.SqlClient;
using System.Configuration;

namespace Capa_datos
{
    public class Conexion
    {
        public static string cadena =
            ConfigurationManager.ConnectionStrings["cn"].ConnectionString;

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}