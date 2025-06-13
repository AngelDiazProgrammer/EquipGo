using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo
{
    public class CrearEquipoDto
    {
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serial { get; set; }
        public string? CodigoBarras { get; set; }
        public string? Ubicacion { get; set; }
        public int? IdUsuarioInfo { get; set; }
        public int? IdEstado { get; set; }
        public int? IdEquipoPersonal { get; set; }
        public int? IdSede { get; set; }
        public int? IdTipoDispositivo { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string? SistemaOperativo { get; set; }
        public string? MacEquipo { get; set; }
        public string? VersionSoftware { get; set; }
    }
}
