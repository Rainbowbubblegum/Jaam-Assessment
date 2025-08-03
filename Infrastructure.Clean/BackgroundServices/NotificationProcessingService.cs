using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Clean.BackgroundServices
{
    public class NotificationProcessingService : BackgroundService
    {
        private readonly ILogger<NotificationProcessingService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public NotificationProcessingService(
            ILogger<NotificationProcessingService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Processing Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    
                    await ProcessPendingNotifications(scope);
                    
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing notifications");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }

            _logger.LogInformation("Notification Processing Service stopped");
        }

        private async Task ProcessPendingNotifications(IServiceScope scope)
        {
            _logger.LogInformation("Processing pending notifications...");
            
            var notificationService = scope.ServiceProvider.GetRequiredService<Application.Clean.INotificationService>();
            
            await Task.CompletedTask;
        }
    }
}