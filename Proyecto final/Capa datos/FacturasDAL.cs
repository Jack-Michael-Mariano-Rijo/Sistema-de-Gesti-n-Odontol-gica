using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Capa_datos
{
    public class FacturasDAL
    {
        // =========================
        // FACTURAS (CRUD BÁSICO)
        // =========================

        public bool InsertarFactura(int pacienteID, string estado)
        {
            return EjecutarSPSinRetorno("InsertarFactura",
                new SqlParameter("@PacienteID", pacienteID),
                new SqlParameter("@Estado", estado));
        }

        public bool EditarFactura(int facturaID, int pacienteID, string estado)
        {
            return EjecutarSPSinRetorno("EditarFactura",
                new SqlParameter("@FacturaID", facturaID),
                new SqlParameter("@PacienteID", pacienteID),
                new SqlParameter("@Estado", estado));
        }

        public bool EliminarFactura(int facturaID)
        {
            return EjecutarSPSinRetorno("EliminarFactura",
                new SqlParameter("@FacturaID", facturaID));
        }

        // =========================
        // FACTURA COMPLETA
        // =========================

        public int CrearFacturaCompleta(int pacienteID, int tratamientoID, decimal monto, string metodoPago, string estado)
        {
            SqlConnection con = Conexion.ObtenerConexion();
            int facturaID = 0;

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("sp_CrearFacturaCompleta", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PacienteID", pacienteID);
                cmd.Parameters.AddWithValue("@TratamientoID", tratamientoID);
                cmd.Parameters.AddWithValue("@Monto", monto);
                cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
                cmd.Parameters.AddWithValue("@Estado", estado);

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    facturaID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en CrearFacturaCompleta: " + ex.Message);
                facturaID = 0;
            }
            finally
            {
                con.Close();
            }

            return facturaID;
        }

        // =========================
        // PAGOS
        // =========================

        public bool RegistrarPago(int facturaID, decimal monto, string metodoPago)
        {
            return EjecutarSPSinRetorno("RegistrarPago",
                new SqlParameter("@FacturaID", facturaID),
                new SqlParameter("@Monto", monto),
                new SqlParameter("@MetodoPago", metodoPago));
        }

        // =========================
        // CONSULTAS
        // =========================

        public DataTable MostrarFacturas()
        {
            return EjecutarSPDataTable("MostrarFacturas");
        }

        public DataTable ObtenerPacientes()
        {
            return EjecutarSPDataTable("ObtenerPacientes");
        }

        public DataTable ObtenerTratamientos()
        {
            return EjecutarSPDataTable("ObtenerTratamientos");
        }

        public DataTable ObtenerEstadisticas()
        {
            return EjecutarSPDataTable("EstadisticasFacturacion");
        }

        // =========================
        // PRECIO TRATAMIENTO
        // =========================

        public decimal ObtenerPrecioTratamiento(int tratamientoID)
        {
            SqlConnection con = Conexion.ObtenerConexion();
            decimal costo = 0;

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("ObtenerPrecioTratamiento", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TratamientoID", tratamientoID);

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    costo = Convert.ToDecimal(result);
                }
            }
            catch
            {
                costo = 0;
            }
            finally
            {
                con.Close();
            }

            return costo;
        }

        // =========================
        // BACKUPS (SIN COMPRESIÓN PARA EXPRESS)
        // =========================

        /// <summary>
        /// Realiza backup completo de la base de datos
        /// </summary>
        public bool RealizarBackupCompleto(string ruta)
        {
            SqlConnection con = ObtenerConexionMaster();

            try
            {
                con.Open();

                string sql = @"
                    BACKUP DATABASE [Clinica_Odontologica] 
                    TO DISK = @Ruta 
                    WITH FORMAT, 
                         NAME = 'Backup completo Clinica_Odontologica',
                         STATS = 10";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Ruta", ruta);
                cmd.CommandTimeout = 300;
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error backup: " + ex.Message);
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Realiza backup diferencial de la base de datos
        /// </summary>
        public bool RealizarBackupDiferencial(string ruta)
        {
            SqlConnection con = ObtenerConexionMaster();

            try
            {
                con.Open();

                string sql = @"
                    BACKUP DATABASE [Clinica_Odontologica] 
                    TO DISK = @Ruta 
                    WITH DIFFERENTIAL,
                         NAME = 'Backup diferencial Clinica_Odontologica',
                         STATS = 10";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Ruta", ruta);
                cmd.CommandTimeout = 300;
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error backup diferencial: " + ex.Message);
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Obtiene los nombres lógicos de los archivos dentro del backup
        /// </summary>
        public (string LogicalNameData, string LogicalNameLog) ObtenerNombresLogicosBackup(string rutaBackup)
        {
            SqlConnection con = ObtenerConexionMaster();

            try
            {
                con.Open();

                string sql = "RESTORE FILELISTONLY FROM DISK = @RutaBackup";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@RutaBackup", rutaBackup);
                    cmd.CommandTimeout = 60;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        string logicalNameData = null;
                        string logicalNameLog = null;

                        while (reader.Read())
                        {
                            string type = reader["Type"].ToString();
                            string logicalName = reader["LogicalName"].ToString();

                            if (type == "D")
                                logicalNameData = logicalName;
                            else if (type == "L")
                                logicalNameLog = logicalName;
                        }

                        reader.Close();

                        if (string.IsNullOrEmpty(logicalNameData) || string.IsNullOrEmpty(logicalNameLog))
                            throw new Exception("No se pudieron obtener los nombres lógicos del backup");

                        return (logicalNameData, logicalNameLog);
                    }
                }
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Obtiene la ruta de datos predeterminada de SQL Server
        /// </summary>
        public string ObtenerRutaDatosSQL()
        {
            SqlConnection con = ObtenerConexionMaster();

            try
            {
                con.Open();

                string sql = @"
                    SELECT 
                        SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)) + 1) AS DataPath
                    FROM sys.master_files 
                    WHERE database_id = DB_ID('master') AND type_desc = 'ROWS'";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    string ruta = cmd.ExecuteScalar()?.ToString();

                    if (string.IsNullOrEmpty(ruta))
                    {
                        // Ruta por defecto para SQL Express
                        ruta = @"C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\";
                    }

                    return ruta;
                }
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Restaura la base de datos desde un archivo .bak
        /// </summary>
        public void RestaurarBackup(string rutaBackup)
        {
            SqlConnection con = null;

            try
            {
                // Conectar a master
                con = ObtenerConexionMaster();
                con.Open();

                // Obtener nombres lógicos del backup
                var (logicalNameData, logicalNameLog) = ObtenerNombresLogicosBackup(rutaBackup);

                // Obtener ruta de datos de SQL Server
                string dataPath = ObtenerRutaDatosSQL();
                string rutaMDF = Path.Combine(dataPath, "Clinica_Odontologica.mdf");
                string rutaLDF = Path.Combine(dataPath, "Clinica_Odontologica_log.ldf");

                // Poner base de datos en modo SINGLE_USER si existe
                string sqlSingleUser = @"
                    IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Clinica_Odontologica')
                    BEGIN
                        ALTER DATABASE [Clinica_Odontologica] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    END";

                using (SqlCommand cmd = new SqlCommand(sqlSingleUser, con))
                {
                    cmd.CommandTimeout = 60;
                    cmd.ExecuteNonQuery();
                }

                // Restaurar backup con MOVE
                string sqlRestore = @"
                    RESTORE DATABASE [Clinica_Odontologica] 
                    FROM DISK = @RutaBackup 
                    WITH REPLACE, 
                         RECOVERY,
                         MOVE @LogicalNameData TO @RutaMDF,
                         MOVE @LogicalNameLog TO @RutaLDF,
                         STATS = 10";

                using (SqlCommand cmd = new SqlCommand(sqlRestore, con))
                {
                    cmd.Parameters.AddWithValue("@RutaBackup", rutaBackup);
                    cmd.Parameters.AddWithValue("@LogicalNameData", logicalNameData);
                    cmd.Parameters.AddWithValue("@LogicalNameLog", logicalNameLog);
                    cmd.Parameters.AddWithValue("@RutaMDF", rutaMDF);
                    cmd.Parameters.AddWithValue("@RutaLDF", rutaLDF);
                    cmd.CommandTimeout = 300;
                    cmd.ExecuteNonQuery();
                }

                // Poner base de datos en modo MULTI_USER
                string sqlMultiUser = @"
                    IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Clinica_Odontologica')
                    BEGIN
                        ALTER DATABASE [Clinica_Odontologica] SET MULTI_USER;
                    END";

                using (SqlCommand cmd = new SqlCommand(sqlMultiUser, con))
                {
                    cmd.CommandTimeout = 60;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error restaurar: " + ex.Message);
                throw;
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        /// <summary>
        /// Verifica si la base de datos existe
        /// </summary>
        public bool BaseDeDatosExiste()
        {
            SqlConnection con = ObtenerConexionMaster();

            try
            {
                con.Open();

                string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = 'Clinica_Odontologica'";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Obtiene información de un backup (fecha, nombre, etc.)
        /// </summary>
        public DataTable ObtenerInformacionBackup(string rutaBackup)
        {
            SqlConnection con = ObtenerConexionMaster();
            DataTable info = new DataTable();

            try
            {
                con.Open();

                string sql = "RESTORE HEADERONLY FROM DISK = @RutaBackup";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@RutaBackup", rutaBackup);
                    cmd.CommandTimeout = 60;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(info);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return info;
        }

        // =========================
        // HELPERS PRIVADOS
        // =========================

        private bool EjecutarSPSinRetorno(string sp, params SqlParameter[] parameters)
        {
            SqlConnection con = Conexion.ObtenerConexion();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sp, con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en EjecutarSPSinRetorno: " + ex.Message);
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        private DataTable EjecutarSPDataTable(string sp, params SqlParameter[] parameters)
        {
            SqlConnection con = Conexion.ObtenerConexion();
            DataTable tabla = new DataTable();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sp, con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en EjecutarSPDataTable: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return tabla;
        }

        // =========================
        // CONEXIÓN A MASTER
        // =========================

        private SqlConnection ObtenerConexionMaster()
        {
            // Obtener la cadena original de app.config
            string cadenaOriginal = Conexion.cadena;

            // Reemplazar la base de datos por "master"
            string cadenaMaster = System.Text.RegularExpressions.Regex.Replace(
                cadenaOriginal,
                "Database=[^;]+;",
                "Database=master;");

            return new SqlConnection(cadenaMaster);
        }
    }
}