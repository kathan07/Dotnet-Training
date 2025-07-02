using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;

namespace EFCore.Data.Repositories.OrderRepository
{
    public interface IOrderRepository
    {
        Task<Order?> AddOrder(Order order);
        Task<Order?> GetOrder(int id, bool includeFields = false, bool includeUser = false);
        Task<IEnumerable<Order>> GetOrders(
            Expression<Func<Order, bool>>? filter = null,
            Func<IQueryable<Order>, IOrderedQueryable<Order>>? orderBy = null,
            bool includeFields = false,
            bool includeUser = false);
        Task<Order?> UpdateOrder(Order order);
    }
}
