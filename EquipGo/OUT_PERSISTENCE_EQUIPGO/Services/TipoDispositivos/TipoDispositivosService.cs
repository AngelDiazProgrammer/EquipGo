using Interface.Services.TipoDispositivos;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.TipoDispositivos
{
    public class TipoDispositivosService : ITipoDispositivosService
    {
        private readonly EquipGoDbContext _context;

        public TipoDispositivosService(EquipGoDbContext context)
        {
            _context = context;
        }
        public async Task<List<TiposDispositivos>> ObtenerTodasAsync()
        {
            return await _context.TiposDispositivos.AsNoTracking().ToListAsync();
        }
    }
}
