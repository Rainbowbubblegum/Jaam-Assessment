using Application.Clean.Events;
using Domain.Clean;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Clean.Handlers
{
    public class TaskCompletedEventHandler : INotificationHandler<TaskCompletedEvent>
    {
        private readonly INotificationService _notificationService;

        public TaskCompletedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(TaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.AssigneeId.HasValue)
            {
                var userNotification = new Notification
                {
                    UserId = notification.AssigneeId.Value,
                    Message = $"Congratulations! You have completed task: {notification.TaskTitle}",
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow
                };

                await _notificationService.CreateNotificationAsync(userNotification);
            }
        }
    }
}