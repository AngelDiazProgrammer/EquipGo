using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Transacciones
{
    public interface ITransaccionService
    {
        Task<TransaccionRequest> ObtenerUltimaTransaccionPorCodigoBarrasAsync(string codigoBarras);
        Task<bool> RegistrarTransaccionAsync(TransaccionRequest request);
        Task<List<TransaccionDashboardDto>> ObtenerTransaccionesHoyAsync();
        Task<List<TransaccionDashboardDto>> ObtenerTodasLasTransaccionesAsync();
        Task<List<TransaccionVisitanteDashboardDto>> ObtenerTransaccionesVisitantesHoyAsync();
        Task<List<TransaccionVisitanteDashboardDto>> ObtenerTodasLasTransaccionesVisitantesAsync();
        Task<ConteoTransaccionesDto> ObtenerConteosDashboardAsync();

        // NUEVOS MÉTODOS PARA REPORTES CON FILTROS
        Task<List<TransaccionDashboardDto>> ObtenerTransaccionesFiltradasAsync(FiltrosTransaccionDto filtros);
        Task<List<TransaccionVisitanteDashboardDto>> ObtenerTransaccionesVisitantesFiltradasAsync(FiltrosTransaccionVisitanteDto filtros);
 

        //transacciones de visitantes
        Task<bool> RegistrarTransaccionVisitanteAsync(TransaccionVisitanteRequest request);

    }
}
