using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class PacientesService
    {
        private PacientesDAL dal = new PacientesDAL();

        public bool Insertar(string n, string a, string c, string s, string t, string d)
            => dal.InsertarPaciente(n, a, c, s, t, d);

        public bool Editar(int id, string n, string a, string c, string s, string t, string d)
            => dal.EditarPaciente(id, n, a, c, s, t, d);

        public bool Eliminar(int id)
            => dal.EliminarPaciente(id);

        public DataTable Listar()
            => dal.MostrarPacientes();
    }
}