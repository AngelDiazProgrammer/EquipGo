using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Proveedores
{
    public interface IProveedoresService
    {
        Task<List<OUT_DOMAIN_EQUIPGO.Entities.Smart.Proveedores>> ObtenerTodasAsync();
    }
}
