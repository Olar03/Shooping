using Microsoft.EntityFrameworkCore;
using Shooping.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//Agregar al Builder otros servicio (BD )
builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddTransient<LoadDb>(); //Hace la inyección una sola vez 
//builder.Services.AddScoped<LoadDb>(); //Hace la inyección cada vez que se necesita 
//builder.Services.AddSingleton<LoadDb>(); // lo inye ta una vez y lo deja en memoria
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();// Permite hacer cambios en caliente

var app = builder.Build();
LoadData();

void LoadData()
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopedFactory.CreateScope())
    {
        LoadDb? service = scope.ServiceProvider.GetService<LoadDb>();
        service.LoadAsync().Wait();
    }

}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
