using Interface.Repositories;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class UsuariosSessionRepository : GenericRepository<UsuariosSession>, IUsuariosSessionRepository
    {
        public UsuariosSessionRepository(EquipGoDbContext context) : base(context) { }
    }

}
