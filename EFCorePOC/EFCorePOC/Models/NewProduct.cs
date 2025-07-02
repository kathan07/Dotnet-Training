using System.ComponentModel.DataAnnotations;

namespace EFCorePOC.Web.Models
{
    public class NewProduct
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
    }
}
