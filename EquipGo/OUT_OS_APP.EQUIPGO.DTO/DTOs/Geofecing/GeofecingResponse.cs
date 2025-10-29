using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Geofecing
{
    public class GeofencingResponse
    {
        public bool DebeNotificar { get; set; }          
        public string Mensaje { get; set; }              
        public string Serial { get; set; }               
        public DateTime? UltimaTransaccionFecha { get; set; } 
    }
}
