// Interface/Services/Usuarios/IUsuariosInformacionService.cs
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.Services.Usuarios
{
    public interface IUsuariosInformacionService
    {
        // CRUD Básico
        Task<List<UsuarioInformacionDto>> ObtenerTodosLosUsuariosInformacionAsync();
        Task<UsuarioInformacionDto?> ObtenerPorIdAsync(int id);
        Task<UsuariosInformacion?> ConsultarUsuarioPorDocumentoAsync(string documento);
        Task<UsuariosInformacion?> ConsultarUsuarioPorNombreAsync(string nombres, string apellidos);
        Task<int> CrearUsuarioAsync(UsuariosInformacion usuario);
        Task<bool> ActualizarUsuarioAsync(UsuariosInformacion usuario);
        Task<bool> ActualizarUsuarioAdminAsync(int id, UsuarioCrearDto dto);
        Task<bool> EliminarAsync(int id);

        // Carga Masiva
        //Task<ResultadoCargaMasivaDto> CargaMasivaUsuariosAsync(List<UsuarioCrearDto> usuariosDto);

        Task<ResultadoCargaMasivaDto> CargaMasivaUsuariosAsync(
        List<UsuarioCrearDto> usuariosDto,
        Dictionary<string, int> campañasExistentes);
        Task<byte[]> GenerarPlantillaCargaMasivaAsync();

        // Combos/Selects
        Task<List<TipoDocumento>> ObtenerTipoDocumentoAsync();
        Task<List<Area>> ObtenerAreasAsync();
        Task<List<Campaña>> ObtenerCampañasAsync();
        Task<List<Estado>> ObtenerEstadosAsync();

        // Usuarios Combinados
        Task<List<object>> ObtenerUsuariosCombinadosAsync();

        //filtros
        Task<IEnumerable<object>> FiltrarAsync(Dictionary<string, string> filtros);
    }
}