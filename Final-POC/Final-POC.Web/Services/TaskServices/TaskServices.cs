using Final_POC.Core.DTOs;
using Final_POC.Web.Services.ApiServices;

namespace Final_POC.Web.Services.TaskServices
{
    public class TaskServices: ITaskServices
    {
        private readonly IApiServices _apiServices;
        private readonly ILogger<TaskServices> _logger;

        public TaskServices(IApiServices apiServices, ILogger<TaskServices> logger)
        {
            _apiServices = apiServices;
            _logger = logger;
        }

        public async Task<ApiResponseDto<TaskDetailDto>> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            try
            {
                _logger.LogInformation("Attempting to create new task");
                return await _apiServices.PostAsync<ApiResponseDto<TaskDetailDto>>("task/create", createTaskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during task creation");
                return ApiResponseDto<TaskDetailDto>.FailResponse("An error occurred during task creation");
            }
        }

        public async Task<ApiResponseDto<TaskDetailDto>> UpdateTaskAsync(UpdateTaskDto updateTaskDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update task with ID: {TaskId}", updateTaskDto.Id);
                return await _apiServices.PutAsync<ApiResponseDto<TaskDetailDto>>("task/update", updateTaskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", updateTaskDto.Id);
                return ApiResponseDto<TaskDetailDto>.FailResponse("An error occurred while updating task");
            }
        }

        public async Task<ApiResponseDto<TaskDetailDto>> GetTaskByIdAsync(int taskId)
        {
            try
            {
                _logger.LogInformation("Retrieving task with ID: {TaskId}", taskId);
                return await _apiServices.GetAsync<ApiResponseDto<TaskDetailDto>>($"task/{taskId}", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task with ID: {TaskId}", taskId);
                return ApiResponseDto<TaskDetailDto>.FailResponse("An error occurred while retrieving task");
            }
        }

        public async Task<ApiResponseDto<List<TaskListDto>>> GetAllTasksAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all tasks");
                return await _apiServices.GetAsync<ApiResponseDto<List<TaskListDto>>>("task/list/all", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tasks");
                return ApiResponseDto<List<TaskListDto>>.FailResponse("An error occurred while retrieving tasks");
            }
        }

        public async Task<ApiResponseDto<TaskDetailDto>> UpdateTaskStatusAsync(UpdateTaskStatusDto updateTaskStatusDto)
        {
            try
            {
                _logger.LogInformation("Updating status for task with ID: {TaskId}", updateTaskStatusDto.Id);
                return await _apiServices.PatchAsync<ApiResponseDto<TaskDetailDto>>("task/updatestatus", updateTaskStatusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for task with ID: {TaskId}", updateTaskStatusDto.Id);
                return ApiResponseDto<TaskDetailDto>.FailResponse("An error occurred while updating task status");
            }
        }

        public async Task<ApiResponseDto<List<TaskListDto>>> GetUserTasksAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving tasks for current user");
                return await _apiServices.GetAsync<ApiResponseDto<List<TaskListDto>>>("task/list/user/tasks", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for current user");
                return ApiResponseDto<List<TaskListDto>>.FailResponse("An error occurred while retrieving user tasks");
            }
        }

        public async Task<ApiResponseDto<List<TaskListDto>>> GetFilteredTasksAsync(TaskFilterDto filterDto)
        {
            try
            {
                _logger.LogInformation("Filtering tasks with provided criteria");

                var queryParams = new Dictionary<string, string>();

                // Add filter parameters if they have values
                if (!string.IsNullOrEmpty(filterDto.Title))
                    queryParams.Add("Title", filterDto.Title);

                if (!string.IsNullOrEmpty(filterDto.Status))
                    queryParams.Add("Status", filterDto.Status);

                if (filterDto.AssignedToId.HasValue)
                    queryParams.Add("AssignedToId", filterDto.AssignedToId.Value.ToString());

                if (filterDto.CreatedById.HasValue)
                    queryParams.Add("CreatedById", filterDto.CreatedById.Value.ToString());

                if (filterDto.CreatedAfter.HasValue)
                    queryParams.Add("CreatedAfter", filterDto.CreatedAfter.Value.ToString("o"));

                if (filterDto.CreatedBefore.HasValue)
                    queryParams.Add("CreatedBefore", filterDto.CreatedBefore.Value.ToString("o"));

                // Add sorting parameters
                queryParams.Add("SortBy", filterDto.SortBy);
                queryParams.Add("SortDescending", filterDto.SortDescending.ToString().ToLower());

                return await _apiServices.GetAsync<ApiResponseDto<List<TaskListDto>>>("task/filter", queryParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering tasks");
                return ApiResponseDto<List<TaskListDto>>.FailResponse("An error occurred while filtering tasks");
            }
        }
    }
}
