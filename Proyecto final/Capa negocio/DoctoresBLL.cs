using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class DoctoresBLL
    {
        private DoctoresDAL dal = new DoctoresDAL();

        public bool Insertar(string nombres, string especialidad, string telefono)
        {
            if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(especialidad))
                return false;

            return dal.InsertarDoctor(nombres, especialidad, telefono);
        }

        public DataTable Mostrar()
        {
            return dal.MostrarDoctores();
        }

        public bool Editar(int doctorID, string nombres, string especialidad, string telefono)
        {
            if (doctorID <= 0)
                return false;

            return dal.EditarDoctor(doctorID, nombres, especialidad, telefono);
        }

        public bool Eliminar(int doctorID)
        {
            if (doctorID <= 0)
                return false;

            return dal.EliminarDoctor(doctorID);
        }
    }
}