using EquipGo.Agent.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
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
            // Crear carpeta de logs si no existe
            Directory.CreateDirectory(Path.GetDirectoryName(rutaLog)!);

            // Registrar para que se inicie automáticamente con Windows
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
                            $"[{DateTime.Now}] ❌ Clave maestra no encontrada. Abortando sincronización.\n");
                        await Task.Delay(TimeSpan.FromMinutes(IntervaloMinutos), stoppingToken);
                        continue;
                    }

                    var servicio = new EquipoInfoService();
                    var datos = servicio.ObtenerDatos();

                    var geoService = new GeoService();
                    var ubicacion = await geoService.ObtenerUbicacionAsync();

                    if (ubicacion.latitud.HasValue && ubicacion.longitud.HasValue)
                    {
                        datos.Latitud = (decimal)ubicacion.latitud.Value;
                        datos.Longitud = (decimal)ubicacion.longitud.Value;
                    }

                    await File.AppendAllTextAsync(rutaLog,
                        $"[{DateTime.Now}] Intentando sincronizar equipo SERIAL: {datos.Serial}, LAT: {datos.Latitud}, LNG: {datos.Longitud}...\n");

                    var json = JsonSerializer.Serialize(datos);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("X-Empresa-Token", clave);

                    var response = await httpClient.PostAsync("https://localhost:7096/api/equipos/sync", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await File.AppendAllTextAsync(rutaLog,
                            $"[{DateTime.Now}] ✅ Sincronización exitosa.\n");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        await File.AppendAllTextAsync(rutaLog,
                            $"[{DateTime.Now}] ❌ Error al sincronizar: {error}\n");
                    }
                }
                catch (Exception ex)
                {
                    await File.AppendAllTextAsync(rutaLog,
                        $"[{DateTime.Now}] ❌ Excepción: {ex.Message}\n");
                }

                await Task.Delay(TimeSpan.FromMinutes(IntervaloMinutos), stoppingToken);
            }
        }
    }
}
