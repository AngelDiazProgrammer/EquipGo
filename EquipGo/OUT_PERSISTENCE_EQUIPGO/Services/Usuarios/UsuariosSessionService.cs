using Interface.Services.Usuarios;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region CRUD Básico

        public async Task<List<UsuariosSession>> GetAllAsync()
        {
            return await _context.UsuariosSession
                .Include(u => u.TipoDocumento)
                .Include(u => u.Estado)
                .Include(u => u.Rol)
                .Include(u => u.Sede)
                .OrderByDescending(u => u.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UsuariosSession> GetByIdAsync(int id)
        {
            return await _context.UsuariosSession
                .Include(u => u.TipoDocumento)
                .Include(u => u.Estado)
                .Include(u => u.Rol)
                .Include(u => u.Sede)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UsuariosSession> GetByDocumentoAsync(string numeroDocumento)
        {
            return await _context.UsuariosSession
                .Include(u => u.TipoDocumento)
                .Include(u => u.Estado)
                .Include(u => u.Rol)
                .Include(u => u.Sede)
                .FirstOrDefaultAsync(u => u.NumeroDocumento == numeroDocumento);
        }

        public async Task<int> CreateAsync(UsuariosSession usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Validar que el tipo de documento existe
            var tipoDoc = await _context.TipoDocumento
                .FirstOrDefaultAsync(t => t.Id == usuario.IdTipodocumento);
            if (tipoDoc == null)
                throw new Exception("El tipo de documento seleccionado no existe.");

            // Validar que el rol existe
            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == usuario.IdRol);
            if (rol == null)
                throw new Exception("El rol seleccionado no existe.");

            // Validar que la sede existe
            var sede = await _context.Sedes
                .FirstOrDefaultAsync(s => s.Id == usuario.IdSede);
            if (sede == null)
                throw new Exception("La sede seleccionada no existe.");

            // Validar que el estado existe
            var estado = await _context.Estados
                .FirstOrDefaultAsync(e => e.Id == usuario.IdEstado);
            if (estado == null)
                throw new Exception("El estado seleccionado no existe.");

            // Validar documento único
            var documentoExistente = await _context.UsuariosSession
                .AnyAsync(u => u.NumeroDocumento == usuario.NumeroDocumento);
            if (documentoExistente)
                throw new Exception("Ya existe un usuario con este número de documento.");

            usuario.FechaCreacion = DateTime.UtcNow;
            usuario.UltimaModificacion = DateTime.UtcNow;

            await _context.UsuariosSession.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario.Id;
        }

        public async Task<bool> UpdateAsync(UsuariosSession usuario)
        {
            if (usuario == null)
                return false;

            var usuarioExistente = await _context.UsuariosSession
                .FirstOrDefaultAsync(u => u.Id == usuario.Id);

            if (usuarioExistente == null)
                return false;

            // Validaciones similares a Create
            var tipoDoc = await _context.TipoDocumento
                .FirstOrDefaultAsync(t => t.Id == usuario.IdTipodocumento);
            if (tipoDoc == null)
                throw new Exception("El tipo de documento seleccionado no existe.");

            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == usuario.IdRol);
            if (rol == null)
                throw new Exception("El rol seleccionado no existe.");

            var sede = await _context.Sedes
                .FirstOrDefaultAsync(s => s.Id == usuario.IdSede);
            if (sede == null)
                throw new Exception("La sede seleccionada no existe.");

            var estado = await _context.Estados
                .FirstOrDefaultAsync(e => e.Id == usuario.IdEstado);
            if (estado == null)
                throw new Exception("El estado seleccionado no existe.");

            // Validar documento único (excluyendo el actual)
            var documentoExistente = await _context.UsuariosSession
                .AnyAsync(u => u.NumeroDocumento == usuario.NumeroDocumento && u.Id != usuario.Id);
            if (documentoExistente)
                throw new Exception("Ya existe otro usuario con este número de documento.");

            // Actualizar propiedades
            usuarioExistente.IdTipodocumento = usuario.IdTipodocumento;
            usuarioExistente.NumeroDocumento = usuario.NumeroDocumento;
            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Apellido = usuario.Apellido;
            usuarioExistente.Contraseña = usuario.Contraseña; // Considerar encriptación
            usuarioExistente.IdEstado = usuario.IdEstado;
            usuarioExistente.IdRol = usuario.IdRol;
            usuarioExistente.IdSede = usuario.IdSede;
            usuarioExistente.UltimaModificacion = DateTime.UtcNow;

            _context.UsuariosSession.Update(usuarioExistente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _context.UsuariosSession.FindAsync(id);
            if (usuario == null)
                return false;

            _context.UsuariosSession.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Métodos Específicos

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

        public async Task<UsuariosSession> AuthenticateAsync(string numeroDocumento, string contraseña)
        {
            return await _context.UsuariosSession
                .Include(u => u.TipoDocumento)
                .Include(u => u.Estado)
                .Include(u => u.Rol)
                .Include(u => u.Sede)
                .FirstOrDefaultAsync(u =>
                    u.NumeroDocumento == numeroDocumento &&
                    u.Contraseña == contraseña && // Considerar encriptación
                    u.IdEstado == 1); // Solo usuarios activos
        }

        public async Task<bool> ChangePasswordAsync(int usuarioId, string nuevaContraseña)
        {
            var usuario = await _context.UsuariosSession.FindAsync(usuarioId);
            if (usuario == null)
                return false;

            usuario.Contraseña = nuevaContraseña; // Considerar encriptación
            usuario.UltimaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Métodos para Combos/Selects

        public async Task<List<object>> GetFormDataAsync()
        {
            var tiposDocumento = await _context.TipoDocumento
                .Select(t => new { id = t.Id, nombreDocumento = t.NombreDocumento })
                .ToListAsync();

            var estados = await _context.Estados
                .Select(e => new { id = e.Id, nombreEstado = e.NombreEstado })
                .ToListAsync();

            var roles = await _context.Roles
                .Select(r => new { id = r.Id, nombreRol = r.NombreRol })
                .ToListAsync();

            var sedes = await _context.Sedes
                .Select(s => new { id = s.Id, nombreSede = s.NombreSede })
                .ToListAsync();

            return new List<object>
            {
                new { tiposDocumento },
                new { estados },
                new { roles },
                new { sedes }
            };
        }

        public async Task<List<TipoDocumento>> GetTiposDocumentoAsync()
        {
            return await _context.TipoDocumento.ToListAsync();
        }

        public async Task<List<Estado>> GetEstadosAsync()
        {
            return await _context.Estados.ToListAsync();
        }

        public async Task<List<Rol>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<List<OUT_DOMAIN_EQUIPGO.Entities.Smart.Sedes>> GetSedesAsync()
        {
            return await _context.Sedes.ToListAsync();
        }

        #endregion
    }
}