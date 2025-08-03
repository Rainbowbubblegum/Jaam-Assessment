using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain.Clean;
using System.Security.Claims;

namespace Infrastructure.Clean.AuthorizationHandlers
{
    public class DatabaseAuthorizationHandler : AuthorizationHandler<DatabaseAuthorizationRequirement>
    {
        private readonly TaskManagementDbContext _context;

        public DatabaseAuthorizationHandler(TaskManagementDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DatabaseAuthorizationRequirement requirement)
        {
            var user = context.User;
            
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return;
            }

            // Admin can do everything
            if (user.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Get current user ID
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out var currentUserId))
            {
                return;
            }

            switch (requirement.Operation)
            {
                case DatabaseOperation.ReadTasks:
                    // Users can read tasks they're assigned to or open tasks
                    var accessibleTasks = await _context.Tasks
                        .Where(t => t.AssigneeId == currentUserId || 
                                   t.Status == "Open" ||
                                   t.CreatedById == currentUserId)
                        .CountAsync();
                    
                    if (accessibleTasks > 0)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case DatabaseOperation.ReadUsers:
                    // Users can only read their own profile
                    var userExists = await _context.Users
                        .AnyAsync(u => u.Id == currentUserId);
                    
                    if (userExists)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case DatabaseOperation.WriteTasks:
                    // Users can only write to tasks they're assigned to
                    var assignedTasks = await _context.Tasks
                        .Where(t => t.AssigneeId == currentUserId)
                        .CountAsync();
                    
                    if (assignedTasks > 0)
                    {
                        context.Succeed(requirement);
                    }
                    break;

                case DatabaseOperation.WriteUsers:
                    // Users can only write to their own profile
                    var ownUser = await _context.Users
                        .AnyAsync(u => u.Id == currentUserId);
                    
                    if (ownUser)
                    {
                        context.Succeed(requirement);
                    }
                    break;
            }
        }
    }

    public class DatabaseAuthorizationRequirement : IAuthorizationRequirement
    {
        public DatabaseOperation Operation { get; }

        public DatabaseAuthorizationRequirement(DatabaseOperation operation)
        {
            Operation = operation;
        }
    }

    public enum DatabaseOperation
    {
        ReadTasks,
        ReadUsers,
        WriteTasks,
        WriteUsers
    }
} 