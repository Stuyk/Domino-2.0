using System;
using System.Collections.Generic;
using System.Text;

namespace DominoBlockchain
{
    public class Transaction : StandardData
    {
        public string Sender { get; set; } // Who is this transaction taken from?
        public string Reciever { get; set; } // Who recieves this transaction?
        public ulong Amount { get; set; } // Amount Sent

        public static Transaction CreateNewTransaction(string sender, string receiver, ulong amount) 
            => new Transaction { Sender = sender, Reciever = receiver, Amount = amount };
    }
}
