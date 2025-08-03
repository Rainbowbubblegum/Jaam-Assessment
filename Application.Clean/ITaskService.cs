using Domain.Clean;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Clean
{
    public interface ITaskService
    {
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int id);
        Task AssignTaskAsync(int taskId, int userId);
        Task<TaskItem> CreateSubTaskAsync(int parentTaskId, TaskItem subTask);
        Task<IEnumerable<TaskItem>> GetSubTasksAsync(int parentTaskId);
        Task<TaskItem> CreateFollowUpTaskAsync(int completedTaskId, TaskItem followUpTask);
    }
}