using EquipGo.ClientApp.DTOs;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;

namespace EquipGo.ClientApp.Services
{
    public class EquipoInfoService
    {
        public EquipoSyncRequestDto ObtenerDatosEquipo()
        {
            try
            {
                var dto = new EquipoSyncRequestDto
                {
                    Serial = ObtenerSerial(),
                    Marca = ObtenerMarca(),
                    Modelo = ObtenerModelo(),
                    MacEquipo = ObtenerMac(),
                    SistemaOperativo = ObtenerSistemaOperativo(),
                    VersionSoftware = ObtenerVersionSoftware(),
                    Latitud = 5.000000m,  // temporal
                    Longitud = -74.000000m // temporal
                };

                return dto;
            }
            catch (Exception ex)
            {
                LogHelper.Log($"❌ Error al obtener datos del equipo: {ex.Message}");
                return null;
            }
        }

        private string ObtenerSerial()
        {
            return new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault()?["SerialNumber"]?.ToString() ?? "Desconocido";
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

        private string ObtenerMac()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                     n.NetworkInterfaceType != NetworkInterfaceType.Loopback)?
                .GetPhysicalAddress()
                .ToString() ?? "Desconocida";
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
