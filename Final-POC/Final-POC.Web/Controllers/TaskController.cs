using System.Security.Claims;
using Final_POC.Core.DTOs;
using Final_POC.Web.Services.TaskServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_POC.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskServices _taskServices;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskServices taskServices, ILogger<TaskController> logger)
        {
            _taskServices = taskServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ApiResponseDto<List<TaskListDto>> response;

                // Check if user is in Admin role
                if (User.IsInRole("Admin"))
                {
                    _logger.LogInformation("Admin user accessing all tasks");
                    response = await _taskServices.GetAllTasksAsync();
                }
                else
                {
                    _logger.LogInformation("Regular user accessing assigned tasks");
                    response = await _taskServices.GetUserTasksAsync();
                }

                if (response.Success)
                {
                    return View(response.Data);
                }

                ViewBag.ErrorMessage = response.Message;
                return View(new List<TaskListDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks list");
                ViewBag.ErrorMessage = "Failed to fetch tasks.";
                return View(new List<TaskListDto>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var response = await _taskServices.GetAllTasksAsync();
                if (response.Success)
                {
                    return Ok(new { success = true, data = response.Data });
                }
                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tasks");
                return StatusCode(500, new { success = false, message = "Failed to fetch tasks." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _taskServices.GetTaskByIdAsync(id);
                if (response.Success)
                {
                    // Verify if regular user has access to this task
                    if (!User.IsInRole("Admin"))
                    {
                        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId) ||
                            response.Data.AssignedToId != userId)
                        {
                            return Forbid();
                        }
                    }

                    return Ok(new { success = true, data = response.Data });
                }
                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task details for ID: {TaskId}", id);
                return StatusCode(500, new { success = false, message = "Failed to retrieve task." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { success = false, message = "Validation failed", errors });
            }

            try
            {
                var response = await _taskServices.CreateTaskAsync(createTaskDto);
                if (response.Success)
                {
                    return Ok(new { success = true, message = "Task created successfully." });
                }

                return BadRequest(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new task");
                return StatusCode(500, new { success = false, message = "An error occurred while creating the task." });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _taskServices.GetTaskByIdAsync(id);
                if (response.Success)
                {
                    // Convert TaskDetailDto to UpdateTaskDto for the edit form
                    var updateTaskDto = new UpdateTaskDto
                    {
                        Id = response.Data.Id,
                        Title = response.Data.Title,
                        Description = response.Data.Description,
                        AssignedToId = response.Data.AssignedToId
                    };

                    return Ok(new { success = true, data = updateTaskDto });
                }
                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task for edit with ID: {TaskId}", id);
                return StatusCode(500, new { success = false, message = "Failed to retrieve task." });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { success = false, message = "Validation failed", errors });
            }

            try
            {
                var response = await _taskServices.UpdateTaskAsync(updateTaskDto);
                if (response.Success)
                {
                    return Ok(new { success = true, message = "Task updated successfully." });
                }

                return BadRequest(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", updateTaskDto.Id);
                return StatusCode(500, new { success = false, message = "An error occurred while updating the task." });
            }
        }

        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateTaskStatusDto updateTaskStatusDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { success = false, message = "Validation failed", errors });
            }

            try
            {
                // For non-admin users, verify the task is assigned to them
                if (!User.IsInRole("Admin"))
                {
                    var taskResponse = await _taskServices.GetTaskByIdAsync(updateTaskStatusDto.Id);
                    if (!taskResponse.Success)
                    {
                        return NotFound(new { success = false, message = "Task not found" });
                    }

                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId) ||
                        taskResponse.Data.AssignedToId != userId)
                    {
                        return Forbid();
                    }
                }

                var response = await _taskServices.UpdateTaskStatusAsync(updateTaskStatusDto);
                if (response.Success)
                {
                    return Ok(new { success = true, message = "Task status updated successfully." });
                }

                return BadRequest(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status for ID: {TaskId}", updateTaskStatusDto.Id);
                return StatusCode(500, new { success = false, message = "An error occurred while updating the task status." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(TaskFilterDto filterDto)
        {
            try
            {
                var response = await _taskServices.GetFilteredTasksAsync(filterDto);
                if (response.Success)
                {
                    return Ok(new { success = true, data = response.Data });
                }

                return NotFound(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering tasks");
                return StatusCode(500, new { success = false, message = "An error occurred while filtering tasks." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyTasks()
        {
            try
            {
                var response = await _taskServices.GetUserTasksAsync();
                if (response.Success)
                {
                    return Ok(new { success = true, data = response.Data });
                }

                ViewBag.ErrorMessage = response.Message;
                return View(new List<TaskListDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user's tasks");
                ViewBag.ErrorMessage = "Failed to fetch your tasks.";
                return View(new List<TaskListDto>());
            }
        }
    }
}
