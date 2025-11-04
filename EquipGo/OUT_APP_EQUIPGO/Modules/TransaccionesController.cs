using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController : ControllerBase
{
    private readonly ITransaccionService _transaccionService;

    public TransaccionesController(ITransaccionService transaccionService)
    {
        _transaccionService = transaccionService;
    }

    // GET: api/transacciones/todas
    [HttpGet("todas")]
    public async Task<IActionResult> ObtenerTodasLasTransacciones()
    {
        try
        {
            var transacciones = await _transaccionService.ObtenerTodasLasTransaccionesAsync();
            return Ok(new
            {
                Success = true,
                Data = transacciones,
                Count = transacciones.Count,
                Message = "Transacciones obtenidas correctamente"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al obtener las transacciones",
                Error = ex.Message
            });
        }
    }

    // GET: api/transacciones/todas-visitantes
    [HttpGet("todas-visitantes")]
    public async Task<IActionResult> ObtenerTodasLasTransaccionesVisitantes()
    {
        try
        {
            var transacciones = await _transaccionService.ObtenerTodasLasTransaccionesVisitantesAsync();
            return Ok(new
            {
                Success = true,
                Data = transacciones,
                Count = transacciones.Count,
                Message = "Transacciones de visitantes obtenidas correctamente"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al obtener las transacciones de visitantes",
                Error = ex.Message
            });
        }
    }


    #region Excel
    // NUEVOS ENDPOINTS PARA REPORTES CON FILTROS SIMPLES
    [HttpPost("reporte-transacciones")]
    public async Task<IActionResult> GenerarReporteTransacciones([FromBody] FiltrosTransaccionDto filtros)
    {
        try
        {
            var transacciones = await _transaccionService.ObtenerTransaccionesFiltradasAsync(filtros);

            return Ok(new
            {
                Success = true,
                Data = transacciones,
                Count = transacciones.Count,
                Message = "Reporte de transacciones generado correctamente",
                FiltrosAplicados = filtros
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al generar el reporte de transacciones",
                Error = ex.Message
            });
        }
    }

    [HttpPost("reporte-transacciones-visitantes")]
    public async Task<IActionResult> GenerarReporteTransaccionesVisitantes([FromBody] FiltrosTransaccionVisitanteDto filtros)
    {
        try
        {
            var transacciones = await _transaccionService.ObtenerTransaccionesVisitantesFiltradasAsync(filtros);

            return Ok(new
            {
                Success = true,
                Data = transacciones,
                Count = transacciones.Count,
                Message = "Reporte de transacciones de visitantes generado correctamente",
                FiltrosAplicados = filtros
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al generar el reporte de transacciones de visitantes",
                Error = ex.Message
            });
        }
    }

    // ENDPOINT PARA DESCARGAR EXCEL CON FILTROS SIMPLES
    [HttpPost("descargar-excel-transacciones")]
    public async Task<IActionResult> DescargarExcelTransacciones([FromBody] FiltrosTransaccionDto filtros)
    {
        try
        {
            var transacciones = await _transaccionService.ObtenerTransaccionesFiltradasAsync(filtros);

            // Aquí iría la lógica para generar el Excel
            // Por ahora retornamos los datos para que JavaScript genere el Excel
            return Ok(new
            {
                Success = true,
                Data = transacciones,
                Count = transacciones.Count,
                Message = "Datos para Excel generados correctamente",
                FiltrosAplicados = filtros
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al generar el Excel de transacciones",
                Error = ex.Message
            });
        }
    }

    [HttpPost("descargar-excel-transacciones-visitantes")]
    public async Task<IActionResult> DescargarExcelTransaccionesVisitantes([FromBody] FiltrosTransaccionVisitanteDto filtros)
    {
        try
        {
            var transacciones = await _transaccionService.ObtenerTransaccionesVisitantesFiltradasAsync(filtros);

            return Ok(new
            {
                Success = true,
                Data = transacciones,
                Count = transacciones.Count,
                Message = "Datos para Excel generados correctamente",
                FiltrosAplicados = filtros
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al generar el Excel de transacciones de visitantes",
                Error = ex.Message
            });
        }
    }
    #endregion
}