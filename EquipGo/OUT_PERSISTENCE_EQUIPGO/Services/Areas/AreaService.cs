using Interface.Services.Areas;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Areas
{
    public class AreaService : IAreaService
    {
        private readonly EquipGoDbContext _context;

        public AreaService(EquipGoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Area>> ObtenerTodasAsync()
        {
            return await _context.Areas.AsNoTracking().ToListAsync();
        }
    }
}
