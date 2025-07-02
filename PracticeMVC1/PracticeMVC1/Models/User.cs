using System.ComponentModel.DataAnnotations;

namespace PracticeMVC1.Models
{
    public class User
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
