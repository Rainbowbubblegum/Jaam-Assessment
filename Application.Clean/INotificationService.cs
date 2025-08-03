using Domain.Clean;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Clean
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int notificationId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}