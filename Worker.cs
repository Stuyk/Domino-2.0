using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DominoBlockchain
{
    public static class Worker
    {
        public static ThreadSafeQueue<Transaction> QueuedTransactions = new ThreadSafeQueue<Transaction>();
        //public static ThreadSafeQueue<Block> QueuedBlocks = new ThreadSafeQueue<Block> {Type = CollectionType.Block};

        public static void Initialize()
        {
            var workerThread = new Thread(Pulse) {IsBackground = true};
            workerThread.Start();
        }

        public static void Pulse()
        {
            while (Domino.Running)
            {
                PulseTransactions();
                //Pulse(QueuedBlocks);
                Thread.Sleep(100);
            }
        }

        public static void PulseTransactions()
        {
            if (QueuedTransactions.Count < 1) return;
            Domino.CurrentBlock.ConfirmedTx.AddRange(QueuedTransactions.TakeAndRemove(Domino.MaxTxPerBlock));

            if (Domino.CurrentBlock.ConfirmedTx.Count >= Domino.MaxTxPerBlock)
            {
                var priorHash = Domino.CurrentBlock.Hash;
                Domino.CurrentBlock = Block.CreateNewBlock(previousHash: priorHash, maxSupply: Domino.MaximumEconomySupply, mainAccount: Domino.MainAccountName);
            }
        }

        public static IEnumerable<T> TakeAndRemove<T>(this Queue<T> queue, int count)
        {
            for (var i = 0; i < count && queue.Count > 0; i++) yield return queue.Dequeue();
        }
    }

    public class ThreadSafeList<T> : List<T>
    {
        private readonly object _lockObject = new object();

        public new void AddRange(IEnumerable<T> collection)
        {
            lock (_lockObject) base.AddRange(collection);
        }

        public new void Add(T item)
        {
            lock (_lockObject) base.Add(item);
        }

        public new void Remove(T item)
        {
            lock (_lockObject) base.Remove(item);
        }

        public new int Count
        {
            get
            {
                int returnValue;
                lock (_lockObject) returnValue = base.Count;
                return returnValue;
            }
        }

        public new void Clear()
        {
            lock (_lockObject) base.Clear();
        }
    }

    public class ThreadSafeQueue<T> : Queue<T>
    {
        private readonly object _lockObject = new object();

        public new int Count
        {
            get
            {
                int returnValue;
                lock (_lockObject) returnValue = base.Count;
                return returnValue;
            }
        }

        public new void Clear()
        {
            lock (_lockObject) base.Clear();
        }

        public new T Dequeue()
        {
            T returnValue;
            lock (_lockObject) returnValue = base.Dequeue();
            return returnValue;
        }

        public new void Enqueue(T item)
        {
            lock (_lockObject) base.Enqueue(item);
        }
    }
}

