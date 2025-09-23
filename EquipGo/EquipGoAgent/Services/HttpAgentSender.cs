using EquipGoAgent.Dtos;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace EquipGo.Agent
{
    public class HttpAgentSender
    {
        private readonly FileLogger _logger;
        private readonly string _url = "http://steady-optimal-eagle.ngrok-free.app/api/equipos/sync"; // cambia por tu API real

        public HttpAgentSender(FileLogger logger)
        {
            _logger = logger;
        }

        public void Send(EquipoSyncRequestDto dto)
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

                    _logger.Log($"✅ DTO enviado: {json}");
                    _logger.Log($"🌍 Respuesta: {response.StatusCode} - {resp}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log("❌ Error en HttpAgentSender: " + ex.Message);
            }
        }
    }
}
