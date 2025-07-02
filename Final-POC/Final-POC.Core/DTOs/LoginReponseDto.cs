using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_POC.Core.DTOs
{
    public class LoginReponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}
