using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.Services.Equipos
{
    public interface IEquipoService
    {
        Task<List<EquipoDto>> ObtenerTodosLosEquiposAsync();
        Task<EquipoEscaneadoDto?> ConsultarPorCodigoBarrasAsync(string codigoBarras);
    }
}