using EFCore.Service.DTOModels;
using EFCore.Service.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace EFCorePOC.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDTO order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _orderService.AddOrder(order);
                if (result == null)
                {
                    return StatusCode(500, "Failed to add the order");
                }

                TempData["message"] = $"Order with ID {result.Id} has been added successfully.";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditOrder([FromBody] OrderDTO order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _orderService.UpdateOrder(order);
                if (result == null)
                {
                    return NotFound("Order not found.");
                }

                TempData["message"] = $"Order with ID {result.Id} has been updated successfully.";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListOrders(
            [FromQuery] int? id = null,
            [FromQuery] int? userId = null,
            [FromQuery] int? statusId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] decimal? minTotal = null,
            [FromQuery] decimal? maxTotal = null,
            [FromQuery] string? sortBy = null)
        {
            try
            {
                var orders = await _orderService.GetOrders(
                    filter: o => (!id.HasValue || o.Id == id) &&
                                 (!userId.HasValue || o.UserId == userId) &&
                                 (!statusId.HasValue || o.StatusId == statusId) &&
                                 (!fromDate.HasValue || o.CreatedAt >= fromDate.Value) &&
                                 (!toDate.HasValue || o.CreatedAt <= toDate.Value) &&
                                 (!minTotal.HasValue || o.TotalAmount >= minTotal.Value) &&
                                 (!maxTotal.HasValue || o.TotalAmount <= maxTotal.Value),
                    orderBy: sortBy switch
                    {
                        "id" => q => q.OrderBy(o => o.Id),
                        "id_desc" => q => q.OrderByDescending(o => o.Id),
                        "date" => q => q.OrderBy(o => o.CreatedAt),
                        "date_desc" => q => q.OrderByDescending(o => o.CreatedAt),
                        "amount" => q => q.OrderBy(o => o.TotalAmount),
                        "amount_desc" => q => q.OrderByDescending(o => o.TotalAmount),
                        "status" => q => q.OrderBy(o => o.StatusId),
                        "status_desc" => q => q.OrderByDescending(o => o.StatusId),
                        _ => null
                    },
                    includeFields: true,
                    includeUser: true
                );

                if (id.HasValue)
                {
                    var singleOrder = orders.FirstOrDefault();
                    if (singleOrder == null)
                    {
                        return NotFound("Order not found.");
                    }
                    return Ok(singleOrder);
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> OrderList()
        {
            try
            {
                // Get user info from session
                var userJson = HttpContext.Session.GetString("User");
                var user = !string.IsNullOrEmpty(userJson)
                    ? System.Text.Json.JsonSerializer.Deserialize<UserDTO>(userJson)
                    : null;

                // Filter orders based on user role
                if (user?.Role == "Admin")
                {
                    // Admin sees all orders
                    var orders = await _orderService.GetOrders(
                        filter: null,
                        orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                        includeFields: false,
                        includeUser: false
                    );
                    return View("OrderList", orders);
                }
                else if (user != null)
                {
                    // Regular user sees only their orders
                    var orders = await _orderService.GetOrders(
                        filter: o => o.UserId == user.Id,
                        orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                        includeFields: false,
                        includeUser: false
                    );
                    return View("OrderList", orders);
                }
                else
                {
                    // Not logged in
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
