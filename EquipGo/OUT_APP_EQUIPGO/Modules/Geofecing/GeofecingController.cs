// GeofencingController.cs
using Interface.Services.Geofecing;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Geofecing;

[ApiController]
[Route("api/[controller]")]
public class GeofencingController : ControllerBase
{
    private readonly IGeofencingService _geofencingService;

    public GeofencingController(IGeofencingService geofencingService)
    {
        _geofencingService = geofencingService;
    }

    [HttpPost("process-location")]
    public async Task<ActionResult<GeofencingResponse>> ProcessLocation([FromBody] LocationRequest request)
    {
        try
        {
            var result = await _geofencingService.ProcessLocation(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error procesando ubicación: {ex.Message}");
        }
    }
}