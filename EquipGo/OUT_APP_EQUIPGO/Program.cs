using Interface;
using Interface.Services.Autenticacion;
using Interface.Services.Equipos;
using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using OUT_APP_EQUIPGO.Components;
using OUT_APP_EQUIPGO.Modules;
using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_PERSISTENCE_EQUIPGO.Services.Equipos;
using OUT_PERSISTENCE_EQUIPGO.Services.Seguridad;
using OUT_PERSISTENCE_EQUIPGO.Services.Transacciones;
using OUT_PERSISTENCE_EQUIPGO.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Configuración de conexión a la base de datos
builder.Services.AddDbContext<EquipGoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EquipGoConnection")));

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Agregar después de AddRazorComponents()
builder.Services.AddScoped<EquipGoDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<CookieService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
// Configurar la autorización
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSessionExtension();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery(); // ✅ Correcto

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // ✅ Aquí va
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


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


app.Run();
