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
            var equipo = await _unit.Equipos.GetByCodigoBarrasAsync(codigoBarras);

            if (equipo == null) return null;

            var usuario = await _unit.UsuariosInformacion.GetByIdAsync(equipo.IdUsuarioInfo);

            var historial = await _unit.Transacciones
                .FindAsync(t => t.CodigoBarras == codigoBarras);

            var historialTexto = historial
                .OrderByDescending(t => t.FechaHora)
                .Take(5)
                .Select(t => $"{t.FechaHora:G} - {t.Usuario}")
                .ToList();

            return new EquipoEscaneadoDto
            {
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                Serial = equipo.Serial,
                Ubicacion = equipo.Ubicacion,
                CodigoBarras = equipo.CodigoBarras,

                NombreUsuario = $"{usuario?.Nombres} {usuario?.Apellidos}",
                DocumentoUsuario = usuario?.NumeroDocumento,
                Area = usuario?.IdArea.ToString(), // luego se puede mapear el nombre
                Campaña = usuario?.IdCampaña.ToString(),

                HistorialTransacciones = historialTexto
            };
        }
    }
}
