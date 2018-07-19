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
        public static readonly object LockObject = new object();
        
        public static Block CurrentBlock;

        public const string MainAccountName = "DominoBlockchain";
        public const decimal TaxAmount = 0.02m; // 2%
        public const ulong MaximumSupply = ulong.MaxValue;
        public const int BlockTime = 5_000; // MS
        public const int MaxTxPerBlock = 128;

        public static readonly ThreadSafeCollection<Transaction> ConfirmedTxs = new ThreadSafeCollection<Transaction>();

        // Entry point
        public Domino()
        {
            NAPI.Util.ConsoleOutput("[DOMINO] Initializing..");

            Database.Initialize();
            CurrentBlock = Database.LastBlock;
            //Dispatcher.Test();

            // First setup for the collection if it does not exist. Create our first transaction that is our money cap.
            if (!Database.Exists<Block>())
            {
                CreateGenesisBlock();
                NAPI.Util.ConsoleOutput("[DOMINO] Genesis Block Created.");
                return;
            }

            // Check if the blocks are properly signed.
            if (!Block.VerifyAllBlocks())
            {
                NAPI.Util.ConsoleOutput("[DOMINO] Block verification failed. Stopping resource...");
                return;
            }

            // If transactions are verified successfully move foreward.
            NAPI.Util.ConsoleOutput("[DOMINO] Blocks verified successfully.");

            // Create new dummy transaction
            new Transaction { Sender = MainAccountName, Reciever = "Stuyk", Amount = 1000 };
        }

        [ServerEvent(Event.PlayerSpawn)]
        public void HashPlayerNameOnSpawn(Client client)
        {
            client.SetData("DOMINO", Util.ComputeHash(client.Name));
        }

        /// <summary>
        /// Create the first transaction for the program.
        /// </summary>
        private static void CreateGenesisBlock()
        {
            // Create a new block.
            CurrentBlock = new Block { Hash = Util.ComputeHash("Stuyk", MaximumSupply, MainAccountName)};

            // Generate a new transaction.
            var mainAccountHash = Util.ComputeHash(MainAccountName);
            new Transaction
            {
                Sender = mainAccountHash,
                Reciever = mainAccountHash,
                Amount = MaximumSupply
            };

            //// Get the hash we want to try and use.
            //string hash = Util.ComputeHash(block.PreviousBlockHash, block.ConfirmedTx, block.Time, block.Nonce);
            
            //// Begin signing the block.
            //while (hash.Substring(0, 3) != "000")
            //{
            //    block.Nonce++;
            //    hash = Util.ComputeHash(block.PreviousBlockHash, block.ConfirmedTx, block.Time, block.Nonce);
            //}

            //block.Hash = hash;
        }
    }
}