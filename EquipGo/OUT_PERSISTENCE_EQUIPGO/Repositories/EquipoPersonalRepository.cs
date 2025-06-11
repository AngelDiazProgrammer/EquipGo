using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class EquipoPersonalRepository : IEquipoPersonalRepositoy
    {
        private readonly EquipGoDbContext _context;

        public EquipoPersonalRepository(EquipGoDbContext context)
        {
            _context = context;
        }

        public IQueryable<EquiposPersonal> Query()
        {
            return _context.EquiposPersonales.AsQueryable();
        }

        public async Task AddAsync(EquiposPersonal entity)
        {
            await _context.EquiposPersonales.AddAsync(entity);
        }

        // Aquí podrías agregar más métodos si los necesitas
    }
}
