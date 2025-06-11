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
    public class SedesRepository : GenericRepository<Sedes>, ISedesRepository
    {
        public SedesRepository(EquipGoDbContext context) : base(context){}

        public async Task<Sedes?> GetByIdAsync(int id)
        {
            return await _context.Sedes.FindAsync(id);
        }
    }
}
