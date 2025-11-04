var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://0.0.0.0:7194");

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // necesario para enviar al backend
builder.Services.AddLogging(); // Agregar logging si no está

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RegistroVisitante}/{action=Index}/{id?}");

// ✅ IMPORTANTE: Agregar mapeo para API controllers
app.MapControllers();

app.Run();