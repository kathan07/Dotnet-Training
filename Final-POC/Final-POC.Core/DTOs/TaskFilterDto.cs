using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_POC.Core.DTOs
{
    public class TaskFilterDto
    {
        public string? Title { get; set; }
        public string? Status { get; set; }
        public int? AssignedToId { get; set; }
        public int? CreatedById { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string SortBy { get; set; } = "CreatedAt"; // Default sort by creation date
        public bool SortDescending { get; set; } = true; // Default newest first
    }
}
