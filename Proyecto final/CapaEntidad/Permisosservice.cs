using System;
using System.Collections.Generic;

namespace Proyecto_final
{
    
    public class PermisosService
    {
        public enum Rol
        {
            Administrador,
            Secretaria,
            Doctor
        }

        public enum Permiso
        {
            // Dashboard
            VerCitasDelDia,
            VerPacientesRegistrados,
            VerFacturasPendientes,
            VerIngresosHoy,

            // Pacientes
            AccesoMenuPacientes,
            CrearPaciente,
            EditarPaciente,
            EliminarPaciente,

            // Citas
            AccesoMenuCitas,
            CrearCita,
            EditarCita,
            EliminarCita,

            // Facturación
            AccesoMenuFacturacion,
            VerCardsEstadisticos,
            VerFacturas,
            CrearFactura,
            EditarFactura,
            EliminarFactura,

            // Reportes
            AccesoMenuReportes,

            // Botones rápidos
            VerBotonHorarios,
            VerBotonNuevosDoctores,
            VerBotonInicio
        }

        private Rol rolActual;
        private Dictionary<Rol, HashSet<Permiso>> permisosMap;

        public PermisosService(string nombreRol)
        {
            // Parsear el rol desde string
            if (Enum.TryParse<Rol>(nombreRol, out var rol))
            {
                rolActual = rol;
            }
            else
            {
                rolActual = Rol.Doctor; // Rol por defecto (más restrictivo)
            }

            InicializarPermisos();
        }

        public PermisosService(Rol rol)
        {
            rolActual = rol;
            InicializarPermisos();
        }

        private void InicializarPermisos()
        {
            permisosMap = new Dictionary<Rol, HashSet<Permiso>>();

            // ========== ADMINISTRADOR ==========
            permisosMap[Rol.Administrador] = new HashSet<Permiso>
            {
                // Dashboard - Ver todo
                Permiso.VerCitasDelDia,
                Permiso.VerPacientesRegistrados,
                Permiso.VerFacturasPendientes,
                Permiso.VerIngresosHoy,

                // Pacientes - Acceso completo
                Permiso.AccesoMenuPacientes,
                Permiso.CrearPaciente,
                Permiso.EditarPaciente,
                Permiso.EliminarPaciente,

                // Citas - Acceso completo
                Permiso.AccesoMenuCitas,
                Permiso.CrearCita,
                Permiso.EditarCita,
                Permiso.EliminarCita,

                // Facturación - Acceso completo
                Permiso.AccesoMenuFacturacion,
                Permiso.VerCardsEstadisticos,
                Permiso.VerFacturas,
                Permiso.CrearFactura,
                Permiso.EditarFactura,
                Permiso.EliminarFactura,

                // Reportes
                Permiso.AccesoMenuReportes,

                // Botones rápidos
                Permiso.VerBotonHorarios,
                Permiso.VerBotonNuevosDoctores,
                Permiso.VerBotonInicio
            };

            // ========== SECRETARIA ==========
            permisosMap[Rol.Secretaria] = new HashSet<Permiso>
            {
                // Dashboard - Ver solo citas y pacientes
                Permiso.VerCitasDelDia,
                Permiso.VerPacientesRegistrados,

                // Pacientes - Acceso completo
                Permiso.AccesoMenuPacientes,
                Permiso.CrearPaciente,
                Permiso.EditarPaciente,
                Permiso.EliminarPaciente,

                // Citas - Acceso completo
                Permiso.AccesoMenuCitas,
                Permiso.CrearCita,
                Permiso.EditarCita,
                Permiso.EliminarCita,

                // Facturación - Solo lectura
                Permiso.AccesoMenuFacturacion,
                Permiso.VerFacturas,
                Permiso.CrearFactura,

                // Botones rápidos
                Permiso.VerBotonHorarios,
                Permiso.VerBotonInicio
            };

            // ========== DOCTOR ==========
            permisosMap[Rol.Doctor] = new HashSet<Permiso>
            {
                // Dashboard - Ver solo citas y pacientes
                Permiso.VerCitasDelDia,
                Permiso.VerPacientesRegistrados,

                // Pacientes - Acceso completo
                Permiso.AccesoMenuPacientes,
                Permiso.CrearPaciente,
                Permiso.EditarPaciente,
                Permiso.EliminarPaciente,

                // Citas - Acceso completo
                Permiso.AccesoMenuCitas,
                Permiso.CrearCita,
                Permiso.EditarCita,
                Permiso.EliminarCita,

                // Botones rápidos
                Permiso.VerBotonHorarios,
                Permiso.VerBotonInicio
            };
        }

        /// <summary>
        /// Verifica si el usuario actual tiene un permiso específico
        /// </summary>
        public bool Tiene(Permiso permiso)
        {
            if (permisosMap.TryGetValue(rolActual, out var permisos))
            {
                return permisos.Contains(permiso);
            }
            return false;
        }

        /// <summary>
        /// Verifica si tiene TODOS los permisos de una lista
        /// </summary>
        public bool TieneTodos(params Permiso[] permisos)
        {
            foreach (var permiso in permisos)
            {
                if (!Tiene(permiso))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Verifica si tiene AL MENOS UNO de los permisos
        /// </summary>
        public bool TieneAlguno(params Permiso[] permisos)
        {
            foreach (var permiso in permisos)
            {
                if (Tiene(permiso))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Obtiene el rol actual
        /// </summary>
        public Rol ObtenerRol()
        {
            return rolActual;
        }

        /// <summary>
        /// Obtiene el nombre del rol
        /// </summary>
        public string ObtenerNombreRol()
        {
            return rolActual.ToString();
        }
    }
}