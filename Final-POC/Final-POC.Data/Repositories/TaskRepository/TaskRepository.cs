using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Data.Contexts;
using Final_POC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Final_POC.Data.Repositories.TaskRepository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(ApplicationDbContext context, ILogger<TaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Tasks?> GetTask(Expression<Func<Tasks, bool>> predicate)
        {
            try
            {
                return await _context.Tasks
                    .Include(t => t.AssignedTo)
                    .Include(t => t.CreatedBy)
                    .Include(t => t.TaskDetail)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get task with predicate: {Predicate}", predicate);
                return null;
            }
        }

        public async Task<List<Tasks>> GetTasks(
            Expression<Func<Tasks, bool>>? filter = null,
            Func<IQueryable<Tasks>, IOrderedQueryable<Tasks>>? orderBy = null)
        {
            try
            {
                IQueryable<Tasks> query = _context.Tasks
                    .Include(t => t.AssignedTo)
                    .Include(t => t.CreatedBy)
                    .Include(t => t.TaskDetail)
                    .AsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get tasks");
                return new List<Tasks>();
            }
        }

        public async Task<Tasks?> CreateTask(Tasks task)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Tasks.AddAsync(task);
                    await _context.SaveChangesAsync();
                    var createdTask = await GetTask(x => x.Id == task.Id);
                    await transaction.CommitAsync();
                    return createdTask;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to create task");
                    return null;
                }
            }
        }

        public async Task<Tasks?> UpdateTaskStatus(Tasks task)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingTask = await _context.Tasks
                        .Include(x => x.TaskDetail)
                        .FirstOrDefaultAsync(x => x.Id == task.Id);

                    if (existingTask == null) return null;

                    existingTask.Status = task.Status;
                    await _context.SaveChangesAsync();
                    var updatedTask = await GetTask(x => x.Id == task.Id);
                    await transaction.CommitAsync();
                    return updatedTask;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to update task status for task ID: {TaskId}", task.Id);
                    return null;
                }
            }
        }

        public async Task<Tasks?> UpdateTask(Tasks task)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingTask = await _context.Tasks
                        .Include(x => x.TaskDetail)
                        .FirstOrDefaultAsync(x => x.Id == task.Id);

                    if (existingTask == null) return null;

                    existingTask.Title = task.Title;
                    existingTask.AssignedToId = task.AssignedToId;

                    if (existingTask.TaskDetail == null)
                    {
                        if (task.TaskDetail != null)
                        {
                            existingTask.TaskDetail = new TaskDetail
                            {
                                Description = task.TaskDetail.Description,
                                TaskId = existingTask.Id
                            };
                        }
                    }
                    else
                    {
                        if (task.TaskDetail != null)
                        {
                            existingTask.TaskDetail.Description = task.TaskDetail.Description;
                        }
                    }

                    await _context.SaveChangesAsync();
                    var updatedTask = await GetTask(x => x.Id == task.Id);
                    await transaction.CommitAsync();
                    return updatedTask;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to update task for ID: {TaskId}", task.Id);
                    return null;
                }
            }
        }
    }
}
