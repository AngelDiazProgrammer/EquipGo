using Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class EquiposRepository : GenericRepository<Equipos>, IEquiposRepository
    {
        public EquiposRepository(EquipGoDbContext context) : base(context) { }

        public async Task<Equipos?> GetByCodigoBarrasAsync(string codigoBarras)
        {
            return await _context.Equipos.FirstOrDefaultAsync(e => e.CodigoBarras == codigoBarras);
        }

    }
}
