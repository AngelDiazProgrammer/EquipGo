using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.Services.Usuarios
{
    public interface IUsuariosSessionService
    {
        #region CRUD Básico
        Task<List<UsuariosSession>> GetAllAsync();
        Task<UsuariosSession> GetByIdAsync(int id);
        Task<UsuariosSession> GetByDocumentoAsync(string numeroDocumento);
        Task<int> CreateAsync(UsuariosSession usuario);
        Task<bool> UpdateAsync(UsuariosSession usuario);
        Task<bool> DeleteAsync(int id);
        #endregion

        #region Métodos Específicos
        Task<UsuariosSession> GetWithSedeAsync(int usuarioId);
        Task<string> GetNombreSedeByUsuarioIdAsync(int usuarioId);
        Task<int> GetSedeByUsuarioIdAsync(int usuarioId);
        Task<UsuariosSession> AuthenticateAsync(string numeroDocumento, string contraseña);
        Task<bool> ChangePasswordAsync(int usuarioId, string nuevaContraseña);
        #endregion

        #region Métodos para Combos/Selects
        Task<List<object>> GetFormDataAsync();
        Task<List<TipoDocumento>> GetTiposDocumentoAsync();
        Task<List<Estado>> GetEstadosAsync();
        Task<List<Rol>> GetRolesAsync();
        Task<List<OUT_DOMAIN_EQUIPGO.Entities.Smart.Sedes>> GetSedesAsync();
        #endregion
    }
}