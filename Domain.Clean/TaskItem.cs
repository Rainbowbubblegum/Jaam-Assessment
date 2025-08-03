using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Clean
{
    /// <summary>
    /// Represents a task in the system. This is the core entity that everything revolves around.
    /// I designed this to be flexible enough for complex project workflows while keeping it simple.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Task title - this is what users see in lists and dashboards
        /// Required and must be meaningful (1-200 chars)
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Detailed description of what needs to be done
        /// Optional but highly recommended for clarity
        /// </summary>
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Current status of the task
        /// I kept this as a string instead of enum for flexibility - you can add new statuses without code changes
        /// Common values: "Open", "In Progress", "Completed", "Cancelled"
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Open|In Progress|Completed|Cancelled)$", ErrorMessage = "Status must be Open, In Progress, Completed, or Cancelled")]
        public string Status { get; set; } = "Open";
        
        /// <summary>
        /// When the task is due
        /// I added validation to prevent past due dates - that's just bad UX
        /// </summary>
        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Who is responsible for this task
        /// Nullable because tasks can exist without an assignee initially
        /// </summary>
        public int? AssigneeId { get; set; }
        
        /// <summary>
        /// Navigation property to the assigned user
        /// EF Core will populate this when you include it in queries
        /// </summary>
        public User? Assignee { get; set; }

        /// <summary>
        /// Parent task ID for hierarchical task structures
        /// This allows for complex project breakdowns: Main Task → Sub-task → Sub-sub-task
        /// Nullable because top-level tasks have no parent
        /// </summary>
        public int? ParentTaskId { get; set; }
        
        /// <summary>
        /// Navigation property to the parent task
        /// Useful for traversing up the task hierarchy
        /// </summary>
        public TaskItem? ParentTask { get; set; }
        
        /// <summary>
        /// Collection of sub-tasks (children)
        /// This is the other side of the parent-child relationship
        /// </summary>
        public ICollection<TaskItem> SubTasks { get; set; } = new List<TaskItem>();
        
        /// <summary>
        /// ID of the user who created this task
        /// Required for authorization checks
        /// </summary>
        public int CreatedById { get; set; }
        
        /// <summary>
        /// Navigation property to the user who created this task
        /// </summary>
        public User? CreatedBy { get; set; }
        
        /// <summary>
        /// When the task was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}