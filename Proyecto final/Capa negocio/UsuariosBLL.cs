using System;
using System.Data;
using Capa_datos;

namespace Capa_negocio
{
    public class UsuariosBLL
    {
        UsuariosDAL dal = new UsuariosDAL();

        public DataTable Login(string usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(password))
                return null;

            return dal.Login(usuario, password);
        }

        public bool Insertar(string usuario, string password, int rolID)
        {
            if (usuario.Length < 3 || password.Length < 3)
                return false;

            return dal.InsertarUsuario(usuario, password, rolID);
        }

        public DataTable Mostrar()
        {
            return dal.MostrarUsuarios();
        }

        public bool Editar(int id, string usuario, string password, int rolID, bool estado)
        {
            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(password))
                return false;

            return dal.EditarUsuario(id, usuario, password, rolID, estado);
        }

        public bool Eliminar(int id)
        {
            return dal.EliminarUsuario(id);
        }
    }
}