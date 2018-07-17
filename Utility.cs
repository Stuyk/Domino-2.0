using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using GTANetworkAPI;
using LiteDB;

namespace DominoBlockchain
{
    public static class Utility
    {
        public static readonly Random RandGen = new Random();

        /// <summary>
        /// Returns a SHA256 hash from supplied arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string ComputeHash(params object[] args)
        {
            var ctrHash = string.Empty;
            foreach (var o in args) ctrHash += o;
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(ctrHash));
                var strbuilder = new StringBuilder();
                for (var i = 0; i < bytes.Length; i++) strbuilder.Append(bytes[i].ToString("x2"));
                return strbuilder.ToString();
            }
        }

        /// <summary>
        /// Generate a new transaction and push to the database if account balance is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static void CreateNewTransaction(string sender, string reciever, ulong amount)
        {
            // Verify all current blocks before we add our transaction.
            //if (!Verification.VerifyAllBlocks()) return; // unnecessary

            if (Domino.CurrentBlock == null) Domino.CurrentBlock = CreateBlock();

            // Create the new transaction.
            var transaction = new Transaction
            {
                Sender = ComputeHash(sender),
                Reciever = ComputeHash(reciever),
                Amount = amount
            };

            // Add it to our queue system.
            Worker.QueuedTransactions.Enqueue(transaction);
            NAPI.Util.ConsoleOutput("[DOMINO] Created a new transaction.");

            if (Domino.CurrentBlock?.ConfirmedTx.Count < 128) return;

            // Generate our new block if the old block is full.
            Domino.CurrentBlock = CreateBlock();
        }

        public static Block CreateBlock()
        {
            return new Block // Generate a New Block
            {
                PreviousHash = !string.IsNullOrEmpty(Domino.CurrentBlock.Hash) ? Domino.CurrentBlock.Hash : GetLatestBlockHash(), // Get the last Block from collection.
                CreationDate = DateTime.UtcNow.ToBinary()
            };
        }

        public static string GetLatestBlockHash()
        {
            var queryCol = Database.GetCollection<Block>();
            var findCol = queryCol?.FindOne(Query.All(Query.Descending));
            var hashCol = findCol?.Hash;
            Console.WriteLine(hashCol);
            return hashCol;
        }

        internal static CollectionType ParseType(this string input, [CallerMemberName] string f = null, [CallerLineNumber] int n = 0)
        {
            Console.WriteLine(f + "--> " + n + " -- " + input);
            Enum.TryParse(input, out CollectionType result);
            return result;
        }
    }
}
