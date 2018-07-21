using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;


namespace Domino
{
    public class Commands : Script
    {
        public Commands()
        {

        }

        [Command("balance")]
        public void DominoGetBalance(Client client)
        {
            if (Settings.DisableBasicCommands)
                return;

            client.SendChatMessage($"Current Balance: {Accounting.GetAccountBalance(client)}");
        }

        [Command("send")]
        public void DominoSendMoney(Client client, string targetName, decimal amount)
        {
            if (Settings.DisableBasicCommands)
                return;

            Accounting.SendPeerToPeerTransaction(client, targetName, amount);
        }
    }
}
