using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.Services.Equipos
{
    public interface IEquipoService
    {
        Task<List<EquipoDto>> ObtenerTodosLosEquiposAsync();
        Task<EquipoEscaneadoDto?> ConsultarPorCodigoBarrasAsync(string codigoBarras);
        Task<bool> CrearEquipoAdminAsync(CrearEquipoDto equipoDto);

        Task<EquipoDto?> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarEquipoAdminAsync(int id, CrearEquipoDto dto);
        Task<bool> EliminarAsync(int id);

        //Consultar visitante
        Task<RegistroVisitanteDto?> ConsultarVisitantePorDocumentoAsync(string numeroDocumento);



        //RegistrarEquipoNoCorporativo
        Task<List<TipoDocumento>> ObtenerTipoDocumentoAsync();
        Task<UsuariosInformacion?> ConsultarUsuarioPorDocumentoAsync(string documento);
        Task<int> CrearUsuarioAsync(UsuariosInformacion usuario);
        Task<List<Area>> ObtenerAreasAsync();
        Task<List<Campaña>> ObtenerCampañasAsync();
        Task<List<EquiposPersonal>> ObtenerEquiposPersonalesAsync();
        Task<EquiposPersonal?> ObtenerEquipoPersonalPorIdAsync(int id);
        Task<bool> CrearEquipoAsync(OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos equipo);
        Task<string> SincronizarEquipoAsync(EquipoSyncRequestDto dto);
    }
}