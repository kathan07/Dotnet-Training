using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EFCore.Data.Models;
using EFCore.Data.Repositories.OrderRepository;
using EFCore.Service.DTOModels;

namespace EFCore.Service.OrderService
{
    public class OrderService: IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;


        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OrderDTO?> AddOrder(OrderDTO order)
        {
            if (order == null)
            {
                return null;
            }

            try
            {
                var orderEntity = _mapper.Map<Order>(order);
                var response = await _orderRepository.AddOrder(orderEntity);
                if (response == null) return null;

                var newOrder = await GetOrder(response.Id, true, true);
                return newOrder;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderDTO?> GetOrder(int id, bool includeFields = false, bool includeUser = false)
        {
            try
            {
                var response = await _orderRepository.GetOrder(id, includeFields, includeUser);
                if (response == null) return null;

                return _mapper.Map<OrderDTO>(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders(
            Expression<Func<Order, bool>>? filter = null,
            Func<IQueryable<Order>, IOrderedQueryable<Order>>? orderBy = null,
            bool includeFields = false,
            bool includeUser = false)
        {
            try
            {
                var response = await _orderRepository.GetOrders(filter, orderBy, includeFields, includeUser);
                return _mapper.Map<IEnumerable<OrderDTO>>(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderDTO?> UpdateOrder(OrderDTO order)
        {
            if (order == null)
            {
                return null;
            }

            try
            {
                var existingOrder = await _orderRepository.GetOrder(order.Id, true, true);
                if (existingOrder == null) return null;

                var updatedOrder = _mapper.Map(order, existingOrder);
                var response = await _orderRepository.UpdateOrder(updatedOrder);

                if (response == null) return null;

                var updatedOrderDTO = await GetOrder(response.Id, true, true);
                return updatedOrderDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
