using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OUT_PERSISTENCE_EQUIPGO.Context;


    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly EquipGoDbContext _context;

        public ProveedoresController(EquipGoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var proveedores = await _context.Proveedores
                .Select(p => new { p.Id, p.NombreProveedor })
                .ToListAsync();

            return Ok(proveedores);
        }
    }