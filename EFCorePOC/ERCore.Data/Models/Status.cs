using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Data.Models
{
    public class Status
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!; // "Pending", "Accepted", "Rejected", "Shipped", "Delivered"

        // Navigation Property
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
