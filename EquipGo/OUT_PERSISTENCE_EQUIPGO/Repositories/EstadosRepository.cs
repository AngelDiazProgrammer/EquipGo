using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface.Repositories;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class EstadosRepository : GenericRepository<Estado>, IEstadosRepository
    {
        public EstadosRepository(EquipGoDbContext context) : base(context){ }
        public async Task<Estado?> GetByIdAsync(int id)
        {
            return await _context.Estados.FindAsync(id);
        }
    }
}
