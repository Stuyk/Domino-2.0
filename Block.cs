using GTANetworkAPI;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DominoBlockchain
{
    public class Block : Shared
    {
        public uint Nonce { get; set; }
        public string MerkleRootHash { get; set; }
        // public long Difficulty { get; set; }

        public Transaction[] Transactions;

        public Block PreviousBlock { get; set; }

        public Block() // When creating a new block, stamp the old one.
        {
            // Sign the previous block
            if (PreviousBlock == null) return;

            // Generate MarkleRoot hash
            PreviousBlock.MerkleRootHash = ComputeMerkleRootHash();
            
            // Generate Block hash
            PreviousBlock.Hash = Util.ComputeHash(PreviousBlock.Hash, PreviousBlock.MerkleRootHash, PreviousBlock.Nonce, PreviousBlock.Timestamp);

            NAPI.Util.ConsoleOutput("[DOMINO] New block found: " + PreviousBlock.Hash);
            Database.Add(PreviousBlock);
        }

        private string ComputeMerkleRootHash()
        {
            if (Transactions == null) return string.Empty;
            var txs = Transactions.Select(x => x.Hash).ToArray();

            string Hash2(string a, string b)
            {
                var a1 = Enumerable.Range(0, a.Length / 2).Select(x => Convert.ToByte(a.Substring(x * 2, 2), 16)).ToArray();
                Array.Reverse((Array)a1);
                var b1 = Enumerable.Range(0, b.Length / 2).Select(x => Convert.ToByte(b.Substring(x * 2, 2), 16)).ToArray();
                Array.Reverse((Array)b1);
                var c = a1.Concat(b1).ToArray();
                var sha256 = SHA256.Create();
                var firstHash = sha256.ComputeHash(c);
                var hashOfHash = sha256.ComputeHash(firstHash);
                Array.Reverse(hashOfHash);
                return BitConverter.ToString(hashOfHash).Replace("-", string.Empty).ToLower();
            }

            while (txs.Length > 1)
            {
                var hashList = new List<string>(txs.Length);
                var len = (txs.Length % 2 != 0) ? txs.Length - 1 : txs.Length;
                for (var i = 0; i < len; i += 2) hashList.Add(Hash2(txs[i], txs[i + 1]));
                if (len < txs.Length) hashList.Add(Hash2(txs[txs.Length - 1], txs[txs.Length - 1]));
                txs = hashList.ToArray();
            }

            return txs[0];
        }

        //private string RetrieveHash() => Util.ComputeHash(PreviousBlockHash, ConfirmedTx, CreationDate, Nonce);

        //public Block()
        //{
        //    string hash = RetrieveHash();

        //    // Find the hash we need.
        //    while (hash.Substring(0, 3) != "000")
        //    {
        //        if (LatestConfirmedBlock.Hash != PreviousBlockHash)
        //            PreviousBlockHash = LatestConfirmedBlock.Hash;

        //        Nonce++;
        //        hash = RetrieveHash();
        //    }

        //    Hash = hash;

        //    Database.GetCollection<Block>().InsertEx(this);
        //    NAPI.Util.ConsoleOutput("[DOMINO] Block mined successfully.");
        //}

        /// <summary>
        /// Determine if all current transactions are VALID.
        /// </summary>
        /// <returns></returns>
        public static bool VerifyAllBlocks()
        {
            if (Database.Count < 1) return true;

            Block previousBlock = null;
            foreach (var block in Database.FullLedgerCollection)
            {
                string hash = Util.ComputeHash(block.PreviousBlock.Hash, block.Transactions, block.Timestamp, block.Nonce);

                if (block.Hash != hash)
                {
                    NAPI.Util.ConsoleOutput(block.Id == 1
                        ? $"[DOMINO] GENESIS TRANSACTION ERROR AT => ID: {block.Id}"
                        : $"[DOMINO] TRANSACTION HASH ERROR AT => ID: {block.Id}");
                    return false;
                }

                if (block.PreviousBlock.Hash != previousBlock?.Hash)
                {
                    NAPI.Util.ConsoleOutput($"[DOMINO] TRANSACTION PREVIOUS HASH ERROR AT => ID: {block.Id}");
                    return false;
                }

                previousBlock = block;
            }
            return true;
        }
    }
}
