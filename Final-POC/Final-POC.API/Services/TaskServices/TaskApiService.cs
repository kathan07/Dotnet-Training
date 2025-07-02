using Final_POC.Business.Services.TaskService;
using Final_POC.Core.DTOs;

namespace Final_POC.API.Services.TaskServices
{
    public class TaskApiService: ITaskApiService
    {
        private readonly ITaskBusinessService _taskBusinessService;
        private readonly ILogger<TaskApiService> _logger;
        public TaskApiService(ITaskBusinessService taskBusinessService, ILogger<TaskApiService> logger)
        {
            _taskBusinessService = taskBusinessService;
            _logger = logger;
        }

        public async Task<ApiResponseDto<TaskDetailDto>> CreateTask(CreateTaskDto request)
        {
            _logger.LogInformation("Processing task creation request with title {Title}", request.Title);

            var result = await _taskBusinessService.CreateTask(request);

            if (result == null)
            {
                _logger.LogWarning("Task creation failed for title {Title}", request.Title);
                return ApiResponseDto<TaskDetailDto>.FailResponse("Task creation failed.");
            }

            _logger.LogInformation("Task creation successful with ID {TaskId}", result.Id);
            return ApiResponseDto<TaskDetailDto>.SuccessResponse(result, "Task created successfully.");
        }

        public async Task<ApiResponseDto<TaskDetailDto>> UpdateTask(UpdateTaskDto request)
        {
            _logger.LogInformation("Processing task update request for ID {TaskId}", request.Id);

            var result = await _taskBusinessService.UpdateTask(request);

            if (result == null)
            {
                _logger.LogWarning("Task update failed for ID {TaskId}", request.Id);
                return ApiResponseDto<TaskDetailDto>.FailResponse("Task update failed.");
            }

            _logger.LogInformation("Task update successful for ID {TaskId}", result.Id);
            return ApiResponseDto<TaskDetailDto>.SuccessResponse(result, "Task updated successfully.");
        }

        public async Task<ApiResponseDto<TaskDetailDto>> UpdateTaskStatus(UpdateTaskStatusDto request)
        {
            _logger.LogInformation("Processing task status update request for ID {TaskId} to status {Status}", request.Id, request.Status);

            var result = await _taskBusinessService.UpdateTaskStatus(request);

            if (result == null)
            {
                _logger.LogWarning("Task status update failed for ID {TaskId}", request.Id);
                return ApiResponseDto<TaskDetailDto>.FailResponse("Task status update failed.");
            }

            _logger.LogInformation("Task status update successful for ID {TaskId}", result.Id);
            return ApiResponseDto<TaskDetailDto>.SuccessResponse(result, "Task status updated successfully.");
        }

        public async Task<ApiResponseDto<TaskDetailDto>> GetTaskById(int id)
        {
            _logger.LogInformation("Processing request to get task with ID {TaskId}", id);

            var result = await _taskBusinessService.GetTaskById(id);

            if (result == null)
            {
                _logger.LogWarning("Task not found with ID {TaskId}", id);
                return ApiResponseDto<TaskDetailDto>.FailResponse("Task not found.");
            }

            _logger.LogInformation("Successfully retrieved task with ID {TaskId}", id);
            return ApiResponseDto<TaskDetailDto>.SuccessResponse(result, "Task retrieved successfully.");
        }

        public async Task<ApiResponseDto<List<TaskListDto>>> GetAllTasks()
        {
            _logger.LogInformation("Processing request to get all tasks");

            var results = await _taskBusinessService.GetAllTasks();

            if (!results.Any())
            {
                _logger.LogInformation("No tasks found");
                return ApiResponseDto<List<TaskListDto>>.SuccessResponse(new List<TaskListDto>(), "No tasks found.");
            }

            _logger.LogInformation("Successfully retrieved {Count} tasks", results.Count);
            return ApiResponseDto<List<TaskListDto>>.SuccessResponse(results, "Tasks retrieved successfully.");
        }

        public async Task<ApiResponseDto<List<TaskListDto>>> GetFilteredTasks(TaskFilterDto filterDto)
        {
            _logger.LogInformation("Processing request to get filtered tasks");

            var results = await _taskBusinessService.GetFilteredTasks(filterDto);

            if (!results.Any())
            {
                _logger.LogInformation("No tasks found matching the specified filters");
                return ApiResponseDto<List<TaskListDto>>.SuccessResponse(new List<TaskListDto>(), "No tasks found matching the criteria.");
            }

            _logger.LogInformation("Successfully retrieved {Count} filtered tasks", results.Count);
            return ApiResponseDto<List<TaskListDto>>.SuccessResponse(results, "Filtered tasks retrieved successfully.");
        }

    }
}
