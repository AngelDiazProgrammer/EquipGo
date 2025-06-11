using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface ITipoDocumentoRepository
    {
        IQueryable<TipoDocumento> Query();
        // Puedes agregar más métodos si necesitas (Add, Update, Delete, etc.)
    }
}
