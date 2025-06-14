using EquipGo.Agent.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EquipGo.Agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string rutaLog = @"C:\ProgramData\EquipGo\Logs\info.txt";
        private const int IntervaloMinutos = 15;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(rutaLog)!);
            AutoStartManager.RegistrarInicioAutomatico();

            _logger.LogInformation("⏳ Servicio iniciado correctamente.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var clave = ClaveService.LeerClave();
                    if (string.IsNullOrWhiteSpace(clave))
                    {
                        await File.AppendAllTextAsync(rutaLog,
                            $"[{DateTime.Now}] ❌ Clave maestra no encontrada. Abortando sincronización.\n").ConfigureAwait(false);
                        await Task.Delay(TimeSpan.FromMinutes(IntervaloMinutos), stoppingToken).ConfigureAwait(false);
                        continue;
                    }

                    var servicio = new EquipoInfoService();
                    var datos = await servicio.ObtenerDatosAsync().ConfigureAwait(false);

                    // Conversión robusta de coordenadas
                    if (datos.Latitud != 0 && datos.Longitud != 0)
                    {
                        string inputLat = datos.Latitud.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
                        string inputLng = datos.Longitud.ToString(CultureInfo.InvariantCulture).Replace(',', '.');

                    }

                    await File.AppendAllTextAsync(rutaLog,
                        $"[{DateTime.Now}] Intentando sincronizar equipo SERIAL: {datos.Serial}, LAT: {datos.Latitud}, LNG: {datos.Longitud}...\n").ConfigureAwait(false);

                    var json = JsonSerializer.Serialize(datos);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("X-Empresa-Token", clave);

                    var response = await httpClient.PostAsync("https://localhost:7096/api/equipos/sync", content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        await File.AppendAllTextAsync(rutaLog,
                            $"[{DateTime.Now}] ✅ Sincronización exitosa.\n").ConfigureAwait(false);
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        await File.AppendAllTextAsync(rutaLog,
                            $"[{DateTime.Now}] ❌ Error al sincronizar: {error}\n").ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    await File.AppendAllTextAsync(rutaLog,
                        $"[{DateTime.Now}] ❌ Excepción: {ex.Message}\n").ConfigureAwait(false);
                }

                await Task.Delay(TimeSpan.FromMinutes(IntervaloMinutos), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
