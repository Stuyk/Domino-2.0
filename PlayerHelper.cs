﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domino
{
    public class PlayerHelper
    {
        public string PlayerName { get; set; }
        public string Hash { get; set; }
        public Client Client { get; set; }

        public PlayerHelper(Client client, string secret = "")
        {
            Client = client;
            PlayerName = client.Name;
            Hash = Utility.ComputeHash(PlayerName + secret);
            client.SetData("DominoAccount", this);
        }
    }
}
