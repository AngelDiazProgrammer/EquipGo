using Interface;
using Interface.Services.Geofecing;
using Microsoft.EntityFrameworkCore;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Geofecing;
using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Geofecing
{
    public class GeofencingService : IGeofencingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EquipGoDbContext _context;

        public GeofencingService(IUnitOfWork unitOfWork, EquipGoDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<GeofencingResponse> ProcessLocation(LocationRequest request)
        {
            try
            {
                // 1. Verificar si está dentro de alguna sede
                var estaEnSede = await EstaEnSede(request.Latitude, request.Longitude);

                // 2. Si está FUERA de todas las sedes
                if (!estaEnSede)
                {
                    // 3. Buscar última transacción por SERIAL
                    var ultimaTransaccion = await GetUltimaTransaccionPorSerial(request.Serial);

                    // 4. Si la última transacción fue una ENTRADA (1)
                    if (ultimaTransaccion != null && ultimaTransaccion.IdTipoTransaccion == 1)
                    {
                        return new GeofencingResponse
                        {
                            DebeNotificar = true,
                            Mensaje = "⚠️ ¡Atención! No olvides registrar la SALIDA de tu equipo la próxima vez que visites la SEDE.",
                            Serial = request.Serial,
                            UltimaTransaccionFecha = ultimaTransaccion.FechaHora
                        };
                    }
                }

                // 5. Si no debe notificar
                return new GeofencingResponse { DebeNotificar = false };
            }
            catch (Exception ex)
            {
                // Log del error
                throw new Exception($"Error en geofencing: {ex.Message}", ex);
            }
        }

        private async Task<OUT_DOMAIN_EQUIPGO.Entities.Procesos.Transacciones> GetUltimaTransaccionPorSerial(string serial)
        {
            // 1. Buscar el equipo por Serial
            var equipo = await _context.Equipos
                .FirstOrDefaultAsync(e => e.Serial == serial);

            if (equipo == null)
            {
                // Log: Equipo no encontrado
                return null;
            }

            // 2. Verificar que el equipo tenga IdUsuarioInfo
            if (!equipo.IdUsuarioInfo.HasValue)
            {
                // Log: Equipo no tiene usuario asignado
                return null;
            }

            // 3. Buscar la última transacción por IdUsuarioInfo
            return await _context.Transacciones
                .Where(t => t.IdUsuarioInfo == equipo.IdUsuarioInfo.Value)
                .OrderByDescending(t => t.FechaHora)
                .FirstOrDefaultAsync();
        }

        private async Task<bool> EstaEnSede(double lat, double lon)
        {
            var zonas = await _context.ZonasSedes
                .AsNoTracking()
                .Select(z => new
                {
                    z.Nombre, // <-- agrega este campo
                    Latitude = (double)z.Latitude,
                    Longitud = (double)z.Longitud,
                    ReadioMetros = (double)z.ReadioMetros
                })
                .ToListAsync();

            Console.WriteLine($"📍 Coordenadas recibidas del agente: {lat}, {lon}");

            foreach (var zona in zonas)
            {
                var distancia = CalculateDistance(lat, lon, zona.Latitude, zona.Longitud);
                Console.WriteLine($"🏢 {zona.Nombre} → Lat={zona.Latitude}, Lon={zona.Longitud}, Radio={zona.ReadioMetros} m, Distancia={distancia} m");

                if (distancia <= zona.ReadioMetros)
                    return true;
            }

            return false;
        }


        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Radio de la Tierra en metros

            double latRad1 = lat1 * Math.PI / 180;
            double latRad2 = lat2 * Math.PI / 180;
            double deltaLat = (lat2 - lat1) * Math.PI / 180;
            double deltaLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(latRad1) * Math.Cos(latRad2) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

    }
}
