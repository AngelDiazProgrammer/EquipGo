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
    public class UsuariosInformacionRepository : GenericRepository<UsuariosInformacion>, IUsuariosInformacionRepository
    {
        public UsuariosInformacionRepository(EquipGoDbContext context) : base(context) { }
        public async Task<UsuariosInformacion> GetByIdAsync(int id)
        {
            return await _context.UsuariosInformacion
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public IQueryable<UsuariosInformacion> Query()
        {
            return _context.UsuariosInformacion.AsQueryable();
        }

        public async Task AddAsync(UsuariosInformacion entity)
        {
            await _context.UsuariosInformacion.AddAsync(entity);
        }

    }
}
