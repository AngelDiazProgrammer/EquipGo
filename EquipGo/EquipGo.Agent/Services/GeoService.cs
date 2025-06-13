using System.Net.Http;
using System.Text.Json;
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
                    return (null, null);

                var content = await response.Content.ReadAsStringAsync();
                var datos = JsonSerializer.Deserialize<RespuestaGeoIP>(content);

                if (datos?.Status == "success")
                    return (datos.Lat, datos.Lon);

                return (null, null);
            }
            catch
            {
                return (null, null);
            }
        }

        private class RespuestaGeoIP
        {
            public string? Status { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }
}
