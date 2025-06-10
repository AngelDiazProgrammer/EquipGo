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
        Task<bool> RegistrarTransaccionAsync(TransaccionRequest request);
        Task<List<TransaccionDashboardDto>> ObtenerTransaccionesHoyAsync();

    }
}
