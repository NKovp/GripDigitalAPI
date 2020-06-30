using GripDigitalAPI.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GripDigitalAPI.Models.API
{
    public class GameResponse : Response
    {
        public string GameType { get; set; }
        public List<Player> Players { get; set; }
    }
}
