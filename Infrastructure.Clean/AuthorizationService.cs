using Application.Clean;
using Domain.Clean;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Clean
{
    public class AuthorizationService : Application.Clean.IAuthorizationService
    {
        private readonly Microsoft.AspNetCore.Authorization.IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationService(
            Microsoft.AspNetCore.Authorization.IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CanReadTaskAsync(TaskItem task)
        {
            var requirement = new Application.Clean.AuthorizationHandlers.TaskAuthorizationRequirement(
                Application.Clean.AuthorizationHandlers.TaskOperation.Read);
            
            var result = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext?.User, 
                task, 
                requirement);
            
            return result.Succeeded;
        }

        public async Task<bool> CanUpdateTaskAsync(TaskItem task)
        {
            var requirement = new Application.Clean.AuthorizationHandlers.TaskAuthorizationRequirement(
                Application.Clean.AuthorizationHandlers.TaskOperation.Update);
            
            var result = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext?.User, 
                task, 
                requirement);
            
            return result.Succeeded;
        }

        public async Task<bool> CanDeleteTaskAsync(TaskItem task)
        {
            var requirement = new Application.Clean.AuthorizationHandlers.TaskAuthorizationRequirement(
                Application.Clean.AuthorizationHandlers.TaskOperation.Delete);
            
            var result = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext?.User, 
                task, 
                requirement);
            
            return result.Succeeded;
        }

        public async Task<bool> CanCreateTaskAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.IsInRole("Admin") ?? false;
        }

        public async Task<bool> CanAssignTaskAsync(TaskItem task)
        {
            var requirement = new Application.Clean.AuthorizationHandlers.TaskAuthorizationRequirement(
                Application.Clean.AuthorizationHandlers.TaskOperation.Assign);
            
            var result = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext?.User, 
                task, 
                requirement);
            
            return result.Succeeded;
        }

        public async Task<bool> CanReadUserAsync(User user)
        {
            var requirement = new Application.Clean.AuthorizationHandlers.UserAuthorizationRequirement(
                Application.Clean.AuthorizationHandlers.UserOperation.Read);
            
            var result = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext?.User, 
                user, 
                requirement);
            
            return result.Succeeded;
        }

        public async Task<bool> CanUpdateUserAsync(User user)
        {
            var requirement = new Application.Clean.AuthorizationHandlers.UserAuthorizationRequirement(
                Application.Clean.AuthorizationHandlers.UserOperation.Update);
            
            var result = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext?.User, 
                user, 
                requirement);
            
            return result.Succeeded;
        }

        public async Task<bool> CanDeleteUserAsync(User user)
        {
            var requirement = new Application.Clean.AuthorizationHandlers.UserAuthorizationRequirement(
                Application.Clean.AuthorizationHandlers.UserOperation.Delete);
            
            var result = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext?.User, 
                user, 
                requirement);
            
            return result.Succeeded;
        }

        public async Task<bool> CanCreateUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.IsInRole("Admin") ?? false;
        }

        public async Task<bool> CanManageUsersAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.IsInRole("Admin") ?? false;
        }
    }
} 