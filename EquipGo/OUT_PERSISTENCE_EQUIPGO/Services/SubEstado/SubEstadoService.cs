using Interface;
using Interface.Services.SubEstado;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.SubEstado;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.SubEstados
{
    public class SubEstadoService : ISubEstadoService
    {
        private readonly EquipGoDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public SubEstadoService(EquipGoDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // ✅ Obtener todos los subestados
        public async Task<List<SubEstadoDto>> ObtenerTodosAsync()
        {
            var subEstados = await _context.SubEstados.AsNoTracking().ToListAsync();

            Console.WriteLine($"🧠 Registros encontrados: {subEstados.Count}");

            foreach (var s in subEstados)
                Console.WriteLine($"➡️ ID: {s.Id}, Nombre: {s.NombreSubEstado}");

            var subEstadoDtos = subEstados.Select(s => new SubEstadoDto
            {
                Id = s.Id,
                NombreSubEstado = s.NombreSubEstado
            }).ToList();

            return subEstadoDtos;
        }

        // ✅ Crear un nuevo subestado
        public async Task<bool> CrearSubEstadoAdminAsync(SubEstadoDto subEstadoDto)
        {
            if (string.IsNullOrEmpty(subEstadoDto.NombreSubEstado))
                throw new Exception("El nombre del subestado es obligatorio.");

            var subEstado = new SubEstado
            {
                Id = subEstadoDto.Id,
                NombreSubEstado = subEstadoDto.NombreSubEstado
            };

            await _unitOfWork.SubEstados.AddAsync(subEstado);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ✅ Obtener un subestado por ID
        public async Task<SubEstadoDto?> ObtenerPorIdAsync(int id)
        {
            var subEstado = await _unitOfWork.SubEstados.Query()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subEstado == null)
                return null;

            return new SubEstadoDto
            {
                Id = subEstado.Id,
                NombreSubEstado = subEstado.NombreSubEstado
            };
        }

        // ✅ Actualizar un subestado existente
        public async Task<bool> ActualizarSubEstadoAdminAsync(int id, SubEstadoDto dto)
        {
            var subEstado = await _unitOfWork.SubEstados.Query()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subEstado == null)
                return false;

            subEstado.NombreSubEstado = dto.NombreSubEstado;
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ✅ Eliminar un subestado
        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var subEstado = await _context.SubEstados.FindAsync(id);
                if (subEstado == null) return false;

                _context.SubEstados.Remove(subEstado);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar subestado: {ex.Message}");
                return false;
            }
        }
    }
}
