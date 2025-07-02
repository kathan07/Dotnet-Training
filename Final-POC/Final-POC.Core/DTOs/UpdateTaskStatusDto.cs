using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_POC.Core.DTOs
{
    public class UpdateTaskStatusDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }
    }
}
