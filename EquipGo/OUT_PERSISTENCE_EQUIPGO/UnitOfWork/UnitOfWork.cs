using Interface;
using Interface.Repositories;
using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_PERSISTENCE_EQUIPGO.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EquipGoDbContext _context;

        public IEquiposRepository Equipos { get; }

        public UnitOfWork(EquipGoDbContext context)
        {
            _context = context;
            Equipos = new EquiposRepository(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
