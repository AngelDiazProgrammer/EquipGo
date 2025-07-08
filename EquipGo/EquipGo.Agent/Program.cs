using EquipGo.Agent.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EquipGo.Agent
{
    internal class Program
    {
        private const int IntervaloMinutos = 15;
        private static string rutaLog = Path.Combine(AppContext.BaseDirectory, "info.txt");

        static async Task Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(rutaLog)!);

                await File.AppendAllTextAsync(rutaLog,
                    $"[{DateTime.Now}] 🟢 Iniciando aplicación en segundo plano...\n");

                try
                {
                    AutoStartManager.RegistrarInicioAutomaticoParaTodos(); // ahora para todos los usuarios
                }
                catch (Exception ex)
                {
                    await File.AppendAllTextAsync(rutaLog,
                        $"[{DateTime.Now}] ⚠️ Error registrando auto inicio: {ex.Message}\n");
                }

                while (true)
                {
                    try
                    {
                        var clave = ClaveService.LeerClave();
                        if (string.IsNullOrWhiteSpace(clave))
                        {
                            await File.AppendAllTextAsync(rutaLog,
                                $"[{DateTime.Now}] ❌ Clave maestra no encontrada. Abortando sincronización.\n");
                            await Task.Delay(TimeSpan.FromMinutes(IntervaloMinutos));
                            continue;
                        }

                        var servicio = new EquipoInfoService();
                        var datos = await servicio.ObtenerDatosAsync();

                        await File.AppendAllTextAsync(rutaLog,
                            $"[{DateTime.Now}] Intentando sincronizar equipo SERIAL: {datos.Serial}, LAT: {datos.Latitud}, LNG: {datos.Longitud}...\n");

                        var json = JsonSerializer.Serialize(datos);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        // Solo para pruebas locales - quitar esto en producción
                        var handler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };

                        using var httpClient = new HttpClient(handler);
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

                    await Task.Delay(TimeSpan.FromMinutes(IntervaloMinutos));
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
