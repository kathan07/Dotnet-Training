using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_POC.Data.Models
{
    public class TaskDetail
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Foreign key to Task
        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public virtual Tasks Task { get; set; }
    }
}
