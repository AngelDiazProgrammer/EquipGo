using EquipGo.Agent.Utils;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EquipGo.Agent.Services
{
    public class GeoService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey;

        public GeoService()
        {
            _httpClient = new HttpClient();
            apiKey = ConfigService.GetGoogleApiKey();
        }

        public async Task<(double? latitud, double? longitud)> ObtenerUbicacionAsync()
        {
            try
            {
                // Solicitud a Google Geolocation API
                var requestContent = new StringContent("{}", Encoding.UTF8, "application/json"); // vacío para usar IP + Wi-Fi
                var response = await _httpClient.PostAsync(
                    $"https://www.googleapis.com/geolocation/v1/geolocate?key={apiKey}",
                    requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var datos = JsonSerializer.Deserialize<GoogleGeoResponse>(content);

                    if (datos != null)
                        return (datos.Location.Lat, datos.Location.Lng);
                }
                else
                {
                    File.AppendAllText(@"C:\ProgramData\EquipGo\Logs\info.txt",
                        $"[{DateTime.Now}] ❌ Google API error: {response.StatusCode}\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\ProgramData\EquipGo\Logs\info.txt",
                    $"[{DateTime.Now}] ❌ Excepción en Google GeoService: {ex.Message}\n");
            }

            // Fallback a IP-API si falla Google
            return await ObtenerUbicacionPorIPAsync();
        }

        private async Task<(double? latitud, double? longitud)> ObtenerUbicacionPorIPAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://ip-api.com/json");
                var content = await response.Content.ReadAsStringAsync();
                var datos = JsonSerializer.Deserialize<RespuestaGeoIP>(content);

                if (datos?.Status == "success")
                    return (datos.Lat, datos.Lon);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\ProgramData\EquipGo\Logs\info.txt",
                    $"[{DateTime.Now}] ❌ Fallback IP error: {ex.Message}\n");
            }

            return (null, null);
        }

        private class GoogleGeoResponse
        {
            [JsonPropertyName("location")]
            public GoogleLocation Location { get; set; }

            public class GoogleLocation
            {
                [JsonPropertyName("lat")]
                public double Lat { get; set; }

                [JsonPropertyName("lng")]
                public double Lng { get; set; }
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
