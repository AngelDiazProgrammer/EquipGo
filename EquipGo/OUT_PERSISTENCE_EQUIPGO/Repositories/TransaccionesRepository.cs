using Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class TransaccionesRepository : GenericRepository<Transacciones>, ITransaccionesRepository
    {
        public TransaccionesRepository(EquipGoDbContext context) : base(context) { }

        public async Task<List<Transacciones>> GetHistorialByCodigoBarrasAsync(string codigoBarras)
        {
            return await _context.Transacciones
                .Where(t => t.CodigoBarras == codigoBarras)
                .OrderByDescending(t => t.FechaHora)
                .ToListAsync();
        }
    }
}
