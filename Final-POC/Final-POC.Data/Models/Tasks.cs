using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_POC.Data.Models
{
    public class Tasks
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // "Todo", "InProgress", "Done" as string

        // Make foreign keys nullable
        public int? AssignedToId { get; set; }
        public int? CreatedById { get; set; }

        //[ForeignKey("AssignedToId")]
        public virtual User AssignedTo { get; set; }

        //[ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; }

        // Navigation property to TaskDetail
        public virtual TaskDetail TaskDetail { get; set; }
    }
}
