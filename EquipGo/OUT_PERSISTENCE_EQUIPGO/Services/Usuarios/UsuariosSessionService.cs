using Interface.Services.Usuarios;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Usuarios
{
    public class UsuariosSessionService : IUsuariosSessionService
    {
        private readonly EquipGoDbContext _context;

        public UsuariosSessionService(EquipGoDbContext context)
        {
            _context = context;
        }

        public async Task<UsuariosSession> GetByIdAsync(int id)
        {
            return await _context.UsuariosSession
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UsuariosSession> GetWithSedeAsync(int usuarioId)
        {
            return await _context.UsuariosSession
                .Include(u => u.Sede)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);
        }

        public async Task<string> GetNombreSedeByUsuarioIdAsync(int usuarioId)
        {
            return await _context.UsuariosSession
                .Where(u => u.Id == usuarioId)
                .Include(u => u.Sede)
                .Select(u => u.Sede.NombreSede)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetSedeByUsuarioIdAsync(int usuarioId)
        {
            return await _context.UsuariosSession
                .Where(u => u.Id == usuarioId)
                .Select(u => u.IdSede)
                .FirstOrDefaultAsync();
        }
    }

}

