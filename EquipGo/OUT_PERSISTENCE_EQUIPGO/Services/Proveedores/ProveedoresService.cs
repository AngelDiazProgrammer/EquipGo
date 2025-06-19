using Interface.Services.Proveedores;
using Interface.Services.Sedes;
using Microsoft.EntityFrameworkCore;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Proveedores
{
    public class ProveedoresService : IProveedoresService
    {
        private readonly EquipGoDbContext _context;
        public ProveedoresService(EquipGoDbContext context)
        {
            _context = context;
        }

        public async Task<List<OUT_DOMAIN_EQUIPGO.Entities.Smart.Proveedores>> ObtenerTodasAsync()
        {
            return await _context.Proveedores.AsNoTracking().ToListAsync();
        }
    }
}
