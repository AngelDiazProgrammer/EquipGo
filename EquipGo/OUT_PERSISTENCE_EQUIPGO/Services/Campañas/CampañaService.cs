using Interface.Services.Campañas;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Campañas
{
    public class CampañaService : ICampañaService
    {
        private readonly EquipGoDbContext _context;

        public CampañaService(EquipGoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Campaña>> ObtenerTodasAsync()
        {
            return await _context.Campañas.AsNoTracking().ToListAsync();
        }
    }
}
