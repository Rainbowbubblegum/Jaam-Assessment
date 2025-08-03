using Application.Clean;
using Domain.Clean;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Clean
{
    public class NotificationService : INotificationService
    {
        private readonly TaskManagementDbContext _context;

        public NotificationService(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            // Validate notification
            if (string.IsNullOrWhiteSpace(notification.Message))
                throw new ArgumentException("Notification message is required");
            
            if (notification.UserId <= 0)
                throw new ArgumentException("Valid user ID is required");

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                throw new InvalidOperationException($"Notification with ID {notificationId} not found.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}