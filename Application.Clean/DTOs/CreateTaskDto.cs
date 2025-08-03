using System;

namespace Application.Clean.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public DateTime DueDate { get; set; }
        public int? AssigneeId { get; set; }
        public int? ParentTaskId { get; set; }
    }
}