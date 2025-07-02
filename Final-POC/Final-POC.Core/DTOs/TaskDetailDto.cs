using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_POC.Core.DTOs
{
    public class TaskDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public int? CreatedById { get; set; }
        public string CreatedByName { get; set; }
    }
}
