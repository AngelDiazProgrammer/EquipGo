using Interface.Repositories;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class AreaRepository : GenericRepository<Area>, IAreaRepository
    {
        public AreaRepository(EquipGoDbContext context) : base(context) { }

        public async Task<Area?> GetByIdAsync(int id)
        {
            return await _context.Areas.FindAsync(id);
        }
    }
}
