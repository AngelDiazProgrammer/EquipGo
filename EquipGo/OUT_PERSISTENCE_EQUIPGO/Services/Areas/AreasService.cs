using Interface;
using Interface.Services.Areas;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Areas;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Estado;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Areas
{
    public class AreasService : IAreasService
    {
        private readonly EquipGoDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public AreasService(EquipGoDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AreaDto>> ObtenerTodasAsync()
        {
            var areas = await _context.Areas.AsNoTracking().ToListAsync();

            var areasDtos = areas.Select(e => new AreaDto
            {
                Id = e.Id,
                NombreArea = e.NombreArea,
            }).ToList();

            return areasDtos;
        }

        public async Task<bool> CrearAreaAdminAsync(AreaDto areaDto)
        {
            if (string.IsNullOrEmpty(areaDto.NombreArea))
                throw new Exception("El nombre es obligatorio");

            var area = new Area
            {
                Id = areaDto.Id,
                NombreArea = areaDto.NombreArea
            };
            await _unitOfWork.Areas.AddAsync(area);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<AreaDto?> ObtenerPorIdAsync(int id)
        {
            var area = await _unitOfWork.Areas.Query()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (area == null)
                return null;

            return new AreaDto
            {
                Id = area.Id,
                NombreArea = area.NombreArea,
            };
        }

        public async Task<bool> ActualizarAreaAdminAsync(int id, AreaDto dto)
        {
            var area = await _unitOfWork.Areas.Query().FirstOrDefaultAsync(e => e.Id == id);
            if (area == null)
                return false;

            area.NombreArea = dto.NombreArea;


            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var area = await _context.Areas.FindAsync(id);
                if (area == null) return false;

                _context.Areas.Remove(area);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar el area: {ex.Message}");
                return false;
            }
        }
    }
}
