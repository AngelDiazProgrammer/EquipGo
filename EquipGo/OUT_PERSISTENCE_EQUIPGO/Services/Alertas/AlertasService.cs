using Interface;
using Interface.Services.Alertas;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Alertas;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Alertas
{
    public class AlertasService : IAlertasService
    {
        private readonly EquipGoDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public AlertasService(EquipGoDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AlertaDto>> ObtenerTodasAlertasAsync()
        {
            var alertas = await (from alerta in _context.AlertasGeofencing
                                 join tipo in _context.TiposAlerta on alerta.IdTipoAlerta equals tipo.Id
                                 join equipo in _context.Equipos on alerta.IdEquipo equals equipo.Id
                                 join contador in _context.ContadoresGeofencing on equipo.Serial equals contador.Serial into contadores
                                 from contador in contadores.DefaultIfEmpty()
                                 select new AlertaDto
                                 {
                                     Id = alerta.Id,
                                     IdEquipo = alerta.IdEquipo,
                                     IdTipoAlerta = alerta.IdTipoAlerta,
                                     Descripcion = alerta.Descripcion,
                                     Fecha = alerta.Fecha,
                                     NombreAlerta = tipo.NombreAlerta,
                                     SerialEquipo = equipo.Serial,
                                     MarcaEquipo = equipo.Marca,
                                     ModeloEquipo = equipo.Modelo,
                                     Contador = contador != null ? contador.Contador : 0
                                 })
                                .OrderByDescending(a => a.Fecha)
                                .ToListAsync();

            return alertas;
        }

        public async Task<List<AlertaDto>> ObtenerAlertasRecientesAsync()
        {
            var alertas = await ObtenerTodasAlertasAsync();
            return alertas.Take(100).ToList();
        }

        public async Task<List<AlertaDto>> ObtenerAlertasPorTipoAsync(int idTipoAlerta)
        {
            var alertas = await ObtenerTodasAlertasAsync();
            return alertas.Where(a => a.IdTipoAlerta == idTipoAlerta).ToList();
        }

        public async Task<List<AlertaDto>> ObtenerEquiposBloqueadosAsync()
        {
            // Buscar equipos que tienen contador >= 5 Y alerta activa de tipo 4 (bloqueo)
            var equiposBloqueados = await (from contador in _context.ContadoresGeofencing
                                           where contador.Contador >= 5
                                           join equipo in _context.Equipos on contador.Serial equals equipo.Serial
                                           join alerta in _context.AlertasGeofencing on equipo.Id equals alerta.IdEquipo
                                           where alerta.IdTipoAlerta == 4 // Solo alertas de bloqueo activas
                                           select new
                                           {
                                               Serial = contador.Serial,
                                               Contador = contador.Contador,
                                               Equipo = equipo,
                                               Alerta = alerta
                                           })
                                          .ToListAsync();

            var resultado = new List<AlertaDto>();

            foreach (var bloqueado in equiposBloqueados)
            {
                var tipoAlerta = await _context.TiposAlerta
                    .FirstOrDefaultAsync(t => t.Id == bloqueado.Alerta.IdTipoAlerta);

                resultado.Add(new AlertaDto
                {
                    Id = bloqueado.Alerta.Id,
                    IdEquipo = bloqueado.Alerta.IdEquipo,
                    IdTipoAlerta = bloqueado.Alerta.IdTipoAlerta,
                    Descripcion = bloqueado.Alerta.Descripcion,
                    Fecha = bloqueado.Alerta.Fecha,
                    NombreAlerta = tipoAlerta?.NombreAlerta ?? "Desconocido",
                    SerialEquipo = bloqueado.Serial,
                    MarcaEquipo = bloqueado.Equipo.Marca,
                    ModeloEquipo = bloqueado.Equipo.Modelo,
                    Contador = bloqueado.Contador
                });
            }

            return resultado;
        }

        public async Task<List<TipoAlertaDto>> ObtenerTiposAlertaAsync()
        {
            var tipos = await _context.TiposAlerta
                .Select(t => new TipoAlertaDto
                {
                    Id = t.Id,
                    NombreAlerta = t.NombreAlerta
                })
                .ToListAsync();

            return tipos;
        }

        public async Task<bool> MarcarAlertaComoRevisadaAsync(int idAlerta)
        {
            // Proximas implementaciones
            return await Task.FromResult(true);
        }

        public async Task<bool> ReiniciarContadorAsync(string serial)
        {
            try
            {
                // 1. Reiniciar el contador a 0
                var contador = await _context.ContadoresGeofencing
                    .FirstOrDefaultAsync(c => c.Serial == serial);

                if (contador == null)
                    return false;

                contador.Contador = 0;
                contador.FechaActualizacion = DateTime.Now;

                // 2. Buscar el equipo por serial
                var equipo = await _context.Equipos
                    .FirstOrDefaultAsync(e => e.Serial == serial);

                if (equipo == null)
                    return false;

                // 3. Buscar la alerta activa más reciente para este equipo
                var alertaActiva = await _context.AlertasGeofencing
                    .Where(a => a.IdEquipo == equipo.Id)
                    .OrderByDescending(a => a.Fecha)
                    .FirstOrDefaultAsync();

                if (alertaActiva != null)
                {
                    // 4. Cambiar el IdTipoAlerta a 5 (sin alerta) y actualizar descripción
                    alertaActiva.IdTipoAlerta = 5;
                    alertaActiva.Descripcion = "Equipo desbloqueado - Sin alerta activa";
                    alertaActiva.Fecha = DateTime.Now; // Actualizar fecha
                }
                else
                {
                    // 5. Si no existe alerta, crear una nueva con tipo 5
                    var nuevaAlerta = new AlertasGeofencing
                    {
                        IdEquipo = equipo.Id,
                        IdTipoAlerta = 5,
                        Descripcion = "Equipo desbloqueado - Sin alerta activa",
                        Fecha = DateTime.Now
                    };
                    _context.AlertasGeofencing.Add(nuevaAlerta);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al reiniciar contador y desbloquear: {ex.Message}");
                return false;
            }
        }

        public async Task<int> ObtenerTotalAlertasHoyAsync()
        {
            var hoy = DateTime.Today;
            var alertasHoy = await _context.AlertasGeofencing
                .CountAsync(a => a.Fecha.Date == hoy);

            return alertasHoy;
        }

        public async Task<int> ObtenerTotalEquiposBloqueadosAsync()
        {
            var equiposBloqueados = await ObtenerEquiposBloqueadosAsync();
            return equiposBloqueados.Count;
        }
    }
}