using System;

namespace Capa_entidad
{
    public class PagoEntity
    {
        public int PagoID { get; set; }
        public int FacturaID { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
    }
}