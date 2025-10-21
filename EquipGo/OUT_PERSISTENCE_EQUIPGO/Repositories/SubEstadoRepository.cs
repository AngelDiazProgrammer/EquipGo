using Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class SubEstadoRepository : GenericRepository<SubEstado>, ISubEstadoRepository
    {
        public SubEstadoRepository(EquipGoDbContext context) : base(context) { }
        public async Task<SubEstado?> GetByIdAsync(int id)
        {
            return await _context.SubEstados.FindAsync(id);
        }
    }
}
