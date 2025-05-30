using Interface.Repositories;
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
    }
}
