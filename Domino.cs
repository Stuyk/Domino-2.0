using System;
using System.IO;
using System.Runtime.CompilerServices;
using GTANetworkAPI;

[assembly: InternalsVisibleTo("Domino.Accounting")]
namespace Domino
{
    public class Domino : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Client client)
        {
            PlayerHelper playerHelper = new PlayerHelper(client);
        }

        public Domino()
        {
            Database.Initialize();

            
        }
    }
}