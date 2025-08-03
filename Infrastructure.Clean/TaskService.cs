using Application.Clean;
using Domain.Clean;
using Infrastructure.Clean;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Clean.Events;

namespace Infrastructure.Clean
{
    /// <summary>
    /// Task service implementation - this is where all the business logic lives
    /// I separated this from the controllers to keep them thin and focused on HTTP concerns
    /// This also makes the business logic testable without needing to spin up a web server
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly TaskManagementDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public TaskService(TaskManagementDbContext context, UserManager<User> userManager, IMediator mediator)
        {
            _context = context;
            _userManager = userManager;
            _mediator = mediator;
        }

        /// <summary>
        /// Get all tasks with their assignees loaded
        /// I'm using Include() to avoid N+1 query problems
        /// This loads all the related data in one query instead of lazy loading
        /// </summary>
        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.ParentTask)
                .Include(t => t.SubTasks)
                .ToListAsync();
        }

        /// <summary>
        /// Get a specific task by ID
        /// Includes all related data for a complete view
        /// </summary>
        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.ParentTask)
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (task == null)
                throw new InvalidOperationException($"Task with ID {id} not found.");
            
            return task;
        }

        /// <summary>
        /// Create a new task with validation
        /// I'm doing validation here instead of just relying on API validation
        /// This ensures the business rules are enforced regardless of how the data gets in
        /// </summary>
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            // Validate the task data
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Task title is required");
            
            if (task.DueDate <= DateTime.UtcNow)
                throw new ArgumentException("Due date must be in the future");

            // Check if assignee exists (if specified)
            if (task.AssigneeId.HasValue)
            {
                var assignee = await _userManager.FindByIdAsync(task.AssigneeId.Value.ToString());
                if (assignee == null)
                    throw new InvalidOperationException($"User with ID {task.AssigneeId} not found.");
            }

            // Check if parent task exists (if specified)
            if (task.ParentTaskId.HasValue)
            {
                var parentTask = await _context.Tasks.FindAsync(task.ParentTaskId.Value);
                if (parentTask == null)
                    throw new InvalidOperationException($"Parent task with ID {task.ParentTaskId} not found.");
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            // Publish event for task creation (could trigger notifications, etc.)
            await _mediator.Publish(new TaskAssignedEvent { TaskId = task.Id, UserId = task.AssigneeId ?? 0 });
            
            return await GetTaskByIdAsync(task.Id);
        }

        /// <summary>
        /// Update an existing task
        /// I'm tracking the old status to detect status changes for notifications
        /// </summary>
        public async Task UpdateTaskAsync(TaskItem task)
        {
            var existingTask = await _context.Tasks.FindAsync(task.Id);
            if (existingTask == null)
                throw new InvalidOperationException($"Task with ID {task.Id} not found.");

            // Validate assignee if specified
            if (task.AssigneeId.HasValue)
            {
                var assignee = await _userManager.FindByIdAsync(task.AssigneeId.Value.ToString());
                if (assignee == null)
                    throw new InvalidOperationException($"User with ID {task.AssigneeId} not found.");
            }

            // Track status change for notifications
            var oldStatus = existingTask.Status;
            var oldAssigneeId = existingTask.AssigneeId;
            
            // Update the task properties
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.DueDate = task.DueDate;
            existingTask.AssigneeId = task.AssigneeId;
            existingTask.ParentTaskId = task.ParentTaskId;

            await _context.SaveChangesAsync();

            // Publish events for status changes and assignments
            if (oldStatus != task.Status && task.Status == "Completed")
            {
                await _mediator.Publish(new TaskCompletedEvent { TaskId = task.Id });
            }
            
            if (oldAssigneeId != task.AssigneeId && task.AssigneeId.HasValue)
            {
                await _mediator.Publish(new TaskAssignedEvent { TaskId = task.Id, UserId = task.AssigneeId.Value });
            }
        }

        /// <summary>
        /// Delete a task
        /// I'm doing a soft delete check here - you might want to implement actual soft deletes
        /// </summary>
        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                throw new InvalidOperationException($"Task with ID {id} not found.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Assign a task to a user
        /// This is a common operation that deserves its own method
        /// </summary>
        public async Task AssignTaskAsync(int taskId, int userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                throw new InvalidOperationException($"Task with ID {taskId} not found.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found.");

            var oldAssigneeId = task.AssigneeId;
            task.AssigneeId = userId;
            await _context.SaveChangesAsync();

            // Publish assignment event for notifications
            if (oldAssigneeId != userId)
            {
                await _mediator.Publish(new TaskAssignedEvent { TaskId = taskId, UserId = userId });
            }
        }

        /// <summary>
        /// Get tasks assigned to a specific user
        /// Useful for "my tasks" views
        /// </summary>
        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.ParentTask)
                .Where(t => t.AssigneeId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Create a sub-task under a parent task
        /// This enables hierarchical task structures for complex projects
        /// </summary>
        public async Task<TaskItem> CreateSubTaskAsync(int parentTaskId, TaskItem subTask)
        {
            var parentTask = await _context.Tasks.FindAsync(parentTaskId);
            if (parentTask == null)
                throw new InvalidOperationException($"Parent task with ID {parentTaskId} not found.");

            // Prevent circular references - a task can't be its own parent
            if (parentTaskId == subTask.Id)
                throw new InvalidOperationException("A task cannot be its own parent");

            subTask.ParentTaskId = parentTaskId;
            return await CreateTaskAsync(subTask);
        }

        /// <summary>
        /// Get all sub-tasks of a parent task
        /// Useful for showing task hierarchies
        /// </summary>
        public async Task<IEnumerable<TaskItem>> GetSubTasksAsync(int parentTaskId)
        {
            return await _context.Tasks
                .Include(t => t.Assignee)
                .Where(t => t.ParentTaskId == parentTaskId)
                .ToListAsync();
        }

        /// <summary>
        /// Create a follow-up task after a task is completed
        /// This is useful for workflows where one task leads to another
        /// </summary>
        public async Task<TaskItem> CreateFollowUpTaskAsync(int completedTaskId, TaskItem followUpTask)
        {
            var completedTask = await _context.Tasks.FindAsync(completedTaskId);
            if (completedTask == null)
                throw new InvalidOperationException($"Completed task with ID {completedTaskId} not found.");

            // Only allow follow-ups for completed tasks
            if (completedTask.Status.ToLower() != "completed")
                throw new InvalidOperationException("Follow-up tasks can only be created for completed tasks.");

            // Inherit assignee from completed task if no assignee specified
            if (!followUpTask.AssigneeId.HasValue && completedTask.AssigneeId.HasValue)
            {
                followUpTask.AssigneeId = completedTask.AssigneeId;
            }

            var createdTask = await CreateTaskAsync(followUpTask);
            
            // Publish event for follow-up task creation
            await _mediator.Publish(new TaskAssignedEvent { TaskId = createdTask.Id, UserId = createdTask.AssigneeId ?? 0 });
            
            return createdTask;
        }
    }
}