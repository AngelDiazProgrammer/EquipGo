using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using EquipGo.Agent.Models;

namespace EquipGo.Agent.Services
{
    public class EquipoInfoService
    {
        private readonly GeoService _geoService = new GeoService();
        public async Task<EquipoSyncRequestDto> ObtenerDatosAsync()
        {
            var (latitud, longitud) = await _geoService.ObtenerUbicacionAsync();

            return new EquipoSyncRequestDto
            {
                Marca = ObtenerMarca(),
                Modelo = ObtenerModelo(),
                Serial = ObtenerSerial(),
                MacEquipo = ObtenerMac(),
                SistemaOperativo = ObtenerSistemaOperativo(),
                VersionSoftware = ObtenerVersionSoftware(),
                Latitud = latitud.Value,
                Longitud =longitud.Value 
            };
        }

        private string ObtenerMarca()
        {
            return new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault()?["Manufacturer"]?.ToString() ?? "Desconocido";
        }

        private string ObtenerModelo()
        {
            return new ManagementObjectSearcher("SELECT Model FROM Win32_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault()?["Model"]?.ToString() ?? "Desconocido";
        }

        private string ObtenerSerial()
        {
            return new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault()?["SerialNumber"]?.ToString() ?? "Desconocido";
        }

        private string ObtenerMac()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                     n.NetworkInterfaceType != NetworkInterfaceType.Loopback)?
                .GetPhysicalAddress().ToString() ?? "Desconocida";
        }

        private string ObtenerSistemaOperativo()
        {
            return Environment.OSVersion.VersionString;
        }

        private string ObtenerVersionSoftware()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0";
        }
    }
}
