using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Models;
using EFCore.Service.DTOModels;

namespace EFCore.Service.OrderService
{
    public interface IOrderService
    {
        Task<OrderDTO?> AddOrder(OrderDTO order);
        Task<OrderDTO?> GetOrder(int id, bool includeFields = false, bool includeUser = false);
        Task<IEnumerable<OrderDTO>> GetOrders(
            Expression<Func<Order, bool>>? filter = null,
            Func<IQueryable<Order>, IOrderedQueryable<Order>>? orderBy = null,
            bool includeFields = false,
            bool includeUser = false);
        Task<OrderDTO?> UpdateOrder(OrderDTO order);
    }
}
