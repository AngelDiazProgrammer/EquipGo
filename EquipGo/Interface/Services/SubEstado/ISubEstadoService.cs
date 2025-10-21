using OUT_OS_APP.EQUIPGO.DTO.DTOs.SubEstado;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.Services.SubEstado
{
    public interface ISubEstadoService
    {
        Task<List<SubEstadoDto>> ObtenerTodosAsync();
        Task<bool> CrearSubEstadoAdminAsync(SubEstadoDto subEstadoDto);
        Task<SubEstadoDto?> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarSubEstadoAdminAsync(int id, SubEstadoDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
