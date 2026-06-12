using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class PagosBLL
    {
        PagosDAL dal = new PagosDAL();

        public bool Insertar(int facturaID, decimal monto, string metodoPago)
        {
            if (monto <= 0)
                return false;

            return dal.InsertarPago(facturaID, monto, metodoPago);
        }

        public DataTable Mostrar()
        {
            return dal.MostrarPagos();
        }
    }
}