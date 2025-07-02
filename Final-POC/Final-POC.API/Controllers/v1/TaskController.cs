using Final_POC.API.Services.TaskServices;
using Final_POC.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_POC.API.Controllers.v1
{
    [Route("v1/api/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskApiService _taskApiService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskApiService taskApiService, ILogger<TaskController> logger)
        {
            _taskApiService = taskApiService;
            _logger = logger;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task creation");
                return BadRequest(ApiResponseDto<TaskDetailDto>.FailResponse("Invalid task creation data"));
            }

            var response = await _taskApiService.CreateTask(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task update");
                return BadRequest(ApiResponseDto<TaskDetailDto>.FailResponse("Invalid task update data"));
            }

            var response = await _taskApiService.UpdateTask(request);

            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var response = await _taskApiService.GetTaskById(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("list/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTasks()
        {
            var response = await _taskApiService.GetAllTasks();
            return Ok(response);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetFilteredTasks([FromQuery] TaskFilterDto filterDto)
        {
            if (filterDto == null)
            {
                filterDto = new TaskFilterDto();
            }

            var response = await _taskApiService.GetFilteredTasks(filterDto);
            return Ok(response);
        }


        [HttpPatch("updatestatus")]
        [Authorize]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateTaskStatusDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task status update");
                return BadRequest(ApiResponseDto<TaskDetailDto>.FailResponse("Invalid task status update data"));
            }

            var response = await _taskApiService.UpdateTaskStatus(request);

            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("list/user/tasks")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(ApiResponseDto<string>.FailResponse("User ID not found in token"));
            }

            var filterDto = new TaskFilterDto
            {
                AssignedToId = userId
            };

            var response = await _taskApiService.GetFilteredTasks(filterDto);
            return Ok(response);
        }
    }
}
