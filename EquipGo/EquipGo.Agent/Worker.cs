/* using EquipGo.Agent.Services;
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
        private readonly string rutaLog = Path.Combine(AppContext.BaseDirectory, "info.txt");
        private const int IntervaloMinutos = 15;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

       protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(rutaLog)!);

                await File.AppendAllTextAsync(rutaLog,
                    $"[{DateTime.Now}] 🟢 Iniciando Worker...\n");

                try
                {
                    AutoStartManager.RegistrarInicioAutomatico();
                }
                catch (Exception ex)
                {
                    await File.AppendAllTextAsync(rutaLog,
                        $"[{DateTime.Now}] ⚠️ Error en AutoStartManager: {ex.Message}\n");
                }

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
                        var datos = await servicio.ObtenerDatosAsync();

                        await File.AppendAllTextAsync(rutaLog,
                            $"[{DateTime.Now}] Intentando sincronizar equipo SERIAL: {datos.Serial}, LAT: {datos.Latitud}, LNG: {datos.Longitud}...\n");

                        var json = JsonSerializer.Serialize(datos);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        //IMPORTANTEEEEEEEEEEEEEEEEE
                        #region Prueba debbugin, corregir en produccion 
                        var handler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };

                        using var httpClient = new HttpClient(handler);
                        httpClient.DefaultRequestHeaders.Add("X-Empresa-Token", clave);

                        var response = await httpClient.PostAsync("https://localhost:7096/api/equipos/sync", content);
                        #endregion
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
            catch (Exception fatal)
            {
                var fallbackPath = Path.Combine(AppContext.BaseDirectory, "fatal.txt");
                File.AppendAllText(fallbackPath, $"[FATAL] {DateTime.Now}: {fatal.Message}\n");
            }
        }
    }
}
*/