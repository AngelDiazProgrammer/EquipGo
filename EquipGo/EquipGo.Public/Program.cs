var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://0.0.0.0:7194");
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // necesario para enviar al backend

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RegistroVisitante}/{action=Index}/{id?}");

app.Run();
