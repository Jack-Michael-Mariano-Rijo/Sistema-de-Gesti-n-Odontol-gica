using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class TratamientosBLL
    {
        TratamientosDAL dal = new TratamientosDAL();

        public bool Insertar(string nombre, string descripcion, decimal costo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || costo <= 0)
                return false;

            return dal.InsertarTratamiento(nombre, descripcion, costo);
        }

        public DataTable Mostrar()
        {
            return dal.MostrarTratamientos();
        }
    }
}