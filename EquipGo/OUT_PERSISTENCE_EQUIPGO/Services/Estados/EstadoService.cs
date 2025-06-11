using Interface.Services.Estados;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Estados
{
    public class EstadoService : IEstadoService
    {
        private readonly EquipGoDbContext _context;

        public EstadoService(EquipGoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Estado>> ObtenerTodasAsync()
        {
            return await _context.Estados.AsNoTracking().ToListAsync();
        }
    }
}
