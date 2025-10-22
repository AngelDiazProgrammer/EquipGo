using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo
{
    // DTO para la request de asignación
    public class AsignarUsuarioRequestDto
    {
        public int EquipoId { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int IdArea { get; set; }
        public int IdCampaña { get; set; }
    }

    // DTO para la response de asignación
    public class AsignarUsuarioResponseDto
    {
        public string Message { get; set; }
        public int UsuarioId { get; set; }
        public bool Success { get; set; }
    }
}
