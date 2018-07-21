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
        public bool WasTaxTransaction { get; set; } = false;

        /// <summary>
        /// Create a new transaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        /// <param name="amount"></param>
        public Transaction CreateTransaction(string sender, string reciever, decimal amount, bool isTaxTransaction = false)
        {
            if (isTaxTransaction)
            {
                TaxTransaction(sender, reciever, amount);
                return this;
            }

            Sender = Utility.ComputeHash(sender);
            Reciever = Utility.ComputeHash(reciever);
            Amount = amount;

            decimal taxAmount = DetermineTax(Amount);
            Amount = Amount - taxAmount;
            TransactionFee(taxAmount);

            if (Worker.CurrentBlock == null)
            {
                Worker.CurrentBlock = new Block();
            }

            if (Worker.CurrentBlock.Transactions.Count >= Settings.MaxTxPerBlock)
            {
                Worker.QueuedBlocks.Enqueue(Worker.CurrentBlock);
                Worker.CurrentBlock = new Block();
            }

            Worker.CurrentBlock.Transactions.Add(this);
            return this;
        }

        private void TaxTransaction(string sender, string reciever, decimal amount)
        {
            Sender = sender;
            Reciever = reciever;
            Amount = amount;
            WasTaxTransaction = true;

            if (Worker.CurrentBlock == null)
            {
                Worker.CurrentBlock = new Block();
            }

            if (Worker.CurrentBlock.Transactions.Count >= Settings.MaxTxPerBlock)
            {
                Worker.QueuedBlocks.Enqueue(Worker.CurrentBlock);
                Worker.CurrentBlock = new Block();
            }

            Worker.CurrentBlock.Transactions.Add(this);
        }

        private void TransactionFee(decimal amount)
        {
            Transaction transaction = new Transaction();
            transaction.CreateTransaction(Sender, Utility.ComputeHash(Settings.ServerAccount), amount, true);
        }

        private decimal DetermineTax(decimal amount)
        {
            return amount * Settings.TaxAmount;
        }
    }
}
