using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GripDigitalAPI.Models.API
{
    public class LoginResponse : Response
    {
        public string ApiToken { get; set; }
        public string Username { get; set; }
    }
}
