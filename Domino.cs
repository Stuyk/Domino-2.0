using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GTANetworkAPI;

[assembly: InternalsVisibleTo("Domino.Accounting")]
namespace Domino
{
    public class Domino : Script
    {
        public static readonly List<PlayerHelper> PlayerList = new List<PlayerHelper>();

        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Client client)
        {
            PlayerList.Add(new PlayerHelper(client));
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void PlayerDisconnected(Client client)
        {
            var player = PlayerList.FirstOrDefault(x => x.Client == client);
            if (player != null) PlayerList.Remove(player);
        }

        public Domino()
        {
            Database.Initialize();
        }
    }
}