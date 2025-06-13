namespace OUT_APP_EQUIPGO.Middlewares
{
    public class EmpresaTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private const string TokenEsperado = "OUTS0URS1N62026-EQUIPGO"; //CLAVE DE SEGURIDAD

        public EmpresaTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.Request.Path.StartsWithSegments("/api/equipos/sync"))
            {
                if (!context.Request.Headers.TryGetValue("X-Empresa-Token", out var token) || token != TokenEsperado)
                {
                    Console.WriteLine($"✅ Token recibido correctamente: {token}");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido o faltante.");
                    return;
                }
            }

            await _next(context);
        }
    }

}
