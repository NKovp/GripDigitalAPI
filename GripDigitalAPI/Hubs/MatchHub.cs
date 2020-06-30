using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace GripDigitalAPI.Hubs
{
    //[Authorize]
    public class MatchHub : Hub
    {
        /// <summary>
        /// Position je 1 pro vsechny. Takze kdyz dalsi uzivatel klikne na smer, prida se hodnota k minule. TODO: Rozdelil bych to jeste pro kazdeho usera zvlast
        /// TODO: Vyresit autorizaci
        /// TODO: Vylepsit architekturu
        /// </summary>

        private static List<int> _connectedUsers = new List<int>();
        private static int _startId = 1;
        private static Vector3 position = new Vector3(0f, 0f, 0f);

        //public async Task Send(string message)
        //{
        //    await Clients.All.SendAsync("Receive", message);
        //}

        public async Task Fire()
        {
            await this.Clients.All.SendAsync("Fire");
        }

        public async Task Move(string direction)
        {

            switch(direction)
            {
                case "left":
                    position.X -= 1;
                    break;
                case "right":
                    position.X += 1;
                    break;
                case "up":
                    position.Y += 1;
                    break;
                case "down":
                    position.Y -= 1;
                    break;
                default:
                    break;
            }

            await this.Clients.All.SendAsync("Move", position.ToString());
        }

        public async Task GetPlayers()
        {
            await this.Clients.All.SendAsync("GetPlayers", _connectedUsers);
        }

        public override Task OnConnectedAsync()
        {
            _connectedUsers.Add(_startId);
            _startId++;
            return base.OnConnectedAsync();
        }
    }
}
