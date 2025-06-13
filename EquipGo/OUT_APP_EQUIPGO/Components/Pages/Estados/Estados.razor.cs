using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Estado;
using OUT_PERSISTENCE_EQUIPGO.Services.Equipos;
using OUT_PERSISTENCE_EQUIPGO.Services.Estados;

namespace OUT_APP_EQUIPGO.Components.Pages.Estados
{
    public partial class Estados : ComponentBase
    {
        [Inject]
        Interface.Services.Estados.IEstadoService EstadoService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private List<EstadoDto> estados = new(); 

        protected override async Task OnInitializedAsync()
        {
            try
            {
                estados = await EstadoService.ObtenerTodasAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error al obtener los estados");
                estados = new List<EstadoDto>(); 
            }
        }

        [JSInvokable]
        public async Task<string> RefrescarListaEstados()
        {
            try
            {
                estados = (await EstadoService.ObtenerTodasAsync())
                            .OrderByDescending(e => e.Id)
                            .ToList();
                StateHasChanged();
                return "ok";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al refrescar estados: {ex.Message}");
                return $"error: {ex.Message}";
            }
        }
    }
}
