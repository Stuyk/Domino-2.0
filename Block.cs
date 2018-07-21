using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Domino
{
    internal class Block : StandardData
    {
        public string PreviousHash { get; set; } // null by default
        public int Nonce { get; set; }
        public List<Transaction> Transactions { get; set; }

        // Set the Nonce when the block is created and setup the previous hash.
        public Block()
        {
            Transactions = new List<Transaction>();
            Nonce = 0;
        }

        /// <summary>
        /// Mine our block and determine the Nonce necessary to validate it.
        /// </summary>
        public void MineBlock()
        {
            Worker.CurrentlyMining = true;
            Hash = RetrieveHash();

            Block lastBlock = Database.LastBlock();
            bool skipForGenesis = false;

            if (lastBlock == null)
            {
                skipForGenesis = true;
                PreviousHash = Utility.ComputeHash(Settings.ServerAccount + Settings.MaximumEconomySupply);
            }

            DateTime startTime = DateTime.Now;

            while (Hash.Substring(0, 3) != "000")
            {
                if (!skipForGenesis)
                {
                    if (PreviousHash == null)
                        PreviousHash = Database.LastBlock().Hash;

                    if (Database.LastBlock().Hash != PreviousHash)
                        PreviousHash = Database.LastBlock().Hash;
                }

                Nonce++;
                Hash = RetrieveHash();
            }

            Database.AddToCollection(this);
            Console.WriteLine($"--> [Domino] Block Confirmed. Added to Database.");
            Console.WriteLine($"ID: {ID} || Transaction Count: {Transactions.Count} || Difficulty: {Nonce}");
            Console.WriteLine($"Unconfirmed Blocks: {Worker.QueuedBlocks.Count} || Transaction Time: {(DateTime.Now - startTime).Minutes}m {(DateTime.Now - startTime).Seconds}s \r\n");
            AlertTransactions();
            Worker.CurrentlyMining = false;
        }

        private void AlertTransactions()
        {
            foreach (Transaction trans in Transactions)
            {
                if (trans.WasTaxTransaction)
                    continue;

                Client reciever = NAPI.Player.GetPlayerFromName(Accounting.GetNameFromHash(trans.Reciever));
                Client sender = NAPI.Player.GetPlayerFromName(Accounting.GetNameFromHash(trans.Sender));

                if (reciever != null)
                    reciever.SendChatMessage($"Recieved: ~g~${trans.Amount}");

                if (sender != null)
                    sender.SendChatMessage($"Transaction Complete: ~r~-{trans.Amount}");
            }
        }

        public string RetrieveHash() => Utility.ComputeHash(PreviousHash, Transactions, CreationDate, Nonce);
    }
}
