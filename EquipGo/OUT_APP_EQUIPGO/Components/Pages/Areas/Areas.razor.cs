using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Areas;
using OUT_PERSISTENCE_EQUIPGO.Services.Areas;


namespace OUT_APP_EQUIPGO.Components.Pages.Areas
{
    public partial class Areas : ComponentBase
    {
        [Inject]
        Interface.Services.Areas.IAreasService AreasService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private List<AreaDto> areas = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                areas = await AreasService.ObtenerTodasAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los estados");
                areas = new List<AreaDto>();
            }
        }

        [JSInvokable]
        public async Task<string> RefrescarListaAreas()
        {
            try
            {
                areas = (await AreasService.ObtenerTodasAsync())
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
