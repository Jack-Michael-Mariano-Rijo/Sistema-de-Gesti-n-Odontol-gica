using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class FacturasBLL
    {
        FacturasDAL dal = new FacturasDAL();

        public bool Insertar(int pacienteID)
        {
            return dal.InsertarFactura(pacienteID, "Pendiente");
        }

        public DataTable Mostrar()
        {
            return dal.MostrarFacturas();
        }
    }
}