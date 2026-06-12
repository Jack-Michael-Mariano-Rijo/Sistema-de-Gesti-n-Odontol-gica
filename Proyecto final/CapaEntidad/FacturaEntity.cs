using System;

namespace Capa_entidad
{
    public class FacturaEntity
    {
        public int FacturaID { get; set; }
        public int PacienteID { get; set; }
        public DateTime FechaFactura { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}