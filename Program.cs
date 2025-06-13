using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mapster;                              // ← Mapster DI
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Models;
using WorkshopManager.Services;
using WorkshopManager.Interfaces;
using WorkshopManager.Repositories;

using NLog.Web;   // ⬅️ na samej górze

var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
try
{
    // ⬇️ standardowy builder
    var builder = WebApplication.CreateBuilder(args);

    // usuń domyślnych providerów i dodaj NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    
    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// ░░░ Usługi ░░░─────────────────────────────────────────────────────────────

// DbContext + SQL Server
    builder.Services.AddDbContext<WorkshopDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (użytkownicy + role + tokeny)
    builder.Services
        .AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength          = 6;
        })
        .AddEntityFrameworkStores<WorkshopDbContext>()
        .AddDefaultTokenProviders();

// Mapster – automatyczna rejestracja profili IRegister
    builder.Services.AddMapster();

// Generyczne repozytorium + serwisy biznesowe
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IVehicleService,  VehicleService>();
    builder.Services.AddScoped<IOrderService,    OrderService>();

// MVC + Razor Pages (Identity UI)
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

// Hosted services: automigracja + seed ról
    builder.Services.AddHostedService<MigrationService>();
    builder.Services.AddHostedService<IdentitySeeder>();

    builder.Services.AddScoped<ITaskService,      TaskService>();
    builder.Services.AddScoped<IUsedPartService,  UsedPartService>();
    builder.Services.AddScoped<IPartService, PartService>();
    builder.Services.AddScoped<ITaskService, TaskService>();
    builder.Services.AddScoped<IReportService, ReportService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

// trasy MVC
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

// trasy Razor Pages (Identity UI)
    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    // logujemy krytyczną awarię aplikacji
    logger.Fatal(ex, "Program terminated unexpectedly");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}