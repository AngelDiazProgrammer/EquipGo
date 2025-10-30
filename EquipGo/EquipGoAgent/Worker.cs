using EquipGoAgent.Dtos;
using System;
using System.Management;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EquipGo.Agent
{
    public class Worker
    {
        private readonly FileLogger _logger;
        private readonly GeoHelper _geoHelper;
        private readonly HttpAgentSender _sender;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public Worker()
        {
            _logger = new FileLogger();
            _geoHelper = new GeoHelper(_logger);
            _sender = new HttpAgentSender(_logger);

            // 🔌 Configurar HttpClient para llamar al backend
            _httpClient = new HttpClient();
            _apiBaseUrl = " https://steady-optimal-eagle.ngrok-free.app/api";
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _logger.Log("🚀 Worker iniciado - Modo Integrado con Backend");
        }

        public async Task StartAsync()
        {
            _logger.Log("🔗 Worker conectado al backend de geofencing");

            while (true)
            {
                try
                {
                    _logger.Log("🔄 Iniciando ciclo de verificación...");

                    // 🛰️ Obtener ubicación actual
                    var (lat, lon) = _geoHelper.GetLocation();

                    // 🔹 Serial del equipo (nombre del host)
                    string serial = Environment.MachineName;

                    // 🔹 Marca y modelo (usando WMI)
                    string marca = "Desconocida";
                    string modelo = "Desconocido";
                    try
                    {
                        using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Model FROM Win32_ComputerSystem"))
                        {
                            foreach (ManagementObject mo in searcher.Get())
                            {
                                marca = mo["Manufacturer"]?.ToString() ?? "Desconocida";
                                modelo = mo["Model"]?.ToString() ?? "Desconocido";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Log("⚠️ Error obteniendo Marca/Modelo: " + ex.Message);
                    }

                    // 🔹 Obtener MAC principal
                    string mac = "00-00-00-00-00-00";
                    foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (nic.OperationalStatus == OperationalStatus.Up &&
                            nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            mac = nic.GetPhysicalAddress().ToString();
                            break;
                        }
                    }

                    // 🔹 Sistema operativo y versión del agente
                    string sistemaOperativo = Environment.OSVersion.ToString();
                    string versionSoftware = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

                    // =====================================================
                    // 🧱 Construir DTO para sincronización principal
                    // =====================================================
                    var dtoSync = new EquipoSyncRequestDto
                    {
                        Serial = serial,
                        Marca = marca,
                        Modelo = modelo,
                        MacEquipo = mac,
                        SistemaOperativo = sistemaOperativo,
                        VersionSoftware = versionSoftware,
                        Latitud = lat,
                        Longitud = lon,
                        CodigoBarras = null,
                        IdUsuarioInfo = null,
                        IdEstado = null,
                        IdSubEstado = null,
                        IdSede = null,
                        IdEquipoPersonal = null,
                        IdTipoDispositivo = null,
                        IdProveedor = null
                    };

                    // 📤 Enviar DTO de sincronización
                    _logger.Log($"📡 Iniciando envío de Sync: Serial={serial}, Marca={marca}, Modelo={modelo}, MAC={mac}, SO={sistemaOperativo}, Lat={lat}, Lon={lon}");
                    await Task.Run(() => _sender.Send(dtoSync));

                    // =====================================================
                    // 🔄 NUEVO: Procesar geofencing con el BACKEND
                    // =====================================================
                    _logger.Log($"📍 Enviando ubicación al BACKEND: Serial={serial}, Lat={lat}, Lon={lon}");

                    try
                    {
                        var geoResponse = await ProcesarUbicacionEnBackend(serial, lat, lon);

                        if (geoResponse != null && geoResponse.DebeNotificar)
                        {
                            _logger.Log($"⚠️ Notificación desde backend ({geoResponse.ContadorNotificaciones}/5): {geoResponse.Mensaje}");

                            // 🎯 Mostrar alerta según el nivel del backend
                            bool esNoCerrable = geoResponse.NivelAlerta >= 4; // 4 = Bloqueado
                            NotificacionWindowsService.MostrarAlerta(
                                geoResponse.Mensaje,
                                esNoCerrable,
                                geoResponse.ContadorNotificaciones
                            );
                        }
                        else
                        {
                            _logger.Log("✅ Geofencing sin alerta activa.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"❌ Error procesando geofencing con backend: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"❌ Error general en Worker: {ex.Message}");
                }

                // ⏳ Esperar 5 minutos antes del siguiente ciclo
                _logger.Log("⏳ Esperando 24 horas para próximo ciclo...");
                await Task.Delay(TimeSpan.FromHours(24));
            }
        }

        // 🔌 MÉTODO PARA LLAMAR AL BACKEND DE GEOFENCING
        private async Task<GeofencingResponseDto> ProcesarUbicacionEnBackend(string serial, double lat, double lon)
        {
            try
            {
                _logger.Log($"🔗 Enviando ubicación al backend: {serial}");

                var request = new LocationRequestDto
                {
                    Serial = serial,
                    Latitude = lat,
                    Longitude = lon
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/geofencing/process-location", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<GeofencingResponseDto>(responseJson);
                    _logger.Log($"✅ Respuesta recibida del backend: DebeNotificar={result?.DebeNotificar}, Contador={result?.ContadorNotificaciones}");
                    return result;
                }
                else
                {
                    _logger.Log($"❌ Error del backend: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Error llamando al backend: {ex.Message}");
                return null;
            }
        }

        // 🧪 MÉTODO DE PRUEBA OPCIONAL
        private async Task ProbarConexionBackend()
        {
            try
            {
                _logger.Log("🔍 Probando conexión con backend...");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/geofencing");
                _logger.Log($"✅ Backend responde: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ No se puede conectar al backend: {ex.Message}");
            }
        }
    }
}