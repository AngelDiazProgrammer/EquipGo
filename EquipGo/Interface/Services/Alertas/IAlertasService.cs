using OUT_OS_APP.EQUIPGO.DTO.DTOs.Alertas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Alertas
{
    public interface IAlertasService
    {
        Task<List<AlertaDto>> ObtenerTodasAlertasAsync();
        Task<List<AlertaDto>> ObtenerAlertasRecientesAsync();
        Task<List<AlertaDto>> ObtenerAlertasPorTipoAsync(int idTipoAlerta);
        Task<List<AlertaDto>> ObtenerEquiposBloqueadosAsync();
        Task<List<TipoAlertaDto>> ObtenerTiposAlertaAsync();
        Task<bool> MarcarAlertaComoRevisadaAsync(int idAlerta);
        Task<bool> ReiniciarContadorAsync(string serial);
        Task<int> ObtenerTotalAlertasHoyAsync();
        Task<int> ObtenerTotalEquiposBloqueadosAsync();
    }
}
