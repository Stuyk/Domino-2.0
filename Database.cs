using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Domino
{
    internal class Database
    {
        private static bool _ledgerMutated;

        private static int _count;
        public static int Count
        {
            get
            {
                if (_ledgerMutated) _count = BlockCollection.Count();
                return _count;
            }
        }

        private static LiteCollection<Block> _ledgerCollection;
        public static LiteCollection<Block> BlockCollection
        {
            get
            {
                if (_ledgerMutated)
                {
                    _ledgerCollection = GetCollection<Block>();
                    _ledgerMutated = false;
                }
                return _ledgerCollection;
            }
        }

        private static IEnumerable<Block> _fullLedgerCollection;
        public static IEnumerable<Block> FullBlockCollection
        {
            get
            {
                if (_ledgerMutated)
                {
                    _fullLedgerCollection = GetFullCollection<Block>();
                    _ledgerMutated = false;
                }
                return _fullLedgerCollection;
            }
        }

        // Initialize our Database.
        public static void Initialize()
        {
            Settings.Initialize();

            // Get the collection if it exists.
            if (File.Exists(Settings.DatabaseLocation + Settings.DatabaseFile))
            {
                Console.WriteLine("--> [Domino] Database Found. Beginning Verification. \r\n");
                Verification.VerifyAllBlocks();
                Worker.Initialize();
                return;
            }

            // Else let's run the genesis block creation.
            GenerateGenesisBlock();
        }

        /// <summary>
        /// Get the entire collection based on its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IEnumerable<T> GetFullCollection<T>()
        {
            using (var db = new LiteDatabase(Settings.DatabaseLocation + Settings.DatabaseFile))
            {
                return db.GetCollection<T>().FindAll();
            }
        }

        /// <summary>
        /// Get the collection based on its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static LiteCollection<T> GetCollection<T>()
        {
            using (var db = new LiteDatabase(Settings.DatabaseLocation + Settings.DatabaseFile))
            {
                return db.GetCollection<T>();
            }
        }

        /// <summary>
        /// Check if the collection of the class type exists inside of our database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Exists<T>()
        {
            using (var db = new LiteDatabase(Settings.DatabaseLocation + Settings.DatabaseFile))
            {
                return db.CollectionExists(typeof(T).Name);
            }
        }

        /// <summary>
        /// Add a document to the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void AddToCollection<T>(T data)
        {
            using (var db = new LiteDatabase(Settings.DatabaseLocation + Settings.DatabaseFile))
            {
                db.GetCollection<T>().Insert(data);
            }

            _ledgerMutated = true;
        }

        /// <summary>
        /// Get the last block in the database.
        /// </summary>
        /// <returns></returns>
        public static Block LastBlock()
        {
            using (var db = new LiteDatabase(Settings.DatabaseLocation + Settings.DatabaseFile))
            {
                return db.GetCollection<Block>().FindOne(Query.All(Query.Descending));
            }
        }

        /// <summary>
        /// Generate the Genesis block for our database.
        /// </summary>
        private static void GenerateGenesisBlock()
        {
            if (Exists<Block>())
                return;

            Console.WriteLine("--> [Domino] Creating Genesis Block...");
            // Create our Max Economy Transaction
            Transaction transaction = new Transaction
            {
                Amount = Settings.MaximumEconomySupply,
                Sender = Utility.ComputeHash("GENESIS", Settings.ServerAccount),
                Reciever = Utility.ComputeHash(Settings.ServerAccount)
            };

            // Add our transaction to a new block.
            Block block = new Block(transaction);
            block.MineBlock();
            Initialize();
        }
        
    }
}
