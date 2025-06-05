using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace OUT_APP_EQUIPGO.Modules
{
    public partial class CookieService
    {
        public bool CookieValida { get; set; } = false;

        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IJSRuntime _js;
        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly IConfiguration _configuration;
        public CookieService(IHttpContextAccessor IHttpContextAccessor,
            IConfiguration configuration,
            AuthenticationStateProvider authenticationStateProvider,
            IJSRuntime js)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _js = js;
            _IHttpContextAccessor = IHttpContextAccessor;
            _configuration = configuration;
        }
        public async Task ValidarSesion()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            CookieValida = authState.User.Identity.IsAuthenticated;
            if (!CookieValida)
            {
                await CerrarSesion();
            }

        }

        public async Task CerrarSesion()
        {
            await _js.InvokeVoidAsync("SignOut");
        }

    }
}
