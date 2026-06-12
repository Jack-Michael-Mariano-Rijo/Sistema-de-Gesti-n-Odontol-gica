using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class DetalleFacturaBLL
    {
        DetalleFacturaDAL dal = new DetalleFacturaDAL();

        public bool Insertar(int facturaID, int tratamientoID, int cantidad, decimal precio)
        {
            if (cantidad <= 0 || precio <= 0)
                return false;

            return dal.Insertar(facturaID, tratamientoID, cantidad, precio);
        }

        public DataTable Mostrar()
        {
            return dal.Mostrar();
        }
    }
}