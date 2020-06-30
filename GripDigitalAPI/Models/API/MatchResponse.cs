using GripDigitalAPI.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GripDigitalAPI.Models.API
{
    public class MatchResponse : Response
    {
        public List<Match> MatchList { get; set; }

        public MatchResponse()
        {
            this.MatchList = new List<Match>();
        }
    }
}
