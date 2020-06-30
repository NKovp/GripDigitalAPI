using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using GripDigitalAPI.Infrastructure;
using GripDigitalAPI.Models.Game;
using GripDigitalAPI.Models.API;
using System.Security.Claims;
using GripDigitalAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace GripDigitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        /// <summary>
        /// Nefunguje autorizace. TODO: Vyresit JWT token
        /// TODO: Vylepsit architekturu
        /// Prozatim chodi token v headeru.
        /// Dodelat loogovani
        /// </summary>

        #region PRIVATE

        #region Data

        private static List<string> _allowedTokens = new List<string>();

        private List<Match> _matchList = new List<Match>()
        {
            new Match()
            {
                IsActive = false,
                Name = "Test match",
                Dummy1 = "Dummy",
                Dummy2 = 12
            },
            new Match()
            {
                IsActive = true,
                Name = "Test match2",
                Dummy1 = "Dummy2",
                Dummy2 = 0
            },
            new Match()
            {
                IsActive = true,
                Name = "Test match3",
                Dummy1 = "Dummy3",
                Dummy2 = 18
            },
            new Match()
            {
                IsActive = false,
                Name = "Test match4",
                Dummy1 = "Dummy4",
                Dummy2 = 7
            },
        };

        private List<Player> _players = new List<Player>()
        {
            new Player()
            {
                Nickname = "Testovaci",
                Score = new Dictionary<string, int>()
                {
                    { GameTypes.Type1.ToString(), 15 },
                    { GameTypes.Type3.ToString(), 10 },
                }
            },

            new Player()
            {
                Nickname = "Player2",
                Score = new Dictionary<string, int>()
                {
                    { GameTypes.Type1.ToString(), 10 },
                    { GameTypes.Type2.ToString(), 18 },
                    { GameTypes.Type3.ToString(), 8 },
                }
            },

            new Player()
            {
                Nickname = "Player3",
                Score = new Dictionary<string, int>()
                {
                    { GameTypes.Type1.ToString(), 5 },
                    { GameTypes.Type2.ToString(), 5 },
                    { GameTypes.Type3.ToString(), 14 },
                }
            }
        };

        private List<User> _allowedUsers = new List<User>()
        {
            new User
            {
                Login = "test",
                Password = "Test",
            },
            new User
            {
                Login = "root",
                Password = "heslo15"
            },
        };

        #endregion

        private readonly ILogger<GameController> _logger;

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User user = _allowedUsers.FirstOrDefault(x => x.Login == username && x.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // User not found
            return null;
        }

        /// <summary>
        /// Method will check token
        /// </summary>
        /// <param name="token">string token</param>
        /// <returns>True / False</returns>
        private static bool CheckAuthority(string token)
        {
            // Check received data
            if (string.IsNullOrEmpty(token) || !_allowedTokens.Contains(token))
                return false;

            return true;
        }
        #endregion


        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }


        // GET: api/Game
        [HttpGet("leaderboards")]
        public string GetLeaderboards([FromHeader] string token, string gameType)
        {
            // Default response
            var gameResponse = new GameResponse()
            {
                IsSuccess = false
            };

            try
            {
                if (!CheckAuthority(token))
                {
                    gameResponse.Errors.Add(GameConstants.INVALID_TOKEN_ERROR); 
                    return JsonConvert.SerializeObject(gameResponse);
                }

                #region DUMMY
                var gameList = new List<GameResponse>()
            {
                new GameResponse()
                {
                    GameType = GameTypes.Type1.ToString(),
                    Players = new List<Player>()
                    {
                        _players[0],
                        _players[2],
                    }
                },

                new GameResponse()
                {
                    GameType = GameTypes.Type2.ToString(),
                    Players = new List<Player>()
                    {
                        _players[0],
                        _players[1],
                        _players[2],
                    }
                },

                new GameResponse()
                {
                    GameType = GameTypes.Type3.ToString(),
                    Players = new List<Player>()
                    {
                        _players[1],
                        _players[2],
                    }
                },
            };
                #endregion

                // Find game by gametype
                gameResponse = gameList.Find(x => x.GameType == gameType);

                // Assign current score from Dictionary.
                // Zde je to blbe udelane, vim ze by se dalo lepe. Je to tim ze mam blbe navrzene dummy objekty
                foreach(var item in gameResponse.Players)
                {
                    item.CurrentScore = item.Score[gameType];
                }

                // Sort by score
                gameResponse.Players = gameResponse.Players.OrderByDescending(x => x.CurrentScore).ToList();

                if (gameResponse != null)
                {
                    gameResponse.IsSuccess = true;
                    var json = JsonConvert.SerializeObject(gameResponse);
                    return json;
                }
            }
            catch(Exception E)
            {
                // Logging
            }

            return null;
        }

        // GET: api/Game/5
        [HttpGet("matches")]
        public string GetMatches([FromHeader] string token)
        {
            var matchResponse = new MatchResponse()
            {
                IsSuccess = false
            };

            try
            {
                if (!CheckAuthority(token))
                {
                    matchResponse.Errors.Add(GameConstants.INVALID_TOKEN_ERROR);
                    return JsonConvert.SerializeObject(matchResponse);
                }

                var activeMatches = _matchList.Where(x => x.IsActive == true).ToList();

                if(activeMatches == null || activeMatches.Count == 0)
                {
                    var errorText = "Matches not found.";
                    matchResponse.Errors.Add(errorText);
                    return JsonConvert.SerializeObject(matchResponse);
                }

                matchResponse.IsSuccess = true;
                matchResponse.MatchList = activeMatches;

                return JsonConvert.SerializeObject(matchResponse);

            }
            catch(Exception E)
            {
                // Logging
                return null;
            }
        }

        /// <summary>
        /// Method check login data and generate hash for API
        /// Credentials is case sensitive
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Response model</returns>
        // POST: api/game/login
        [HttpPost("login")]
        public LoginResponse PostLogin(string username, string password)
        {
            _logger.LogTrace(GameConstants.LOG_START_FUNC);
            _logger.LogTrace($"[PostLogin] => (Starting...) : username: {username}, password: {password}");

            try
            {
                var responseModel = new LoginResponse()
                {
                    IsSuccess = false
                };

                _logger.LogDebug($"[PostLogin] => Default response model created.");

                // Try get identity
                var identity = GetIdentity(username, password);
                if (identity == null)
                {
                    var errorText = "Invalid username or password.";

                    _logger.LogWarning($"[PostLogin] => {errorText}");

                    responseModel.Errors.Add(errorText);
                    return responseModel;
                }

                var timeNow = DateTime.UtcNow;

                // Create JWT token
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: timeNow,
                    claims: identity.Claims,
                    expires: timeNow.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                responseModel.ApiToken = encodedJwt;
                responseModel.IsSuccess = true;
                responseModel.Username = username;

                _allowedTokens.Add(responseModel.ApiToken);

                return responseModel;
            }
            catch(Exception E)
            {
                _logger.LogError($"[PostLogin] => (Exception) : Message: {E.Message}");
                _logger.LogError($"[PostLogin] => (Exception) : {E}");

                return null;
            }
        }

        // POST: api/game/match/join
        [HttpPost("match/join")]
        public string PostJoinRoom(string username, string password)
        {
            _logger.LogTrace(GameConstants.LOG_START_FUNC);
            _logger.LogTrace($"[PostLogin] => (Starting...) : username: {username}, password: {password}");

            if(username == null && password == null)
            {
                username = "test";
                password = "Test";
            }

            try
            {
                var responseModel = new LoginResponse()
                {
                    IsSuccess = false
                };

                _logger.LogDebug($"[PostLogin] => Default response model created.");

                // Try get identity
                var identity = GetIdentity(username, password);
                if (identity == null)
                {
                    var errorText = "Invalid username or password.";

                    _logger.LogWarning($"[PostLogin] => {errorText}");

                    responseModel.Errors.Add(errorText);
                    return JsonConvert.SerializeObject(responseModel);
                }

                var timeNow = DateTime.UtcNow;

                // Create JWT token
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: timeNow,
                    claims: identity.Claims,
                    expires: timeNow.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                responseModel.ApiToken = encodedJwt;
                responseModel.IsSuccess = true;
                responseModel.Username = username;

                _allowedTokens.Add(responseModel.ApiToken);

                var jsonResponse = JsonConvert.SerializeObject(responseModel);

                return jsonResponse;
            }
            catch (Exception E)
            {
                _logger.LogError($"[PostLogin] => (Exception) : Message: {E.Message}");
                _logger.LogError($"[PostLogin] => (Exception) : {E}");

                return null;
            }
        }
    }
}
