using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios
{
    public class UsuarioInformacionDto{
        public int Id { get; set; }
        public string TipoDocumento { get; set; }
        public long NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Area { get; set; }
        public string Campana { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaCreacion { get; set; } 
        public DateTime? UltimaModificacion { get; set; }

        //necesarios para edicion
        public int? IdTipoDocumento { get; set; }
        public int? IdArea { get; set; }
        public int? IdCampaña { get; set; }
        public int? IdEstado { get; set; }

    }
}
