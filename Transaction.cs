using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace DominoBlockchain
{
    public class Transaction : Shared
    {
        public string Sender { get; set; } // Who is this transaction taken from?
        public string Reciever { get; set; } // Who recieves this transaction?
        public ulong Amount { get; set; } // Amount Sent

        public string ComputeHash() => Util.ComputeHash(Sender, Reciever, Amount, Timestamp);

        public Transaction()
        {
            // Verify all current blocks before we add our transaction.
            //if (!Verification.VerifyAllBlocks()) return; // unnecessary

            //if (Main.CurrentBlock == null) Main.CurrentBlock = new Block { PreviousBlockHash = Block.LatestConfirmedBlock.Hash };

            // Create the new transaction object.

            // Add it to our queue system.

            // Generate TX Hash
            Hash = ComputeHash();

            NAPI.Util.ConsoleOutput("[DOMINO] New TX created.");
            Dispatcher.EnqueueTransaction(this);
        }

    }
}
