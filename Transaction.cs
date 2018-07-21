using System;
using System.Collections.Generic;
using System.Text;

namespace Domino
{
    internal class Transaction : StandardData
    {
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public decimal Amount { get; set; }
        public bool TaxedTransaction { get; set; } // false by default
        public Transaction TaxTransaction { get; set; }

        /// <summary>
        /// Create a tranasaction.
        /// </summary>
        public Transaction()
        {
            Create(Sender, Reciever, Amount, TaxedTransaction);
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        /// <param name="amount"></param>
        /// <param name="taxed">(optional)</param>
        public static Transaction Create(string sender, string reciever, decimal amount, bool taxed = false)
        {
            var transaction = new Transaction
            {
                Sender = Utility.ComputeHash(sender),
                Reciever = Utility.ComputeHash(reciever),
                Amount = amount,
                TaxedTransaction = taxed
            };

            if (taxed)
            {
                decimal taxAmount = amount * Settings.TaxAmount;
                transaction.Amount -= taxAmount;
                transaction.TaxTransaction = Create(transaction.Sender, Utility.ComputeHash(Settings.ServerAccount), taxAmount, true); 
            }

            if (Worker.CurrentBlock == null)
            {
                Worker.CurrentBlock = new Block();
            }

            if (Worker.CurrentBlock.Transactions.Count >= Settings.MaxTxPerBlock)
            {
                Worker.QueuedBlocks.Enqueue(Worker.CurrentBlock);
                Worker.CurrentBlock = new Block();
            }

            Worker.CurrentBlock.Transactions.Add(transaction);
            return transaction;
        }
    }
}
