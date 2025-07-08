using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Areas
{
    public interface IAreasService
    {
        Task<List<AreaDto>> ObtenerTodasAsync();
        Task<bool> CrearAreaAdminAsync(AreaDto areaDto);
        Task<AreaDto?> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarAreaAdminAsync(int id, AreaDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
