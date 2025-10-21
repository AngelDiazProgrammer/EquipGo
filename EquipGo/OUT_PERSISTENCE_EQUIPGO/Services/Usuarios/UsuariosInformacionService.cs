// En OUT_PERSISTENCE_EQUIPGO/Services/Usuarios/UsuariosInformacionService.cs

using Infrastructure.Services;
using Interface;
using Interface.Services.Active_Directory;
using Interface.Services.Usuarios;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Usuarios
{
    public class UsuariosInformacionService : IUsuariosInformacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActiveDirectoryService _activeDirectoryService;
        public UsuariosInformacionService(IUnitOfWork unitOfWork, IActiveDirectoryService activeDirectoryService)
        {
            _unitOfWork = unitOfWork;
            _activeDirectoryService = activeDirectoryService;
        }

        // Método existente (sin cambios)
        public async Task<List<UsuarioInformacionDto>> ObtenerTodosLosUsuariosInformacionAsync()
        {
            // ... tu código existente ...
            var usuarios = await _unitOfWork.UsuariosInformacion
                                           .Query()
                                           .Include(u => u.IdTipodocumentoNavigation)
                                           .Include(u => u.IdAreaNavigation)
                                           .Include(u => u.IdCampañaNavigation)
                                           .Include(u => u.Estado)
                                           .AsNoTracking()
                                           .ToListAsync();

            var lista = usuarios.Select(u => new UsuarioInformacionDto
            {
                Id = u.Id,
                TipoDocumento = u.IdTipodocumentoNavigation?.NombreDocumento ?? "",
                NumeroDocumento = long.TryParse(u.NumeroDocumento, out var numero) ? numero : 0,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                Area = u.IdAreaNavigation?.NombreArea ?? "",
                Campana = u.IdCampañaNavigation?.NombreCampaña ?? "",
                Estado = u.Estado?.NombreEstado ?? "",
                FechaCreacion = u.FechaCreacion,
                UltimaModificacion = u.UltimaModificacion
            }).ToList();

            return lista;
        }

        // ✅ NUEVAS IMPLEMENTACIONES
        public async Task<List<object>> ObtenerUsuariosCombinadosAsync()
        {
            Console.WriteLine("🔄 [ObtenerUsuariosCombinadosAsync] Iniciando combinación de usuarios...");

            var listaCombinada = new List<object>();
            var usuariosProcesados = new HashSet<string>();

            // 1. Añadir la opción para crear un nuevo usuario
            listaCombinada.Add(new
            {
                usuario = "nuevo",
                nombreCompleto = "➕ Crear nuevo usuario",
                origen = "sistema"
            });

            // 2. Obtener TODOS los usuarios locales de una sola vez
            var usuariosLocales = await _unitOfWork.UsuariosInformacion
                                           .Query()
                                           .Include(u => u.IdTipodocumentoNavigation)
                                           .Include(u => u.IdAreaNavigation)
                                           .Include(u => u.IdCampañaNavigation)
                                           .AsNoTracking()
                                           .ToListAsync();

            Console.WriteLine($"📊 Usuarios locales encontrados: {usuariosLocales.Count}");

            // 3. Crear Lookup para manejar duplicados (usa el primer usuario encontrado)
            var usuariosLocalesLookup = usuariosLocales
                .ToLookup(
                    u => $"{u.Nombres?.Trim().ToLower()}_{u.Apellidos?.Trim().ToLower()}",
                    u => u,
                    StringComparer.OrdinalIgnoreCase
                );

            // 4. Procesar usuarios locales (solo el primero de cada grupo duplicado)
            foreach (var grupo in usuariosLocalesLookup)
            {
                var usuario = grupo.First(); // Tomar el primer usuario del grupo
                var claveUsuario = grupo.Key;

                if (!usuariosProcesados.Contains(claveUsuario))
                {
                    usuariosProcesados.Add(claveUsuario);

                    listaCombinada.Add(new
                    {
                        usuario = usuario.NumeroDocumento.ToString(),
                        nombreCompleto = $"{usuario.Nombres} {usuario.Apellidos}",
                        nombres = usuario.Nombres,
                        apellidos = usuario.Apellidos,
                        numeroDocumento = usuario.NumeroDocumento.ToString(),
                        area = usuario.IdAreaNavigation?.NombreArea,
                        campana = usuario.IdCampañaNavigation?.NombreCampaña,
                        tipoDocumento = usuario.IdTipodocumentoNavigation?.NombreDocumento,
                        idTipoDocumento = usuario.IdTipodocumento,
                        idArea = usuario.IdArea,
                        idCampaña = usuario.IdCampaña,
                        origen = "local",
                        tieneDatosCompletos = true,
                        duplicados = grupo.Count() > 1 ? grupo.Count() : 0 // Información de duplicados
                    });

                    // Log de duplicados si existen
                    if (grupo.Count() > 1)
                    {
                        Console.WriteLine($"⚠️ Usuario duplicado encontrado: {claveUsuario}. Count: {grupo.Count()}");
                    }
                }
            }

            // 5. Obtener usuarios de AD y verificar contra el lookup
            var usuariosAD = await _activeDirectoryService.ObtenerUsuariosAsync();
            Console.WriteLine($"📊 Usuarios AD encontrados: {usuariosAD.Count}");

            foreach (var u in usuariosAD)
            {
                var claveUsuario = $"{u.Nombre?.Trim().ToLower()}_{u.Apellidos?.Trim().ToLower()}";

                // Solo agregar si no existe en usuarios locales
                if (!usuariosProcesados.Contains(claveUsuario))
                {
                    usuariosProcesados.Add(claveUsuario);

                    // Buscar en el lookup local (en memoria, sin consulta a BD)
                    var usuarioLocalExistente = usuariosLocalesLookup[claveUsuario].FirstOrDefault();

                    listaCombinada.Add(new
                    {
                        usuario = u.Usuario,
                        nombreCompleto = $"{u.NombreCompleto} (AD)",
                        nombres = u.Nombre,
                        apellidos = u.Apellidos,
                        // Si existe usuario local, usar sus datos, sino vacío
                        numeroDocumento = usuarioLocalExistente?.NumeroDocumento ?? "",
                        area = usuarioLocalExistente?.IdAreaNavigation?.NombreArea ?? "",
                        campana = usuarioLocalExistente?.IdCampañaNavigation?.NombreCampaña ?? "",
                        tipoDocumento = usuarioLocalExistente?.IdTipodocumentoNavigation?.NombreDocumento ?? "",
                        idTipoDocumento = usuarioLocalExistente?.IdTipodocumento,
                        idArea = usuarioLocalExistente?.IdArea,
                        idCampaña = usuarioLocalExistente?.IdCampaña,
                        origen = "ad",
                        tieneDatosCompletos = usuarioLocalExistente != null,
                        correo = u.Correo
                    });
                }
            }

            Console.WriteLine($"✅ [ObtenerUsuariosCombinadosAsync] Combinación finalizada. Total únicos: {listaCombinada.Count - 1}");
            return listaCombinada;
        }

        public async Task<UsuariosInformacion?> ConsultarUsuarioPorDocumentoAsync(string documento)
        {
            if (string.IsNullOrEmpty(documento))
                return null;

            return await _unitOfWork.UsuariosInformacion.Query()
                .FirstOrDefaultAsync(u => u.NumeroDocumento == documento);
        }

        public async Task<UsuariosInformacion?> ConsultarUsuarioPorNombreAsync(string nombres, string apellidos)
        {
            if (string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
                return null;

            return await _unitOfWork.UsuariosInformacion.Query()
                .FirstOrDefaultAsync(u =>
                    u.Nombres.Trim().ToLower() == nombres.Trim().ToLower() &&
                    u.Apellidos.Trim().ToLower() == apellidos.Trim().ToLower());
        }

        public async Task<int> CrearUsuarioAsync(UsuariosInformacion usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            await _unitOfWork.UsuariosInformacion.AddAsync(usuario);
            await _unitOfWork.CompleteAsync();
            return usuario.Id;
        }

        public async Task<bool> ActualizarUsuarioAsync(UsuariosInformacion usuario)
        {
            if (usuario == null)
                return false;

            // La entidad ya está siendo rastreada por el DbContext,
            // así que solo necesitamos llamar a CompleteAsync para guardar los cambios.
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}