using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo
{
    public class EquipoUsuarioDto
    {
        // Propiedades del equipo
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Serial { get; set; }
        public string CodigoBarras { get; set; }
        public int? IdEstado { get; set; }
        public int? IdSede { get; set; }
        public int? IdTipoDispositivo { get; set; }
        public int? IdProveedor { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string SistemaOperativo { get; set; }
        public string MacEquipo { get; set; }

        // Propiedades del usuario
        public int IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int IdArea { get; set; }
        public int IdCampaña { get; set; }
    }
}
