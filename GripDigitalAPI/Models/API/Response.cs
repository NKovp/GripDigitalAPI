using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GripDigitalAPI.Models.API
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }

        public Response()
        {
            this.Errors = new List<string>();
        }
    }
}
