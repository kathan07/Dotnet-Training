using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Core.DTOs;

namespace Final_POC.Business.Services.TaskService
{
    public interface ITaskBusinessService
    {
        Task<TaskDetailDto?> CreateTask(CreateTaskDto createTaskDto);
        Task<TaskDetailDto?> UpdateTask(UpdateTaskDto updateTaskDto);
        Task<TaskDetailDto?> UpdateTaskStatus(UpdateTaskStatusDto updateTaskStatusDto);
        Task<List<TaskListDto>> GetAllTasks();
        Task<TaskDetailDto?> GetTaskById(int taskId);
        //Task<List<TaskListDto>?> GetTasksAssignedToUser(int userId);
        //Task<List<TaskListDto>?> GetTasksCreatedByUser(int userId);
        //Task<List<TaskListDto>?> GetTasksByStatus(string status);
        Task<List<TaskListDto>> GetFilteredTasks(TaskFilterDto filterDto);
    }
}
