using Final_POC.Core.DTOs;

namespace Final_POC.API.Services.TaskServices
{
    public interface ITaskApiService
    {
        Task<ApiResponseDto<TaskDetailDto>> CreateTask(CreateTaskDto request);
        Task<ApiResponseDto<TaskDetailDto>> UpdateTask(UpdateTaskDto request);
        Task<ApiResponseDto<TaskDetailDto>> UpdateTaskStatus(UpdateTaskStatusDto request);
        Task<ApiResponseDto<TaskDetailDto>> GetTaskById(int id);
        Task<ApiResponseDto<List<TaskListDto>>> GetAllTasks();
        Task<ApiResponseDto<List<TaskListDto>>> GetFilteredTasks(TaskFilterDto filterDto);
    }
}
