using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace JaamJCSAssessment.AuthorizationHandlers
{
    /// <summary>
    /// Legacy authorization filter for backward compatibility
    /// This is kept for existing controller usage but new code should use the modern AuthorizationHandler approach
    /// </summary>
    public class UserResourceAuthorizationHandler : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get the user ID from the route
            var routeData = context.RouteData;
            if (routeData.Values.TryGetValue("userId", out var userIdValue))
            {
                if (int.TryParse(userIdValue?.ToString(), out var requestedUserId))
                {
                    // Get the current user's ID from claims
                    var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                    if (currentUserIdClaim != null && int.TryParse(currentUserIdClaim.Value, out var currentUserId))
                    {
                        // Check if the user is trying to access their own data or is an admin
                        if (requestedUserId != currentUserId && !user.IsInRole("Admin"))
                        {
                            context.Result = new ForbidResult();
                            return;
                        }
                    }
                }
            }
        }
    }
} 