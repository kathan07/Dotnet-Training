using Final_POC.Core.DTOs;

namespace Final_POC.Web.Services.TaskServices
{
    public interface ITaskServices
    {
        Task<ApiResponseDto<TaskDetailDto>> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<ApiResponseDto<TaskDetailDto>> UpdateTaskAsync(UpdateTaskDto updateTaskDto);
        Task<ApiResponseDto<TaskDetailDto>> GetTaskByIdAsync(int taskId);
        Task<ApiResponseDto<List<TaskListDto>>> GetAllTasksAsync();
        Task<ApiResponseDto<List<TaskListDto>>> GetFilteredTasksAsync(TaskFilterDto filterDto);
        Task<ApiResponseDto<TaskDetailDto>> UpdateTaskStatusAsync(UpdateTaskStatusDto updateTaskStatusDto);
        Task<ApiResponseDto<List<TaskListDto>>> GetUserTasksAsync();
    }
}
