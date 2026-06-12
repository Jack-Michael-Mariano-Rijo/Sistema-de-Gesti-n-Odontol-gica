using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class DetalleHistorialBLL
    {
        DetalleHistorialDAL dal = new DetalleHistorialDAL();

        public bool Insertar(int historialID, int tratamientoID, int cantidad)
        {
            if (cantidad <= 0)
                return false;

            return dal.Insertar(historialID, tratamientoID, cantidad);
        }

        public DataTable Mostrar()
        {
            return dal.Mostrar();
        }
    }
}