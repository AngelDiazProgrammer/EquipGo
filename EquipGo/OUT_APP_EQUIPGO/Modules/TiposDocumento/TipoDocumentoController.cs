using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OUT_PERSISTENCE_EQUIPGO.Context;


[ApiController]
[Route("api/[controller]")]
public class TiposDocumentosController : ControllerBase
{
    private readonly EquipGoDbContext _context;

    public TiposDocumentosController(EquipGoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var tiposdocumento = await _context.TipoDocumento
            .Select(t => new { t.Id, t.NombreDocumento })
            .ToListAsync();

        return Ok(tiposdocumento);
    }
}