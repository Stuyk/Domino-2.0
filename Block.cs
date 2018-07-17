using GTANetworkAPI;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace DominoBlockchain
{
    public class Block : StandardData
    {
        public int Nonce { get; set; }
        public string PreviousHash { get; set; }
        public readonly ThreadSafeList<Transaction> ConfirmedTx = new ThreadSafeList<Transaction>();

        private string RetrieveHash() => Utility.ComputeHash(PreviousHash, ConfirmedTx, CreationDate, Nonce);
        public static Block CreateNewBlock(string previousHash, ulong maxSupply, string mainAccount) => new Block { PreviousHash = Utility.ComputeHash("Stuyk", Domino.MaximumEconomySupply, Domino.MainAccountName) };

        public Block()
        {
            string hash = RetrieveHash();

            // Find the hash we need.
            while (hash.Substring(0, 3) != "000")
            {
                if (Utility.GetLatestBlockHash() != PreviousHash)
                    PreviousHash = Utility.GetLatestBlockHash();

                Nonce++;
                hash = RetrieveHash();
            }

            Hash = hash;

            Database.GetCollection<Block>().InsertEx(this);
            NAPI.Util.ConsoleOutput("[DOMINO] Block mined successfully.");
        }


    }
}
