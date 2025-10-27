using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios
{
    public class UsuarioCrearDto
    {
        public int? IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int? IdArea { get; set; }
        public string NombreCampaña { get; set; }
        public int? IdCampaña { get; set; }
        public int? IdEstado { get; set; }
    }
}
