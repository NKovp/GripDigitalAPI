using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GripDigitalAPI.Infrastructure
{
    public class GameConstants
    {
        public const string LOG_START_FUNC = "-------------------------------------------------------------------------------------------";
        public const string INVALID_TOKEN_ERROR = "Token is null or invalid";
    }

    public enum GameTypes
    {
        Type1,
        Type2,
        Type3
    }
}
