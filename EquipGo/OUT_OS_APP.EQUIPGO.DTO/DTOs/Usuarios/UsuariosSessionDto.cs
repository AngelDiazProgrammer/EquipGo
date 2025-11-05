using OUT_OS_APP.EQUIPGO.DTO.DTOs.Estado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios
{
    public class UsuarioSessionCompletoDto
    {
        public int Id { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contraseña { get; set; }
        public int IdEstado { get; set; }
        public int IdRol { get; set; }
        public int IdSede { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimaModificacion { get; set; }

        // Información de las entidades relacionadas
        public TipoDocumentoDto TipoDocumento { get; set; }
        public EstadoDto Estado { get; set; }
        public RolDto Rol { get; set; }
        public SedeDto Sede { get; set; }
    }

    public class UsuarioSessionCrearDto
    {
        public int IdTipodocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contraseña { get; set; }
        public int IdEstado { get; set; } = 1; // Por defecto activo
        public int IdRol { get; set; }
        public int IdSede { get; set; }
    }

    public class UsuarioSessionActualizarDto
    {
        public int Id { get; set; }
        public int IdTipodocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int IdEstado { get; set; }
        public int IdRol { get; set; }
        public int IdSede { get; set; }
    }


    public class UsuarioSessionDto
    {
        public int Id { get; set; }
        public int IdTipodocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; }
        public int IdSede { get; set; }
        public string Sede { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimaModificacion { get; set; }
    }

    public class LoginDto
    {
        public string NumeroDocumento { get; set; }
        public string Contraseña { get; set; }
    }

    public class CambioContraseñaDto
    {
        public int UsuarioId { get; set; }
        public string ContraseñaActual { get; set; }
        public string NuevaContraseña { get; set; }
        public string ConfirmarContraseña { get; set; }
    }

    // DTOs auxiliares para las entidades relacionadas
    public class TipoDocumentoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class EstadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class RolDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class SedeDto
    {
        public int Id { get; set; }
        public string NombreSede { get; set; }
    }
}