using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.Service.DTOModels;


namespace EFCore.Service.DTOModels
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public StatusDTO? Status { get; set; }
        public ICollection<OrderProductDTO>? OrderProducts { get; set; }
    }
}
