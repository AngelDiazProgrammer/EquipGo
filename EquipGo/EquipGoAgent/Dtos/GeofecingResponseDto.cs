using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EquipGoAgent.Dtos
{
    public class GeofencingResponseDto
    {
        [JsonPropertyName("debeNotificar")]
        public bool DebeNotificar { get; set; }

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }

        [JsonPropertyName("serial")]
        public string Serial { get; set; }

        [JsonPropertyName("ultimaTransaccionFecha")]
        public DateTime? UltimaTransaccionFecha { get; set; }

        [JsonPropertyName("contadorNotificaciones")]
        public int ContadorNotificaciones { get; set; }

        [JsonPropertyName("nivelAlerta")]
        public int NivelAlerta { get; set; }
    }
}
