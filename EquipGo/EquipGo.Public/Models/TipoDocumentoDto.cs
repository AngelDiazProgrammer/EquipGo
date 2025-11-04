using System.Text.Json.Serialization;

namespace EquipGo.Public.Models
{
    public class TipoDocumentoDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombreDocumento")]
        public string NombreDocumento { get; set; } = string.Empty;
    }
}
