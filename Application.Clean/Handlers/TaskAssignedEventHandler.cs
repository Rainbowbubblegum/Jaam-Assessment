using Application.Clean.Events;
using Domain.Clean;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Clean.Handlers
{
    public class TaskAssignedEventHandler : INotificationHandler<TaskAssignedEvent>
    {
        private readonly INotificationService _notificationService;

        public TaskAssignedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(TaskAssignedEvent notification, CancellationToken cancellationToken)
        {
            var userNotification = new Notification
            {
                UserId = notification.UserId,
                Message = $"You have been assigned to task: {notification.TaskTitle}",
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            };

            await _notificationService.CreateNotificationAsync(userNotification);
        }
    }
}