using Interface;
using Interface.Services.Geofecing;
using Microsoft.EntityFrameworkCore;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Geofecing;
using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Console.WriteLine($"📍 ProcessLocation - Serial: {request.Serial}");

                // 1. Verificar si está dentro de alguna sede
                var estaEnSede = await EstaEnSede(request.Latitude, request.Longitude);
                Console.WriteLine($"📍 ¿Está en sede? {estaEnSede}");

                // 2. Buscar equipo por serial
                var equipo = await _context.Equipos
                    .FirstOrDefaultAsync(e => e.Serial == request.Serial);

                Console.WriteLine($"🔍 Equipo encontrado: {(equipo != null ? $"ID={equipo.Id}" : "NO encontrado")}");

                if (equipo == null)
                {
                    Console.WriteLine("❌ Equipo no encontrado, retornando false");
                    return new GeofencingResponse { DebeNotificar = false };
                }

                // 3. Si está FUERA de todas las sedes
                if (!estaEnSede)
                {
                    Console.WriteLine("🚫 Equipo FUERA de sede");

                    // 4. Buscar última transacción por SERIAL
                    var ultimaTransaccion = await GetUltimaTransaccionPorSerial(request.Serial);
                    Console.WriteLine($"📋 Última transacción: {(ultimaTransaccion != null ? $"Tipo={ultimaTransaccion.IdTipoTransaccion}" : "NO encontrada")}");

                    // 5. Si la última transacción fue una ENTRADA (1)
                    if (ultimaTransaccion != null && ultimaTransaccion.IdTipoTransaccion == 1)
                    {
                        Console.WriteLine("✅ Condición cumplida: Última transacción = ENTRADA");

                        // 🔄 OBTENER CONTADOR PERSISTENTE DE BD
                        int contadorActual = await ObtenerContadorPersistente(request.Serial);
                        int nuevoContador = Math.Min(contadorActual + 1, 5); // Máximo 5

                        Console.WriteLine($"🔢 Contador BD: {contadorActual} → {nuevoContador}");

                        // 📊 DETERMINAR TIPO DE ALERTA SEGÚN CONTADOR
                        var (idTipoAlerta, descripcion) = DeterminarTipoAlerta(nuevoContador);
                        Console.WriteLine($"📊 Tipo Alerta: {idTipoAlerta}, Desc: {descripcion}");

                        // 💾 GUARDAR CONTADOR EN BD
                        await GuardarContadorPersistente(request.Serial, nuevoContador);

                        // 📝 REGISTRAR ALERTA EN BD
                        await RegistrarOActualizarAlertaEnBD(equipo.Id, idTipoAlerta, descripcion);

                        string mensaje = nuevoContador >= 5
                            ? "🚫 ¡ALERTA CRÍTICA! El equipo lleva 5 notificaciones consecutivas fuera de sede. Contacte al administrador."
                            : $"⚠️ ¡Atención! No olvides registrar la SALIDA de tu equipo la próxima vez que visites la SEDE. ({nuevoContador}/5)";

                        Console.WriteLine($"📤 Enviando respuesta: DebeNotificar=true, Contador={nuevoContador}");

                        return new GeofencingResponse
                        {
                            DebeNotificar = true,
                            Mensaje = mensaje,
                            Serial = request.Serial,
                            UltimaTransaccionFecha = ultimaTransaccion.FechaHora,
                            ContadorNotificaciones = nuevoContador,
                            NivelAlerta = idTipoAlerta
                        };
                    }
                    else
                    {
                        // 📍 SI ESTÁ FUERA DE SEDE PERO NO DEBE NOTIFICAR
                        Console.WriteLine("ℹ️ Fuera de sede pero NO debe notificar - Reiniciando contador");
                        await ReiniciarContadorPersistente(request.Serial);
                        await RegistrarOActualizarAlertaEnBD(equipo.Id, 5, "Equipo fuera de sede - Sin alerta activa (sin transacción de entrada pendiente)");
                    }
                }
                else
                {
                    // 🔄 SI ESTÁ EN SEDE, REINICIAR CONTADOR Y REGISTRAR "SIN ALERTA"
                    Console.WriteLine("✅ Equipo EN sede - Reiniciando contador");
                    await ReiniciarContadorPersistente(request.Serial);
                    await RegistrarOActualizarAlertaEnBD(equipo.Id, 5, "Equipo dentro de sede autorizada - Sin alerta");
                }

                // 6. Si no debe notificar
                Console.WriteLine("📤 Enviando respuesta: DebeNotificar=false");
                return new GeofencingResponse { DebeNotificar = false };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en ProcessLocation: {ex.Message}");
                Console.WriteLine($"💥 StackTrace: {ex.StackTrace}");
                throw new Exception($"Error en geofencing: {ex.Message}", ex);
            }
        }

        // 🔄 MÉTODOS PERSISTENTES PARA CONTADOR

        private async Task<int> ObtenerContadorPersistente(string serial)
        {
            try
            {
                var contador = await _context.ContadoresGeofencing
                    .FirstOrDefaultAsync(c => c.Serial == serial);

                int resultado = contador?.Contador ?? 0;
                Console.WriteLine($"📊 Contador persistente para {serial}: {resultado}");
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo contador persistente: {ex.Message}");
                return 0;
            }
        }

        private async Task GuardarContadorPersistente(string serial, int contador)
        {
            try
            {
                Console.WriteLine($"💾 Guardando contador persistente: {serial} = {contador}");

                var contadorExistente = await _context.ContadoresGeofencing
                    .FirstOrDefaultAsync(c => c.Serial == serial);

                if (contadorExistente != null)
                {
                    contadorExistente.Contador = contador;
                    contadorExistente.FechaUltimaNotificacion = DateTime.Now;
                    contadorExistente.FechaActualizacion = DateTime.Now;
                    Console.WriteLine($"✏️ Actualizando contador existente: {contadorExistente.Id}");
                }
                else
                {
                    var nuevoContador = new ContadorGeofencing
                    {
                        Serial = serial,
                        Contador = contador,
                        FechaUltimaNotificacion = DateTime.Now,
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now
                    };
                    _context.ContadoresGeofencing.Add(nuevoContador);
                    Console.WriteLine($"🆕 Creando nuevo contador para: {serial}");
                }

                int cambios = await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Contador guardado en BD: {cambios} cambios");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error guardando contador persistente: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        private async Task ReiniciarContadorPersistente(string serial)
        {
            try
            {
                var contadorExistente = await _context.ContadoresGeofencing
                    .FirstOrDefaultAsync(c => c.Serial == serial);

                if (contadorExistente != null && contadorExistente.Contador > 0)
                {
                    Console.WriteLine($"🔄 Reiniciando contador: {serial} (previo: {contadorExistente.Contador})");
                    contadorExistente.Contador = 0;
                    contadorExistente.FechaActualizacion = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"ℹ️ Contador ya está en 0 o no existe: {serial}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error reiniciando contador: {ex.Message}");
            }
        }

        // 📊 MÉTODO PARA DETERMINAR TIPO DE ALERTA
        private (int idTipoAlerta, string descripcion) DeterminarTipoAlerta(int contador)
        {
            return contador switch
            {
                1 => (1, "Primera notificación: Equipo detectado fuera de sede sin registro de salida"),
                2 => (1, "Segunda notificación: Equipo permanece fuera de sede sin registro de salida"),
                3 => (2, "Tercera notificación: Alerta media - Equipo fuera de sede por tiempo prolongado"),
                4 => (3, "Cuarta notificación: Alerta alta - Equipo fuera de sede crítico"),
                5 => (4, "Quinta notificación: Alerta bloqueada - Equipo fuera de sede, contacto administrador requerido"),
                _ => (1, $"Notificación #{contador}: Equipo fuera de sede sin registro de salida")
            };
        }

        // 💾 MÉTODO PARA REGISTRAR O ACTUALIZAR ALERTA EN BD
        private async Task RegistrarOActualizarAlertaEnBD(int idEquipo, int idTipoAlerta, string descripcion)
        {
            try
            {
                Console.WriteLine($"💾 Registrando alerta - IdEquipo: {idEquipo}, TipoAlerta: {idTipoAlerta}");

                var alertaExistente = await _context.AlertasGeofencing
                    .FirstOrDefaultAsync(a => a.IdEquipo == idEquipo);

                Console.WriteLine($"🔍 Alerta existente: {(alertaExistente != null ? "SI" : "NO")}");

                if (alertaExistente != null)
                {
                    alertaExistente.IdTipoAlerta = idTipoAlerta;
                    alertaExistente.Descripcion = descripcion;
                    alertaExistente.Fecha = DateTime.Now;
                    Console.WriteLine($"✏️ Actualizando alerta existente ID: {alertaExistente.Id}");
                }
                else
                {
                    var nuevaAlerta = new AlertasGeofencing
                    {
                        IdEquipo = idEquipo,
                        IdTipoAlerta = idTipoAlerta,
                        Descripcion = descripcion,
                        Fecha = DateTime.Now
                    };
                    _context.AlertasGeofencing.Add(nuevaAlerta);
                    Console.WriteLine($"🆕 Creando nueva alerta para equipo: {idEquipo}");
                }

                int cambios = await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Alerta guardada en BD: {cambios} cambios");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR registrando alerta: {ex.Message}");
            }
        }


        private async Task<OUT_DOMAIN_EQUIPGO.Entities.Procesos.Transacciones> GetUltimaTransaccionPorSerial(string serial)
        {
            var equipo = await _context.Equipos
                .FirstOrDefaultAsync(e => e.Serial == serial);

            if (equipo == null || !equipo.IdUsuarioInfo.HasValue)
                return null;

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
                    z.Nombre,
                    Latitude = (double)z.Latitude,
                    Longitud = (double)z.Longitud,
                    ReadioMetros = (double)z.ReadioMetros
                })
                .ToListAsync();

            foreach (var zona in zonas)
            {
                var distancia = CalculateDistance(lat, lon, zona.Latitude, zona.Longitud);
                if (distancia <= zona.ReadioMetros)
                    return true;
            }

            return false;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
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