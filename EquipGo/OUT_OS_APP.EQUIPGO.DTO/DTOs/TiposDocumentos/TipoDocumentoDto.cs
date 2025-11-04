using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.TiposDocumentos
{
    public class TipoDocumentoDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombreDocumento")]
        public string NombreDocumento { get; set; } = string.Empty;
    }
}
