using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo
{
    public class EquipoDto
    {
        public int Id { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serial { get; set; }
        public string? CodigoBarras { get; set; }
        public string? Ubicacion { get; set; }
        public string? UsuarioNombreCompleto { get; set; }
        public string? EstadoNombre { get; set; }
        public string EquipoPersonalNombre { get; set; }
        public string? SedeNombre { get; set; }
        public string TipoDispositivoNombre { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string? SistemaOperativo { get; set; }
        public string? MacEquipo { get; set; }
        public string? VersionSoftware { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? UltimaModificacion { get; set; }

        // Campos necesarios para editar
        public int? IdUsuarioInfo { get; set; }
        public int? IdEstado { get; set; }
        public int? IdEquipoPersonal { get; set; }
        public int? IdSede { get; set; }
        public int? IdTipoDispositivo { get; set; }

    }
}
