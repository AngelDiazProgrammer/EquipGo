using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.SubEstado;
using Interface.Services.SubEstado;

namespace OUT_APP_EQUIPGO.Components.Pages.SubEstados
{
    public partial class SubEstados : ComponentBase
    {
        [Inject]
        ISubEstadoService SubEstadoService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private List<SubEstadoDto> subEstados = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                subEstados = await SubEstadoService.ObtenerTodosAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener los subestados: {ex.Message}");
                subEstados = new List<SubEstadoDto>();
            }
        }

        [JSInvokable]
        public async Task<string> RefrescarListaSubEstados()
        {
            try
            {
                subEstados = (await SubEstadoService.ObtenerTodosAsync())
                             .OrderByDescending(s => s.Id)
                             .ToList();
                StateHasChanged();
                return "ok";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al refrescar subestados: {ex.Message}");
                return $"error: {ex.Message}";
            }
        }
    }
}
