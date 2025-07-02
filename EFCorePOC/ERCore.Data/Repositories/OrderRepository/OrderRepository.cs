using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.Data.Contexts;
using EFCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data.Repositories.OrderRepository
{
    public class OrderRepository: IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order?> AddOrder(Order order)
        {
            if (order == null)
            {
                return null;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                order.CreatedAt = DateTime.Now;
                await _dbContext.Orders.AddAsync(order);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetOrder(int id, bool includeFields = false, bool includeUser = false)
        {
            try
            {
                IQueryable<Order> query = _dbContext.Orders.AsNoTracking();

                if (includeFields)
                {
                    query = query.Include(o => o.OrderProducts)
                                 .ThenInclude(o => o.Product)
                                 .ThenInclude(o => o.Category)
                                 .Include(o => o.Status);
                }

                if (includeUser)
                {
                    query = query.Include(o => o.User);
                }

                return await query.FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<Order>> GetOrders(
            Expression<Func<Order, bool>>? filter = null,
            Func<IQueryable<Order>, IOrderedQueryable<Order>>? orderBy = null,
            bool includeFields = false,
            bool includeUser = false)
        {
            try
            {
                IQueryable<Order> query = _dbContext.Orders.AsNoTracking();

                if (includeFields)
                {
                    query = query.Include(o => o.OrderProducts)
                                 .ThenInclude(o => o.Product)
                                 .ThenInclude(o => o.Category)
                                 .Include(o => o.Status);
                }

                if (includeUser)
                {
                    query = query.Include(o => o.User);
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Order?> UpdateOrder(Order order)
        {
            if (order == null)
            {
                return null;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.Orders.Update(order);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
