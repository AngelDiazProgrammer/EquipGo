using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Estado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Estados
{
    public interface IEstadoService
    {
        Task<List<EstadoDto>> ObtenerTodasAsync();
        Task<bool> CrearEstadoAdminAsync(EstadoDto estadoDto);
        Task<EstadoDto?> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarEstadoAdminAsync(int id, EstadoDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
