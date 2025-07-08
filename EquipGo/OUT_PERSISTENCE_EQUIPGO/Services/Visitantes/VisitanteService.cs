using Domain.Entities.Procesos;
using Interface.Services.Visitantes;
using Microsoft.EntityFrameworkCore;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;
using OUT_PERSISTENCE_EQUIPGO.Context;

namespace Application.Services.Visitantes
{
    public class VisitanteService : IVisitanteService
    {
        private readonly EquipGoDbContext _context;

        public VisitanteService(EquipGoDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarVisitanteAsync(RegistroVisitanteDto dto)
        {
            // Verificar si el visitante ya existe
            var visitante = await _context.UsuariosVisitantes
                .FirstOrDefaultAsync(v =>
                    v.TipoDocumento == dto.TipoDocumento &&
                    v.NumeroDocumento == dto.NumeroDocumento);

            if (visitante == null)
            {
                visitante = new UsuariosVisitantes
                {
                    TipoDocumento = dto.TipoDocumento,
                    NumeroDocumento = dto.NumeroDocumento,
                    Nombres = dto.Nombres,
                    Apellidos = dto.Apellidos,
                    TipoUsuario = dto.TipoUsuario,
                    IdProveedor = dto.IdProveedor,
                    FechaRegistro = DateTime.Now
                };

                _context.UsuariosVisitantes.Add(visitante);
                await _context.SaveChangesAsync();
            }

            // Crear equipo visitante
            var equipo = new EquiposVisitantes
            {
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Serial = dto.Serial,
                Foto = dto.FotoBase64,
                IdUsuarioVisitante = visitante.Id,
                IdSede = null, // se deja nulo como indicaste
                FechaRegistro = DateTime.Now
            };

            _context.EquiposVisitantes.Add(equipo);
            await _context.SaveChangesAsync();
        }


    }
}
