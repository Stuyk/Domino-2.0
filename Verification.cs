using GTANetworkAPI;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace DominoBlockchain
{
    public class Verification : Script
    {
        /// <summary>
        /// Determine if all current transactions are VALID.
        /// </summary>
        /// <returns></returns>
        public static bool VerifyAllBlocks()
        {
            LiteCollection<Block> collection = Database.GetCollection<Block>();
            if (collection.CountEx() < 1) return true;

            var allBlocks = Database.GetFullCollection<Block>();

            var previousBlock = collection.FindById(1);
            foreach (var block in allBlocks)
            {
                string hash = Utility.ComputeHash(block.PreviousHash, block.ConfirmedTx, block.CreationDate, block.Nonce);

                if (block.Id == 1)
                {
                    if (block.Hash != hash)
                    {
                        NAPI.Util.ConsoleOutput($"[DOMINO] GENESIS TRANSACTION ERROR AT => ID: {block.Id}");
                        Domino.Running = false;
                        return false;
                    }

                    previousBlock = block;
                    continue;
                }

                if (block.Hash != hash)
                {
                    NAPI.Util.ConsoleOutput($"[DOMINO] TRANSACTION HASH ERROR AT => ID: {block.Id}");
                    Domino.Running = false;
                    return false;
                }

                if (block.PreviousHash != previousBlock.Hash)
                {
                    NAPI.Util.ConsoleOutput($"[DOMINO] TRANSACTION PREVIOUS HASH ERROR AT => ID: {block.Id}");
                    Domino.Running = false;
                    return false;
                }

                previousBlock = block;
            }
            return true;
        }
    }
}
