using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Domino
{
    internal class Worker
    {
        public static bool Running = false;
        public static Block CurrentBlock;
        public static Queue<Block> QueuedBlocks;
        public static bool CurrentlyMining = false;
        private static int _timeSinceLastBlockCheck;

        public static void Initialize()
        {
            Console.WriteLine("--> [Domino] Blocks Verified. Starting up worker thread.");
            QueuedBlocks = new Queue<Block>();

            Thread workerThread = new Thread(Pulse) { IsBackground = true };
            workerThread.Start();
        }

        public static void Pulse()
        {
            while (Running)
            {
                CheckBlocks();
                Thread.Sleep(1000);
            }
        }

        // This will check to see if our Queue has anything in it.
        public static void CheckBlocks()
        {
            if (CurrentBlock == null) return;

            var tmp = Environment.TickCount;
            if (tmp - _timeSinceLastBlockCheck >= Settings.BlockTime)
            {
                _timeSinceLastBlockCheck = tmp;

                if (CurrentBlock.Transactions.Count < 1)
                    return;

                QueuedBlocks.Enqueue(CurrentBlock);
                CurrentBlock = new Block();
            }

            if (CurrentBlock == null || QueuedBlocks.Count <= 0 || CurrentlyMining || !Verification.VerifyAllBlocks())
                return;

            QueuedBlocks.Dequeue().MineBlock();
        }
    }
}
