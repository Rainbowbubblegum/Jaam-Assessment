using Domain.Clean;
using Microsoft.AspNetCore.Authorization;

namespace Application.Clean
{
    public interface IAuthorizationService
    {
        Task<bool> CanReadTaskAsync(TaskItem task);
        Task<bool> CanUpdateTaskAsync(TaskItem task);
        Task<bool> CanDeleteTaskAsync(TaskItem task);
        Task<bool> CanCreateTaskAsync();
        Task<bool> CanAssignTaskAsync(TaskItem task);
        
        Task<bool> CanReadUserAsync(User user);
        Task<bool> CanUpdateUserAsync(User user);
        Task<bool> CanDeleteUserAsync(User user);
        Task<bool> CanCreateUserAsync();
        Task<bool> CanManageUsersAsync();
    }
} 