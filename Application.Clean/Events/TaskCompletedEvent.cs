using MediatR;
using System;

namespace Application.Clean.Events
{
    public class TaskCompletedEvent : INotification
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public int? AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public DateTime CompletedDate { get; set; }
    }
}