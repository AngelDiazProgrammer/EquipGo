using Microsoft.EntityFrameworkCore;
using Interface.Services.Sedes;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Sedes
{
    public class SedesService : ISedesService
    {
        private readonly EquipGoDbContext _context;
        public SedesService(EquipGoDbContext context)
        {
            _context = context;
        }

        public async Task<List<OUT_DOMAIN_EQUIPGO.Entities.Smart.Sedes>> ObtenerTodasAsync()
        {
            return await _context.Sedes.AsNoTracking().ToListAsync();
        }
    }
}
