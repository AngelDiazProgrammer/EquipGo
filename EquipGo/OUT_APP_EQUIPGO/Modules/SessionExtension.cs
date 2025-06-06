namespace OUT_APP_EQUIPGO.Modules
{
    public static class SessionExtension
    {
        public static IServiceCollection AddSessionExtension(this IServiceCollection services)
        {
            services.AddSession(opt =>
            {
                opt.Cookie.Name = "UserCookie";
                opt.Cookie.SecurePolicy = CookieSecurePolicy.Always; //Investigar
                opt.Cookie.HttpOnly = true;

                opt.IdleTimeout = TimeSpan.FromMinutes(20);
            });
            return services;
        }
    }
}
