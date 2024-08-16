using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;

namespace SeniorProjBackend.Middleware
{
    public class UnconfirmedUserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<UnconfirmedUserCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);

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
                    _logger.LogInformation("\n\n\n\nBeginning unconfirmed user cleanup\n\n\n\n");
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

                var cutoffDate = DateTime.UtcNow.AddHours(-12);
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
                            _logger.LogInformation("\n\n\n\nDeleted unconfirmed user: {user.UserName}, {user.Email}\n\n\n\n", user.UserName, user.Email);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to delete unconfirmed user: {Username}, {Email}. Errors: {Errors}",
                                user.UserName, user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "\n\n\n\nError deleting unconfirmed user: {user.UserName}, {user.Email}\n\n\n\n", user.UserName, user.Email);
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