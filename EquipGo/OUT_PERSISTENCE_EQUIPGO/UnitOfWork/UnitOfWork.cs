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
        public UnitOfWork(EquipGoDbContext context)
        {
            _context = context;
            Equipos = new EquiposRepository(_context);
            UsuariosInformacion = new UsuariosInformacionRepository(_context);
            Transacciones = new TransaccionesRepository(_context);
            Area = new AreaRepository(_context);
            Campaña = new CampañaRepository(_context);
        }

        public IEquiposRepository Equipos { get; }
        public IUsuariosInformacionRepository UsuariosInformacion { get; }
        public ITransaccionesRepository Transacciones { get; }

        public IAreaRepository Area { get; private set; }
        public ICampañaRepository Campaña { get; private set; }

       

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
