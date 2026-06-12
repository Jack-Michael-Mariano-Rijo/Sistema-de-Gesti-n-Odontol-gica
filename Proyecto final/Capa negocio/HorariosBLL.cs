using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class HorariosBLL
    {
        HorariosDAL dal = new HorariosDAL();

        public bool Insertar(int doctorID, string dia, TimeSpan inicio, TimeSpan fin)
        {
            return dal.InsertarHorario(doctorID, dia, inicio, fin);
        }

        public DataTable Mostrar()
        {
            return dal.MostrarHorarios();
        }
    }
}