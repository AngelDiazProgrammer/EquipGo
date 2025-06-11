using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class TipoDocumentoRepository : ITipoDocumentoRepository
    {
        private readonly EquipGoDbContext _context;

        public TipoDocumentoRepository(EquipGoDbContext context)
        {
            _context = context;
        }

        public IQueryable<TipoDocumento> Query()
        {
            return _context.TipoDocumento.AsQueryable();
        }
    }
}
