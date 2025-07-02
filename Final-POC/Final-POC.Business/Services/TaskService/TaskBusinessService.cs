using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Final_POC.Core.DTOs;
using Final_POC.Data.Models;
using Final_POC.Data.Repositories.TaskRepository;
using Microsoft.Extensions.Logging;

namespace Final_POC.Business.Services.TaskService
{
    public class TaskBusinessService: ITaskBusinessService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskBusinessService> _logger;

        public TaskBusinessService(ITaskRepository taskRepository, IMapper mapper, ILogger<TaskBusinessService> logger)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TaskDetailDto?> CreateTask(CreateTaskDto createTaskDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createTaskDto.Title) || createTaskDto.CreatedById <= 0)
                {
                    _logger.LogWarning("Invalid CreateTaskDto received");
                    return null;
                }

                var task = _mapper.Map<Tasks>(createTaskDto);

                if (!string.IsNullOrWhiteSpace(createTaskDto.Description))
                {
                    task.TaskDetail = new TaskDetail
                    {
                        Description = createTaskDto.Description
                    };
                }

                var createdTask = await _taskRepository.CreateTask(task);
                if (createdTask == null)
                {
                    _logger.LogError("Failed to create task for user ID: {CreatedById}", createTaskDto.CreatedById);
                    return null;
                }

                return _mapper.Map<TaskDetailDto>(createdTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a task");
                return null;
            }
        }

        public async Task<TaskDetailDto?> UpdateTask(UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (updateTaskDto.Id <= 0 || string.IsNullOrWhiteSpace(updateTaskDto.Title))
                {
                    _logger.LogWarning("Invalid UpdateTaskDto received");
                    return null;
                }

                var existingTask = await _taskRepository.GetTask(t => t.Id == updateTaskDto.Id);
                if (existingTask == null)
                {
                    _logger.LogWarning("Task not found for update, ID: {TaskId}", updateTaskDto.Id);
                    return null;
                }

                _mapper.Map(updateTaskDto, existingTask);

                if (existingTask.TaskDetail == null && !string.IsNullOrWhiteSpace(updateTaskDto.Description))
                {
                    existingTask.TaskDetail = new TaskDetail
                    {
                        Description = updateTaskDto.Description,
                        TaskId = existingTask.Id
                    };
                }
                else if (existingTask.TaskDetail != null)
                {
                    existingTask.TaskDetail.Description = updateTaskDto.Description;
                }

                var updatedTask = await _taskRepository.UpdateTask(existingTask);
                return updatedTask != null ? _mapper.Map<TaskDetailDto>(updatedTask) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task ID: {TaskId}", updateTaskDto.Id);
                return null;
            }
        }

        public async Task<TaskDetailDto?> UpdateTaskStatus(UpdateTaskStatusDto updateTaskStatusDto)
        {
            try
            {
                if (updateTaskStatusDto.Id <= 0 || !IsValidStatus(updateTaskStatusDto.Status))
                {
                    _logger.LogWarning("Invalid UpdateTaskStatusDto received");
                    return null;
                }

                var existingTask = await _taskRepository.GetTask(t => t.Id == updateTaskStatusDto.Id);
                if (existingTask == null)
                {
                    _logger.LogWarning("Task not found for status update, ID: {TaskId}", updateTaskStatusDto.Id);
                    return null;
                }

                existingTask.Status = updateTaskStatusDto.Status;
                var updatedTask = await _taskRepository.UpdateTaskStatus(existingTask);

                return updatedTask != null ? _mapper.Map<TaskDetailDto>(updatedTask) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task status for ID: {TaskId}", updateTaskStatusDto.Id);
                return null;
            }
        }



        public async Task<List<TaskListDto>> GetAllTasks()
        {
            try
            {
                var tasks = await _taskRepository.GetTasks();
                return _mapper.Map<List<TaskListDto>>(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all tasks");
                return new List<TaskListDto>();
            }
        }

        public async Task<TaskDetailDto?> GetTaskById(int taskId)
        {
            try
            {
                if (taskId <= 0)
                {
                    _logger.LogWarning("Invalid taskId passed to GetTaskById: {TaskId}", taskId);
                    return null;
                }

                var task = await _taskRepository.GetTask(t => t.Id == taskId);
                return task != null ? _mapper.Map<TaskDetailDto>(task) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get task by ID: {TaskId}", taskId);
                return null;
            }
        }


        //public async Task<List<TaskListDto>?> GetTasksAssignedToUser(int userId)
        //{
        //    try
        //    {
        //        if (userId <= 0)
        //        {
        //            _logger.LogWarning("Invalid userId passed to GetTasksAssignedToUser: {UserId}", userId);
        //            return null;
        //        }

        //        var tasks = await _taskRepository.GetTasks(t => t.AssignedToId == userId);
        //        return _mapper.Map<List<TaskListDto>>(tasks);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get tasks assigned to user: {UserId}", userId);
        //        return null;
        //    }
        //}

        //public async Task<List<TaskListDto>?> GetTasksCreatedByUser(int userId)
        //{
        //    try
        //    {
        //        if (userId <= 0)
        //        {
        //            _logger.LogWarning("Invalid userId passed to GetTasksCreatedByUser: {UserId}", userId);
        //            return null;
        //        }

        //        var tasks = await _taskRepository.GetTasks(t => t.CreatedById == userId);
        //        return _mapper.Map<List<TaskListDto>>(tasks);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get tasks created by user: {UserId}", userId);
        //        return null;
        //    }
        //}

        //public async Task<List<TaskListDto>?> GetTasksByStatus(string status)
        //{
        //    try
        //    {
        //        if (!IsValidStatus(status))
        //        {
        //            _logger.LogWarning("Invalid status passed to GetTasksByStatus: {Status}", status);
        //            return null;
        //        }

        //        var tasks = await _taskRepository.GetTasks(t => t.Status == status);
        //        return _mapper.Map<List<TaskListDto>>(tasks);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get tasks by status: {Status}", status);
        //        return null;
        //    }
        //}


        public async Task<List<TaskListDto>> GetFilteredTasks(TaskFilterDto filterDto)
        {
            try
            {
                Expression<Func<Tasks, bool>> filter = task => true;

                if (!string.IsNullOrWhiteSpace(filterDto.Title))
                    filter = task => task.Title.Contains(filterDto.Title);

                if (!string.IsNullOrWhiteSpace(filterDto.Status) && IsValidStatus(filterDto.Status))
                    filter = filter.And(task => task.Status == filterDto.Status);

                if (filterDto.AssignedToId.HasValue)
                    filter = filter.And(task => task.AssignedToId == filterDto.AssignedToId);

                if (filterDto.CreatedById.HasValue)
                    filter = filter.And(task => task.CreatedById == filterDto.CreatedById);

                if (filterDto.CreatedAfter.HasValue)
                    filter = filter.And(task => task.CreatedAt >= filterDto.CreatedAfter.Value);

                if (filterDto.CreatedBefore.HasValue)
                    filter = filter.And(task => task.CreatedAt <= filterDto.CreatedBefore.Value);

                Func<IQueryable<Tasks>, IOrderedQueryable<Tasks>> orderBy = filterDto.SortBy?.ToLower() switch
                {
                    "title" => filterDto.SortDescending
                        ? q => q.OrderByDescending(t => t.Title)
                        : q => q.OrderBy(t => t.Title),

                    "status" => filterDto.SortDescending
                        ? q => q.OrderByDescending(t => t.Status)
                        : q => q.OrderBy(t => t.Status),

                    "createdat" or _ => filterDto.SortDescending
                        ? q => q.OrderByDescending(t => t.CreatedAt)
                        : q => q.OrderBy(t => t.CreatedAt)
                };

                var tasks = await _taskRepository.GetTasks(filter, orderBy);
                return _mapper.Map<List<TaskListDto>>(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get filtered tasks");
                return new List<TaskListDto>();
            }
        }

        private bool IsValidStatus(string status)
        {
            var validStatuses = new[] { "Todo", "InProgress", "Done" };
            return validStatuses.Contains(status);
        }
    }
}
