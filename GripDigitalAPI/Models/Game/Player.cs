using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GripDigitalAPI.Models.Game
{
    public class Player
    {
        public string Nickname { get; set; }
        public int CurrentScore { get; set; }

        [JsonIgnore]
        public Dictionary<string, int> Score { get; set; }
    }
}
