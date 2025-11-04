// Controllers/ProxyController.cs
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;

[ApiController]
[Route("api")]
public class ProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProxyController> _logger;
    private readonly string _servidorBaseUrl = "https://localhost:7096";

    public ProxyController(HttpClient httpClient, ILogger<ProxyController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    [HttpPost("geofencing/process-location")]
    public async Task<IActionResult> ProxyGeofencing()
    {
        return await ProxyToServidor("Geofencing/process-location");
    }

    [HttpPost("equipos/sync")]
    public async Task<IActionResult> ProxyEquiposSync()
    {
        return await ProxyToServidor("Equipos/sync");
    }

    private async Task<IActionResult> ProxyToServidor(string endpoint)
    {
        try
        {
            _logger.LogInformation($"📨 Recibida petición en proxy: {endpoint}");

            // ✅ LEER DIRECTAMENTE EL JSON DEL BODY
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            _logger.LogInformation($"📦 Raw Request Body: {requestBody}");

            var url = $"{_servidorBaseUrl}/api/{endpoint}";
            _logger.LogInformation($"🔗 URL: {url}");

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            request.Headers.Add("X-Empresa-Token", "OUTS0URS1N62026-EQUIPGO");

            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"📥 Response: {response.StatusCode} - {responseContent}");

            return Content(responseContent, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error en proxy: {endpoint}");
            return StatusCode(500, $"Error de proxy: {ex.Message}");
        }
    }
}