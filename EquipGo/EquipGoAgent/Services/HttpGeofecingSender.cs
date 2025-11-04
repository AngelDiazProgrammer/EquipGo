using EquipGoAgent.Dtos;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EquipGo.Agent
{
    public class HttpGeofencingSender
    {
        private readonly FileLogger _logger;
        private readonly string _url = "https://test.outsourcing.com.co:8025/api/geofencing/process-location";

        public HttpGeofencingSender(FileLogger logger)
        {
            _logger = logger;
        }

        public GeofencingResponseDto Send(LocationRequestDto dto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Empresa-Token", "OUTS0URS1N62026-EQUIPGO");

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(_url, content).Result;

                    string resp = response.Content.ReadAsStringAsync().Result;

                    _logger.Log($"✅ GEO enviado: {json}");
                    _logger.Log($"🌍 Respuesta GEO: {response.StatusCode} - {resp}");

                    var geoResponse = JsonConvert.DeserializeObject<GeofencingResponseDto>(resp);
                    return geoResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.Log("❌ Error en HttpGeofencingSender: " + ex.Message);
                return null;
            }
        }
    }
}

