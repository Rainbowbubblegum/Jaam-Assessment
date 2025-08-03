using MediatR;

namespace Application.Clean.Events
{
    public class TaskAssignedEvent : INotification
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}