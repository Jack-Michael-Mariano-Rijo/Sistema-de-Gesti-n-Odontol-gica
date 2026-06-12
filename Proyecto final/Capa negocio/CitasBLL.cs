using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class CitasService
    {
        CitasDAL dal = new CitasDAL();

        public bool InsertarCita(int pacienteID, int doctorID, DateTime fechaCita, TimeSpan horaCita, string estado, string observaciones)
        {
            try
            {
                return dal.InsertarCita(pacienteID, doctorID, fechaCita, horaCita, estado, observaciones);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar cita: " + ex.Message);
            }
        }

        public DataTable MostrarCitas()
        {
            try
            {
                return dal.MostrarCitas();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener citas: " + ex.Message);
            }
        }

        public bool EditarCita(int citaID, int pacienteID, int doctorID, DateTime fechaCita, TimeSpan horaCita, string estado, string observaciones)
        {
            try
            {
                return dal.EditarCita(citaID, pacienteID, doctorID, fechaCita, horaCita, estado, observaciones);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar cita: " + ex.Message);
            }
        }

        public bool EliminarCita(int citaID)
        {
            try
            {
                return dal.EliminarCita(citaID);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar cita: " + ex.Message);
            }
        }
    }
}