﻿using Interface.Repositories;
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
    public class ProveedoresRepository : GenericRepository<Proveedores>, IProveedoresRepository
    {
        public ProveedoresRepository(EquipGoDbContext context) : base(context) { }

        public async Task<Proveedores?> GetByIdAsync(int id)
        {
            return await _context.Proveedores.FindAsync(id);
        }
    }
}
