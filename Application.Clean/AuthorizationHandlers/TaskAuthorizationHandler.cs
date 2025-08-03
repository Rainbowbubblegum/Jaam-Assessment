using Microsoft.AspNetCore.Authorization;
using Domain.Clean;
using System.Security.Claims;

namespace Application.Clean.AuthorizationHandlers
{
    /// <summary>
    /// Handles authorization for TaskItem resources
    /// 
    /// This is where the magic happens - we decide who can do what with tasks.
    /// I'm using ASP.NET Core's AuthorizationHandler pattern which is the modern way
    /// to do resource-based authorization (instead of the old filter approach).
    /// </summary>
    public class TaskAuthorizationHandler : AuthorizationHandler<TaskAuthorizationRequirement, TaskItem>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TaskAuthorizationRequirement requirement,
            TaskItem resource)
        {
            var user = context.User;
            
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Task.CompletedTask;
            }

            // Admins can do everything - simple rule
            if (user.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Get the current user's ID from their JWT token
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out var currentUserId))
            {
                return Task.CompletedTask;
            }

            switch (requirement.Operation)
            {
                case TaskOperation.Read:
                    // Users can read tasks they're assigned to, created by them, or any open tasks
                    if (resource.AssigneeId == currentUserId || 
                        resource.CreatedById == currentUserId ||
                        resource.Status == "Open") // Open tasks are public
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case TaskOperation.Update:
                    // Users can only update tasks they're assigned to
                    if (resource.AssigneeId == currentUserId)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case TaskOperation.Delete:
                    // Only admins can delete tasks (handled by the admin check above)
                    break;

                case TaskOperation.Assign:
                    // Only admins can assign tasks (handled by the admin check above)
                    break;

                case TaskOperation.Create:
                    // Only admins can create tasks (handled by the admin check above)
                    break;
            }

            return Task.CompletedTask;
        }
    }

    public class TaskAuthorizationRequirement : IAuthorizationRequirement
    {
        public TaskOperation Operation { get; }

        public TaskAuthorizationRequirement(TaskOperation operation)
        {
            Operation = operation;
        }
    }

    public enum TaskOperation
    {
        Read,
        Create,
        Update,
        Delete,
        Assign
    }
} 