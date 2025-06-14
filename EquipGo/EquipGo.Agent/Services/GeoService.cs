using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EquipGo.Agent.Services
{
    public class GeoService
    {
        private readonly HttpClient _httpClient;

        public GeoService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(double? latitud, double? longitud)> ObtenerUbicacionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://ip-api.com/json");
                if (!response.IsSuccessStatusCode)
                {
                    File.AppendAllText(@"C:\ProgramData\EquipGo\Logs\info.txt",
                        $"[{DateTime.Now}] ❌ Error HTTP al obtener geolocalización: {response.StatusCode}\n");
                    return (null, null);
                }

                var content = await response.Content.ReadAsStringAsync();
                var datos = JsonSerializer.Deserialize<RespuestaGeoIP>(content);

                if (datos?.Status == "success")
                    return (datos.Lat, datos.Lon);

                File.AppendAllText(@"C:\ProgramData\EquipGo\Logs\info.txt",
                    $"[{DateTime.Now}] ❌ GeoService falló. Respuesta: {content}\n");

                return (null, null);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\ProgramData\EquipGo\Logs\info.txt",
                    $"[{DateTime.Now}] ❌ Excepción en GeoService: {ex.Message}\n");
                return (null, null);
            }
        }


        private class RespuestaGeoIP
        {
            [JsonPropertyName("status")]
            public string? Status { get; set; }

            [JsonPropertyName("lat")]
            public double Lat { get; set; }

            [JsonPropertyName("lon")]
            public double Lon { get; set; }
        }
    }
}
