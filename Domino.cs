using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using LiteDB;

namespace DominoBlockchain
{
    public class Domino : Script
    {
        public static bool Running;
        
        public static Block CurrentBlock;

        public const string MainAccountName = "DominoBlockchain";
        public const decimal TaxAmount = 0.02m; // 2%
        public const ulong MaximumEconomySupply = 5_000_000;
        public const int BlockTime = 5_000; // MS
        public const int MaxTxPerBlock = 128;

        // Time
        //private static DateTime LastQueueCheck = DateTime.Now.AddSeconds(5);
        //private static DateTime TimeSinceLastBlock = DateTime.Now.AddMilliseconds(BlockTime); // 30 Sec. Block Time
        // Setup

        // Entry point
        public Domino()
        {
            NAPI.Util.ConsoleOutput("[DOMINO] Initializing..");

            Database.Initialize();
            Worker.Initialize();

            // First setup for the collection if it does not exist. Create our first transaction that is our money cap.
            if (!Database.CollectionExists<Block>())
            {
                CreateGenesisBlock();
                NAPI.Util.ConsoleOutput("[DOMINO] Genesis Block Created.");
                Running = true;
                return;
            }

            // Check if Verification is proper.
            if (!Verification.VerifyAllBlocks())
            {
                NAPI.Util.ConsoleOutput("[DOMINO] Block verification failed. Stopping resource...");
                Running = false;
                return;
            }

            // If transactions are verified successfully move foreward.
            NAPI.Util.ConsoleOutput("[DOMINO] Blocks verified successfully.");
            Running = true;

            // Create new transaction
            Utility.CreateNewTransaction(MainAccountName, "Stuyk", 1000);
        }

        [ServerEvent(Event.PlayerSpawn)]
        public void HashPlayerNameOnSpawn(Client client)
        {
            client.SetData("DOMINO", Utility.ComputeHash(client.Name));
        }

        /// <summary>
        /// Create the first transaction for the program.
        /// </summary>
        private static void CreateGenesisBlock()
        {
            // Create a new block.
            var block = Block.CreateNewBlock(previousHash: Utility.ComputeHash("Stuyk"), maxSupply: MaximumEconomySupply, mainAccount: MainAccountName);

            // Generate a new transaction.
            var transaction = Transaction.CreateNewTransaction(sender: Utility.ComputeHash(MainAccountName, MaximumEconomySupply, Utility.RandGen.Next()), receiver: Utility.ComputeHash(MainAccountName), amount: MaximumEconomySupply);
            block.ConfirmedTx.Add(transaction);

            // Get the hash we want to try and use.
            string hash = Utility.ComputeHash(block.PreviousHash, block.ConfirmedTx, block.CreationDate, block.Nonce);
            
            // Begin signing the block.
            while (hash.Substring(0, 3) != "000")
            {
                block.Nonce++;
                hash = Utility.ComputeHash(block.PreviousHash, block.ConfirmedTx, block.CreationDate, block.Nonce);
            }

            block.Hash = hash;

            // Block found. Add it to our database.
            Database.GetCollection<Block>().InsertEx(block);
        }
    }
}