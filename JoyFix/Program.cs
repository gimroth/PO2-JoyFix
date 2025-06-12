using JoyFix.Components;
using JoyFix.Data;
using JoyFix.Seeders;
using JoyFix.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja bazy danych
builder.Services.AddDbContext<ContextDB>((serviceProvider, options) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connStr);
});

builder.Services.AddSingleton<DynamicDbContextFactory>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<TechnicianService>();
builder.Services.AddScoped<SpecializationService>();
builder.Services.AddScoped<RepairService>();
builder.Services.AddScoped<RepairRequestService>();
builder.Services.AddScoped<ProtectedSessionStorage>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//migracja i seedery
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContextDB>();
    db.Database.Migrate();
    DatabaseSeeder.Seed(db);
}

app.Run();
