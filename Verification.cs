using GTANetworkAPI;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domino
{
    internal class Verification
    {
        public static bool VerifyAllBlocks()
        {
            // If it's our genesis block. Ignore it.
            if (Database.Count < 1)
                return true;

            Block previousBlock = Database.LastBlock();
            foreach (Block block in Database.FullBlockCollection)
            {
                string hash = block.RetrieveHash();

                if (block.Hash != hash)
                {
                    NAPI.Util.ConsoleOutput(block.ID == 1
                        ? $"[DOMINO] GENESIS TRANSACTION ERROR AT => ID: {block.ID}"
                        : $"[DOMINO] TRANSACTION HASH ERROR AT => ID: {block.ID}");

                    NAPI.Log.Exception($"[DOMINO] TRANSACTION HASH ERROR AT => ID: {block.ID}");
                    Environment.Exit(0);
                    return false;
                }

                if (block.ID != 1)
                {
                    if (block.PreviousHash != previousBlock?.Hash)
                    {
                        NAPI.Util.ConsoleOutput($"[DOMINO] TRANSACTION PREVIOUS HASH ERROR AT => ID: {block.ID}");
                        NAPI.Log.Exception($"[DOMINO] TRANSACTION PREVIOUS HASH ERROR AT => ID: {block.ID}");
                        Environment.Exit(0);
                        return false;
                    }
                }

                previousBlock = block;
            }
            Worker.Running = true;
            return true;
        }
    }
}
