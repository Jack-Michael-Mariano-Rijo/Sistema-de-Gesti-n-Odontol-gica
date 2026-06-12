using System;

namespace Capa_entidad
{
    public class CitaEntity
    {
        public int CitaID { get; set; }
        public int PacienteID { get; set; }
        public int DoctorID { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
    }
}