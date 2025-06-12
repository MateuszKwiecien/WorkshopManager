using Microsoft.AspNetCore.Identity;

namespace WorkshopManager.Services
{
    public class IdentitySeeder : IHostedService
    {
        private readonly IServiceProvider _provider;
        private static readonly string[] Roles =
            { "Admin", "Mechanic", "Recepcjonista" };

        public IdentitySeeder(IServiceProvider provider) => _provider = provider;

        public async Task StartAsync(CancellationToken token)
        {
            using var scope = _provider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        public Task StopAsync(CancellationToken _) => Task.CompletedTask;
    }
}