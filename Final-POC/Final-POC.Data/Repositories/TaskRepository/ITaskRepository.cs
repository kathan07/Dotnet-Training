using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Final_POC.Data.Models;

namespace Final_POC.Data.Repositories.TaskRepository
{
    public interface ITaskRepository
    {
        Task<Tasks?> GetTask(Expression<Func<Tasks, bool>> predicate);
        Task<List<Tasks>> GetTasks(
           Expression<Func<Tasks, bool>>? filter = null,
           Func<IQueryable<Tasks>, IOrderedQueryable<Tasks>>? orderBy = null);
        Task<Tasks?> CreateTask(Tasks task);
        Task<Tasks?> UpdateTaskStatus(Tasks task);
        Task<Tasks?> UpdateTask(Tasks task);
    }
}
