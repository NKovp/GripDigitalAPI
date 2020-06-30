using GripDigitalAPI.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GripDigitalAPI.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Player GameProfile { get; set; }
    }
}
