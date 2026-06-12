using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class HistorialClinicoBLL
    {
        HistorialClinicoDAL dal = new HistorialClinicoDAL();

        public bool Insertar(int pacienteID, int doctorID, string diagnostico, string observaciones)
        {
            if (string.IsNullOrWhiteSpace(diagnostico))
                return false;

            return dal.InsertarHistorial(pacienteID, doctorID, diagnostico, observaciones);
        }

        public DataTable Mostrar()
        {
            return dal.MostrarHistorial();
        }
    }
}