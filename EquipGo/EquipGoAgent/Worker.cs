using EquipGoAgent.Dtos;
using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;

namespace EquipGo.Agent
{
    public class Worker
    {
        private readonly FileLogger _logger;
        private readonly GeoHelper _geoHelper;
        private readonly HttpAgentSender _sender;
        private readonly HttpGeofencingSender _geoSender;

        public Worker()
        {
            _logger = new FileLogger();
            _geoHelper = new GeoHelper(_logger);
            _sender = new HttpAgentSender(_logger);
            _geoSender = new HttpGeofencingSender(_logger);
        }

        public void Start()
        {
            while (true)
            {
                try
                {
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
                    _sender.Send(dtoSync);

                    // =====================================================
                    // 📍 Enviar DTO de geofencing
                    // =====================================================
                    var dtoGeo = new LocationRequestDto
                    {
                        Latitude = lat,
                        Longitude = lon,
                        Serial = serial
                    };

                    _logger.Log($"📍 Enviando ubicación a geofencing: Serial={serial}, Lat={lat}, Lon={lon}");

                    try
                    {
                        var geoResponse = _geoSender.Send(dtoGeo);

                        if (geoResponse != null && geoResponse.DebeNotificar)
                        {
                            _logger.Log($"⚠️ Notificación requerida: {geoResponse.Mensaje}");
                            NotificacionWindowsService.MostrarAlerta(geoResponse.Mensaje ??
                                "⚠️ El equipo ha salido de la sede sin registrar salida.");
                        }
                        else
                        {
                            _logger.Log("✅ Geofencing sin alerta activa.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"❌ Error procesando respuesta GEO: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"❌ Error general en Worker: {ex.Message}");
                }

                // ⏳ Esperar 5 minutos antes del siguiente ciclo
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
