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
        private static int TimeSinceLastBlock = DateTime.Now.AddMilliseconds(Settings.BlockTime).Millisecond;

        public static void Initialize()
        {
            Console.WriteLine($"--> [Domino] Blocks Verified. Starting up worker thread. \r\n");
            QueuedBlocks = new Queue<Block>();
            Thread workerThread = new Thread(Pulse) { IsBackground = true };
            workerThread.Start();
        }

        public static void Pulse()
        {
            while (Running)
            {
                Thread.Sleep(100);
                CheckBlocks();
            }
        }

        // This will check to see if our Queue has anything in it.
        public static void CheckBlocks()
        {
            if (DateTime.Now.Millisecond > TimeSinceLastBlock)
            {
                TimeSinceLastBlock = DateTime.Now.AddMilliseconds(Settings.BlockTime).Millisecond;

                if (CurrentBlock == null || CurrentBlock.Transactions.Count < 1)
                    return;

                QueuedBlocks.Enqueue(CurrentBlock);
                CurrentBlock = new Block();
            }

            if (CurrentBlock == null)
                return;

            if (QueuedBlocks.Count <= 0)
                return;

            if (CurrentlyMining)
                return;

            if (!Verification.VerifyAllBlocks())
                return;

            Block mineBlock = QueuedBlocks.Dequeue();
            mineBlock.MineBlock();
        }
    }
}
