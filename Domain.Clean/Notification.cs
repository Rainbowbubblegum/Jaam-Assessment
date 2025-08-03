using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Clean
{
    /// <summary>
    /// Notification entity for keeping users informed about task changes
    /// This is part of the event-driven architecture - when tasks are assigned, 
    /// completed, or updated, notifications are created automatically
    /// </summary>
    public class Notification
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Which user this notification is for
        /// Required because notifications are always user-specific
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }
        
        /// <summary>
        /// Navigation property to the user who owns this notification
        /// EF Core will populate this when you include it in queries
        /// </summary>
        public User User { get; set; } = null!;
        
        /// <summary>
        /// The actual notification message
        /// Examples: "Task 'Design Database' has been assigned to you"
        ///           "Task 'Implement Authentication' has been completed"
        /// </summary>
        [Required(ErrorMessage = "Message is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 500 characters")]
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the user has read this notification
        /// Used for showing unread counts and filtering notifications
        /// </summary>
        public bool IsRead { get; set; } = false;
        
        /// <summary>
        /// When the notification was created
        /// Used for sorting and showing notification age
        /// </summary>
        [Required(ErrorMessage = "Created date is required")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Optional link to the related task
        /// Useful for "click to view task" functionality
        /// </summary>
        public int? RelatedTaskId { get; set; }
        
        /// <summary>
        /// Type of notification for filtering and styling
        /// Examples: "TaskAssigned", "TaskCompleted", "TaskUpdated"
        /// </summary>
        [StringLength(50, ErrorMessage = "Notification type cannot exceed 50 characters")]
        public string? NotificationType { get; set; }
    }
}