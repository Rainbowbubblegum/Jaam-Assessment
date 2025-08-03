using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Clean;
using Application.Clean.DTOs;
using Domain.Clean;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace JaamJCSAssessment.Controllers
{
    /// <summary>
    /// Tasks API controller - handles all task-related HTTP requests
    /// 
    /// This controller demonstrates several key security concepts:
    /// - JWT authentication (all endpoints require valid tokens)
    /// - Role-based authorization (different policies for different user roles)
    /// - Resource-based authorization (users can only access their own stuff)
    /// - Clean architecture (controllers just handle HTTP, delegate to services)
    /// - Security best practices (validation, error handling, logging)
    /// 
    /// The authorization patterns I'm using:
    /// - AdminOnly: Only admins can do admin stuff (create, delete, assign tasks)
    /// - UserOnly: Any logged-in user can access their own data
    /// - TaskManagement: Both admins and users can view tasks (but with different access levels)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all task endpoints - first line of defense
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, IUserService userService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/tasks
        /// Get all tasks in the system
        /// I'm using the TaskManagement policy which allows both Admins and Users to view tasks
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "TaskManagement")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, "An error occurred while retrieving tasks");
            }
        }

        /// <summary>
        /// GET /api/tasks/{id}
        /// Get a specific task by ID
        /// Anyone with TaskManagement access can view individual tasks
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "TaskManagement")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound($"Task with ID {id} not found");

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId}", id);
                return StatusCode(500, "An error occurred while retrieving the task");
            }
        }

        /// <summary>
        /// POST /api/tasks
        /// Create a new task
        /// 
        /// This endpoint shows several security concepts:
        /// - Role-based auth: Only admins can create tasks (prevents chaos)
        /// - JWT claims extraction: Getting user ID from the token
        /// - Audit trail: Tracking who created what and when
        /// - Input validation: Making sure the data is valid
        /// - Error handling: Proper responses when things go wrong
        /// 
        /// Why only admins can create tasks: Well i want to show the use of authentication, in reality in like Dev Ops users should be able to make tasks .. 
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")] // Only admins can create tasks
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                // Extract user ID from JWT token claims
                // The token contains user info (ID, email, role) that was set during login
                // This is how we know who's making the request without storing session state
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out var currentUserId))
                {
                    return Unauthorized("User ID not found in token");
                }

                // Create the task with audit trail
                // We track who created the task for authorization and audit purposes
                // This lets us do resource-based auth (users can only modify their own tasks)
                var task = new TaskItem
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Status = createTaskDto.Status,
                    DueDate = createTaskDto.DueDate,
                    AssigneeId = createTaskDto.AssigneeId,
                    CreatedById = currentUserId, // Track who created this
                    CreatedAt = DateTime.UtcNow  // Track when this was created
                };

                var createdTask = await _taskService.CreateTaskAsync(task);
                return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, "An error occurred while creating the task");
            }
        }

        /// <summary>
        /// PUT /api/tasks/{id}
        /// Update an existing task
        /// Only Admins can update tasks - this prevents unauthorized modifications
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem task)
        {
            try
            {
                if (id != task.Id)
                    return BadRequest("Task ID mismatch");

                await _taskService.UpdateTaskAsync(task);
                return NoContent(); // 204 No Content - standard for successful updates
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                return StatusCode(500, "An error occurred while updating the task");
            }
        }

        /// <summary>
        /// DELETE /api/tasks/{id}
        /// Delete a task
        /// Only Admins can delete tasks - this prevents accidental deletions
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent(); // 204 No Content - standard for successful deletions
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                return StatusCode(500, "An error occurred while deleting the task");
            }
        }

        /// <summary>
        /// POST /api/tasks/{taskId}/assign/{userId}
        /// Assign a task to a specific user
        /// Only Admins can assign tasks - this ensures proper task management
        /// </summary>
        [HttpPost("{taskId}/assign/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AssignTask(int taskId, int userId)
        {
            try
            {
                await _taskService.AssignTaskAsync(taskId, userId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId} to user {UserId}", taskId, userId);
                return StatusCode(500, "An error occurred while assigning the task");
            }
        }

        /// <summary>
        /// GET /api/tasks/my-tasks
        /// Get tasks assigned to the current user
        /// 
        /// This is a good example of resource-based authorization:
        /// - Users can only see their own tasks
        /// - We extract the user ID from the JWT token
        /// - This prevents users from seeing other people's tasks.. just showing understanding of blocking / filtering who can do what
        /// </summary>
        [HttpGet("my-tasks")]
        [Authorize(Policy = "UserOnly")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetMyTasks()
        {
            try
            {
                // Get user ID from JWT token - this is how we know who's logged in
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("Invalid user token");
                }

                var tasks = await _taskService.GetTasksByUserIdAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user tasks");
                return StatusCode(500, "An error occurred while retrieving your tasks");
            }
        }

        /// <summary>
        /// POST /api/tasks/{parentTaskId}/subtasks
        /// Create a sub-task under a parent task
        /// 
        /// This lets us build hierarchical task structures for complex projects.
        /// Only admins can create sub-tasks to keep things organized.
        /// </summary>
        [HttpPost("{parentTaskId}/subtasks")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<TaskItem>> CreateSubTask(int parentTaskId, [FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                // Get current user ID from claims
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out var currentUserId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var subTask = new TaskItem
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Status = createTaskDto.Status,
                    DueDate = createTaskDto.DueDate,
                    AssigneeId = createTaskDto.AssigneeId,
                    CreatedById = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                var createdSubTask = await _taskService.CreateSubTaskAsync(parentTaskId, subTask);
                return CreatedAtAction(nameof(GetTask), new { id = createdSubTask.Id }, createdSubTask);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sub-task for parent {ParentTaskId}", parentTaskId);
                return StatusCode(500, "An error occurred while creating the sub-task");
            }
        }

        /// <summary>
        /// GET /api/tasks/{parentTaskId}/subtasks
        /// Get all sub-tasks of a parent task
        /// Useful for showing task hierarchies
        /// </summary>
        [HttpGet("{parentTaskId}/subtasks")]
        [Authorize(Policy = "TaskManagement")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetSubTasks(int parentTaskId)
        {
            try
            {
                var subTasks = await _taskService.GetSubTasksAsync(parentTaskId);
                return Ok(subTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sub-tasks for parent {ParentTaskId}", parentTaskId);
                return StatusCode(500, "An error occurred while retrieving sub-tasks");
            }
        }

        /// <summary>
        /// POST /api/tasks/{completedTaskId}/followup
        /// Create a follow-up task after a task is completed
        /// 
        /// This is useful for workflows where one task leads to another.
        /// Only admins can create follow-up tasks to maintain workflow control.
        /// </summary>
        [HttpPost("{completedTaskId}/followup")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<TaskItem>> CreateFollowUpTask(int completedTaskId, [FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                // Get current user ID from claims
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out var currentUserId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var followUpTask = new TaskItem
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Status = createTaskDto.Status,
                    DueDate = createTaskDto.DueDate,
                    AssigneeId = createTaskDto.AssigneeId,
                    CreatedById = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                var createdFollowUpTask = await _taskService.CreateFollowUpTaskAsync(completedTaskId, followUpTask);
                return CreatedAtAction(nameof(GetTask), new { id = createdFollowUpTask.Id }, createdFollowUpTask);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating follow-up task for completed task {CompletedTaskId}", completedTaskId);
                return StatusCode(500, "An error occurred while creating the follow-up task");
            }
        }
    }
}