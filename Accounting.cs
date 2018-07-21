using GTANetworkAPI;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domino
{
    public class Accounting
    {
        /// <summary>
        /// Retrieve account balance based on just accountName.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static decimal GetAccountBalance(Client client)
        {
            string account = GetClientHelper(client).Hash;

            if (account == null)
                return 0;

            IEnumerable<Block> blocks = Database.FullBlockCollection;

            decimal negativeTrans = 0;
            decimal positiveTrans = 0;

            foreach (Block block in blocks)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (transaction.Reciever == account)
                    {
                        positiveTrans += transaction.Amount;
                    }

                    if (transaction.Sender == account)
                    {
                        negativeTrans += transaction.Amount;
                    }
                }
            }

            return (positiveTrans - negativeTrans);
        }

        /// <summary>
        /// Retrieve the account has enough balance. True or False.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool HasEnoughBalance(Client client, decimal amount)
        {
            return GetAccountBalance(client) >= amount;
        }

        /// <summary>
        /// Send a single transaction that will not allow additional transactions until complete.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reciever"></param>
        /// <param name="amount"></param>
        public static void SendPeerToPeerTransaction(Client client, string reciever, decimal amount)
        {
            // Why is the type decimal???
            if (amount == 0 || amount > 100_000 || !HasEnoughBalance(client, amount))
                return;

            Client recievingParty = GetClientByName(reciever);

            if (recievingParty == null)
            {
                client.SendChatMessage($"~o~{reciever} ~w~is ~r~not available~w~. Sending transaction to ~o~{reciever}.");
            }
            
            Transaction.Create(client.Name, reciever, amount);
        }

        public static void SendServerRewardTransaction(Client client, decimal amount)
        {
            if (amount >= 5000)
            {
                Console.WriteLine("[Domino] Excessive amount for reward. Refusing transaction.");
                return;
            }

            Transaction.Create(Settings.ServerAccount, client.Name, amount);
        }

        /// <summary>
        /// Retrieve a player name from hash. Only works for Online players obviously.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string GetNameFromHash(string hash)
        {
            return Domino.PlayerList.FirstOrDefault(x => x.Hash == hash)?.PlayerName;
        }

        /// <summary>
        /// Player Helper class is returned to pull HASH or PlayerName attached to Hash.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static PlayerHelper GetClientHelper(Client client)
        {
            return Domino.PlayerList.FirstOrDefault(x => x.Client == client);
        }

        /// <summary>
        /// Get potential online client.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Client GetClientByName(string name)
        {
            return NAPI.Pools.GetAllPlayers().FirstOrDefault(x => x.Name == name);
        } 


    }
}
