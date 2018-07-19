using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace DominoBlockchain
{
    public class Dispatcher
    {
        public static void Test()
        {
            for (var i = 0; i < 50; i++) EnqueueTransaction(new Transaction());
        }

        public static bool EnqueueTransaction(Transaction tx)
        {
            return ThreadPool.QueueUserWorkItem(state => ProcessTransaction(tx));
        }
        
        private static void ProcessTransaction(Transaction tx)
        {
            Console.WriteLine("[DOMINO] Processing TX on thread: " + Thread.CurrentThread.ManagedThreadId);

            // Compute transaction hash
            var toCheckAgainst = Util.ComputeHash(tx.Sender, tx.Reciever, tx.Amount, tx.Timestamp);
            if (toCheckAgainst != tx.Hash) return;

            Domino.ConfirmedTxs.Add(tx);
            if (Domino.ConfirmedTxs.Count() >= Domino.MaxTxPerBlock)
            {
                lock (Domino.LockObject) // Avoid thread racing
                {
                    // Store all transactions into the current block and clear the transactions buffer collection
                    Domino.CurrentBlock.Transactions = Domino.ConfirmedTxs.ToArrayAndClear();

                    // Create a new block and assign the current block to it as the previous block, then assign it as the current block
                    Domino.CurrentBlock = new Block { PreviousBlock = Domino.CurrentBlock };
                }
            }
        }
    }
}

