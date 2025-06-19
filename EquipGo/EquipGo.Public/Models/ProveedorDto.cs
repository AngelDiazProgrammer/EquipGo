using System.Text.Json.Serialization;

namespace EquipGo.Public.Models
{
    public class ProveedorDto
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("NombreProveedor")] // <- Coincide con el JSON real
        public string Nombre { get; set; } = "";
    }
}
