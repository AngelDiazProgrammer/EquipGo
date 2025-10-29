using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipGoAgent.Dtos
{
    public class GeofencingResponseDto
    {
        public bool DebeNotificar { get; set; }
        public string Mensaje { get; set; }
        public string Serial { get; set; }
        public DateTime? UltimaTransaccionFecha { get; set; }
    }
}
