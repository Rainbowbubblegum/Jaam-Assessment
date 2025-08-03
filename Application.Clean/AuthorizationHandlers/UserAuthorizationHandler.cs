using Microsoft.AspNetCore.Authorization;
using Domain.Clean;
using System.Security.Claims;

namespace Application.Clean.AuthorizationHandlers
{
    public class UserAuthorizationHandler : AuthorizationHandler<UserAuthorizationRequirement, User>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserAuthorizationRequirement requirement,
            User resource)
        {
            var user = context.User;
            
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Task.CompletedTask;
            }

            // Admin can do everything
            if (user.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Get current user ID
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out var currentUserId))
            {
                return Task.CompletedTask;
            }

            switch (requirement.Operation)
            {
                case UserOperation.Read:
                    // Users can read their own profile
                    if (resource.Id == currentUserId)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case UserOperation.Update:
                    // Users can update their own profile
                    if (resource.Id == currentUserId)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case UserOperation.Delete:
                    // Only admins can delete users (handled above)
                    break;

                case UserOperation.Create:
                    // Only admins can create users (handled above)
                    break;

                case UserOperation.Manage:
                    // Only admins can manage users (handled above)
                    break;
            }

            return Task.CompletedTask;
        }
    }

    public class UserAuthorizationRequirement : IAuthorizationRequirement
    {
        public UserOperation Operation { get; }

        public UserAuthorizationRequirement(UserOperation operation)
        {
            Operation = operation;
        }
    }

    public enum UserOperation
    {
        Read,
        Create,
        Update,
        Delete,
        Manage
    }
} 