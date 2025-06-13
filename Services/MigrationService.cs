using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkshopManager.Data;


namespace WorkshopManager.Services
{
    /// <summary>
    /// Hosted service, który przy starcie aplikacji aplikuje wszystkie
    /// oczekujące migracje – żeby baza była zawsze zgodna z modelem.
    /// </summary>
    public class MigrationService : IHostedService
    {
        private readonly IServiceProvider _provider;

        public MigrationService(IServiceProvider provider) => _provider = provider;

        public async Task StartAsync(CancellationToken token)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WorkshopDbContext>();
            await db.Database.MigrateAsync(token);
        }

        public Task StopAsync(CancellationToken token) => Task.CompletedTask;
    }
}