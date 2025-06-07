// ====================
// 📌 Usings ordenados
// ====================
using Interface;
using Interface.Services;
using Interface.Services.Autenticacion;
using Interface.Services.Equipos;
using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using OUT_APP_EQUIPGO.Components;
using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_PERSISTENCE_EQUIPGO.Services.Equipos;
using OUT_PERSISTENCE_EQUIPGO.Services.Seguridad;
using OUT_PERSISTENCE_EQUIPGO.Services.Transacciones;
using OUT_PERSISTENCE_EQUIPGO.UnitOfWork;

// ====================
// 📌 Builder
// ====================
var builder = WebApplication.CreateBuilder(args);

// ====================
// 📌 Configuración de Base de Datos
// ====================
builder.Services.AddDbContext<EquipGoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EquipGoConnection")));

// ====================
// 📌 Registro de Servicios
// ====================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<EquipGoDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ====================
// 📌 Autenticación (Cookies)
// ====================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login"; // ruta de login
        options.AccessDeniedPath = "/accessdenied"; // página de acceso denegado
        options.LogoutPath = "/api/auth/logout"; // ruta de logout
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// ====================
// 📌 HTTP Client para JS Interop (llamadas fetch)
// ====================
builder.Services.AddHttpClient();
builder.Services.AddControllers();
// ====================
// 📌 Autorización
// ====================
builder.Services.AddAuthorizationCore(); // para Blazor
builder.Services.AddAuthorization();     // para el servidor
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; }); // 🔥 habilita errores detallados
// ====================
// 📌 Build App
// ====================
var app = builder.Build();

// ====================
// 📌 Configuración del Pipeline
// ====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers(); // 📌 Habilita endpoints API
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// ====================
// 📌 Verificar Conexión Base de Datos
// ====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EquipGoDbContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✅ Conexión a la base de datos exitosa.");
        }
        else
        {
            Console.WriteLine("❌ Error al conectar con la base de datos.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

// ====================
// 📌 Ejecutar la Aplicación
// ====================
app.Run();
