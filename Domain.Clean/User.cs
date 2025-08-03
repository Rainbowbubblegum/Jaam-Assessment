using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Clean
{
    /// <summary>
    /// User entity that extends ASP.NET Core Identity
    /// I'm using Identity because it handles all the security concerns: password hashing, 
    /// account lockout, email confirmation, etc. No need to reinvent the wheel.
    /// 
    /// The int generic parameter means we're using int for the primary key instead of string
    /// This is more efficient for joins and foreign keys.
    /// </summary>
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// User's display name - what shows up in task assignments and notifications
        /// This is separate from UserName (which is typically the email) for better UX
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// User's role in the system
        /// I kept this simple with just Admin and User roles
        /// Could be extended to claims-based authorization if needed
        /// </summary>
        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be either Admin or User")]
        public string Role { get; set; } = "User";
        
        /// <summary>
        /// Navigation property to tasks assigned to this user
        /// EF Core will populate this when you include it in queries
        /// </summary>
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        
        /// <summary>
        /// Navigation property to notifications for this user
        /// Used for showing notification counts and lists
        /// </summary>
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        
        // Note: Email, UserName, Id, etc. are inherited from IdentityUser<int>
        // Email is used for login, UserName is typically the same as Email
        // Id is the primary key (int instead of string for better performance)
    }
}