using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace EFCorePOC.Web.Models
{
    public class LoginUser
    {
        [Required]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }

    }
}
