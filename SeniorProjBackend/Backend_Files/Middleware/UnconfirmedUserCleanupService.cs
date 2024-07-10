using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;

namespace SeniorProjBackend.Middleware
{
    public class UnconfirmedUserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<UnconfirmedUserCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1);

        public UnconfirmedUserCleanupService(IServiceProvider services, ILogger<UnconfirmedUserCleanupService> logger)
        {
            _services = services;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Beginning unconfirmed user cleanup");
                    await CleanupUnconfirmedUsersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during unconfirmed user cleanup");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        private async Task CleanupUnconfirmedUsersAsync()
        {
            try
            {
                using var scope = _services.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var cutoffDate = DateTime.UtcNow.AddMinutes(-2);
                var unconfirmedUsers = await userManager.Users
                    .Where(u => !u.EmailConfirmed && u.RegistrationDate < cutoffDate)
                    .ToListAsync();

                foreach (var user in unconfirmedUsers)
                {
                    try
                    {
                        var result = await userManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("Deleted unconfirmed user: {Username}, {Email}", user.UserName, user.Email);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to delete unconfirmed user: {Username}, {Email}. Errors: {Errors}",
                                user.UserName, user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting unconfirmed user: {Username}, {Email}", user.UserName, user.Email);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during unconfirmed user cleanup process");
            }
        }
    }
}