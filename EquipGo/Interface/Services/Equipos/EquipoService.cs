using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Equipos
{
    public class EquipoService : IEquipoService
    {
        private readonly IUnitOfWork _unit;

        public EquipoService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<EquipoEscaneadoDto?> ConsultarPorCodigoBarrasAsync(string codigoBarras)
        {
            // Consultar el equipo
            var equipo = await _unit.Equipos.GetByCodigoBarrasAsync(codigoBarras);

            if (equipo == null)
            {
                Console.WriteLine("⚠️ Equipo no encontrado.");
                return null;
            }

            // Log de equipo
            Console.WriteLine($"✅ Equipo encontrado: {equipo.Marca} - {equipo.Modelo}");

            // Consultar el usuario
            var usuario = await _unit.UsuariosInformacion.GetByIdAsync(equipo.IdUsuarioInfo);

            if (usuario == null)
            {
                Console.WriteLine("⚠️ Usuario no encontrado para el equipo.");
            }
            else
            {
                Console.WriteLine($"✅ Usuario: {usuario.Nombres} {usuario.Apellidos} - Documento: {usuario.NumeroDocumento}");
            }

            // Consultar historial de transacciones
            var historial = (await _unit.Transacciones
     .FindAsync(t => t.CodigoBarras == codigoBarras))
     .ToList();


            Console.WriteLine($"✅ Historial de transacciones: {historial.Count} registros");

            // Construir y retornar el DTO
            return new EquipoEscaneadoDto
            {
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                Serial = equipo.Serial,
                Ubicacion = equipo.Ubicacion,
                CodigoBarras = equipo.CodigoBarras,

                NombreUsuario = usuario != null ? $"{usuario.Nombres} {usuario.Apellidos}" : "No asignado",
                DocumentoUsuario = usuario?.NumeroDocumento ?? "N/A",
                Area = usuario?.IdArea.ToString() ?? "N/A",
                Campaña = usuario?.IdCampaña.ToString() ?? "N/A",


                HistorialTransacciones = historial
                    .OrderByDescending(t => t.FechaHora)
                    .Take(5)
                    .Select(t => $"{t.FechaHora:G} - {t.Usuario}")
                    .ToList()
            };
        }
    }
}


