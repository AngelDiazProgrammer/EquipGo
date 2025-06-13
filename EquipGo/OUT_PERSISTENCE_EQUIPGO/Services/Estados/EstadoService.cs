using Interface;
using Interface.Services.Estados;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Estado;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Estados
{
    public class EstadoService : IEstadoService
    {
        private readonly EquipGoDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public EstadoService(EquipGoDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EstadoDto>> ObtenerTodasAsync()
        {
            var estados = await _context.Estados.AsNoTracking().ToListAsync();

            var estadoDtos = estados.Select(e => new EstadoDto
            {
                Id = e.Id,
                NombreEstado = e.NombreEstado
            }).ToList();

            return estadoDtos;
        }

        public async Task<bool> CrearEstadoAdminAsync(EstadoDto estadoDto)
        {
            if (string.IsNullOrEmpty(estadoDto.NombreEstado))
                throw new Exception("El nombre es obligatorio");

            var estado = new Estado
            {
                Id = estadoDto.Id,
                NombreEstado = estadoDto.NombreEstado
            };
            await _unitOfWork.Estados.AddAsync(estado);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<EstadoDto?> ObtenerPorIdAsync(int id)
        {
            var estado = await _unitOfWork.Estados.Query()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estado == null)
                return null;

            return new EstadoDto
            {
                Id = estado.Id,
                NombreEstado = estado.NombreEstado,
            };
        }

        public async Task<bool> ActualizarEstadoAdminAsync(int id, EstadoDto dto)
        {
            var estado = await _unitOfWork.Estados.Query().FirstOrDefaultAsync(e => e.Id == id);
            if (estado == null)
                return false;

            estado.NombreEstado = dto.NombreEstado;


            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var estado = await _context.Estados.FindAsync(id);
                if (estado == null) return false;

                _context.Estados.Remove(estado);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar estado: {ex.Message}");
                return false;
            }
        }

    }
}
