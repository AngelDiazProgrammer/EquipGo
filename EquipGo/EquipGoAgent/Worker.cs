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

        public Worker()
        {
            _logger = new FileLogger();
            _geoHelper = new GeoHelper(_logger);
            _sender = new HttpAgentSender(_logger);
        }

        public void Start()
        {
            while (true)
            {
                try
                {
                    //Obtener ubicación
                    var (lat, lon) = _geoHelper.GetLocation();

                    //Serial del equipo
                    string serial = Environment.MachineName;

                    //Marca y Modelo usando WMI
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

                    //MAC principal
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

                    //Sistema operativo
                    string sistemaOperativo = Environment.OSVersion.ToString();

                    //Versión del agente
                    string versionSoftware = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

                    //Construir DTO
                    var dto = new EquipoSyncRequestDto
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

                    // 📤 Enviar DTO
                    _logger.Log($"📡 Iniciando envío: Serial={serial}, Marca={marca}, Modelo={modelo}, MAC={mac}, SO={sistemaOperativo}, Lat={lat}, Lon={lon}");
                    _sender.Send(dto);
                }
                catch (Exception ex)
                {
                    _logger.Log("❌ Error en Worker: " + ex);
                }

                // ⏳ Esperar 5 minutos
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
