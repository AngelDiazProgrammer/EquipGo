using Interface;
using Interface.Repositories;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
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
            UsuariosSession = new UsuariosSessionRepository(_context);
        }

        public IEquiposRepository Equipos { get; }
        public IUsuariosInformacionRepository UsuariosInformacion { get; }
        public ITransaccionesRepository Transacciones { get; }
        public IAreaRepository Area { get; }
        public ICampañaRepository Campaña { get; }

        public IUsuariosSessionRepository  UsuariosSession { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
