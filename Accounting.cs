using GTANetworkAPI;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domino
{
    public class Accounting
    {
        /// <summary>
        /// Retrieve account balance based on just accountName.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="isHashed"></param>
        /// <returns></returns>
        public static decimal GetAccountBalance(Client client)
        {
            string account = GetClientHelper(client).Hash;

            if (account == null)
                return 0;

            IEnumerable<Block> blocks = Database.GetFullCollection<Block>();

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
        /// <param name="accountName"></param>
        /// <param name="amount"></param>
        /// <param name="isHashed"></param>
        /// <returns></returns>
        public static bool HasEnoughBalance(Client client, decimal amount)
        {
            if (GetAccountBalance(client) >= amount)
                return true;

            return false;
        }

        /// <summary>
        /// Send a single transaction that will not allow additional transactions until complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        /// <param name="amount"></param>
        public static void SendPeerToPeerTransaction(Client client, string reciever, decimal amount)
        {
            if (amount <= 0)
                return;

            if (amount >= 100000)
                return;

            if (!HasEnoughBalance(client, amount))
                return;

            Client recievingParty = GetClientByName(reciever);

            if (recievingParty == null)
            {
                client.SendChatMessage($"~o~{reciever} ~w~is ~r~not available~w~. Sending transaction to ~o~{reciever}.");
            }
            
            Transaction transaction = new Transaction();
            transaction.CreateTransaction(client.Name, reciever, amount);
        }

        public static void SendServerRewardTransaction(Client client, decimal amount)
        {
            if (amount >= 5000)
            {
                Console.WriteLine("[Domino] Excessive amount for reward. Refusing transaction.");
                return;
            }

            Transaction transaction = new Transaction();
            transaction.CreateTransaction(Settings.ServerAccount, client.Name, amount);
        }

        /// <summary>
        /// Returns the server account name.
        /// </summary>
        /// <returns></returns>
        public static string GetServerAccountName()
        {
            return Settings.ServerAccount;
        }

        /// <summary>
        /// Retrieve a player name from hash. Only works for Online players obviously.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string GetNameFromHash(string hash)
        {
            foreach (Client client in NAPI.Pools.GetAllPlayers())
            {
                if ((client.GetData("DominoAccount") as PlayerHelper).Hash == hash)
                    return client.Name;
            }
            return null;
        }

        /// <summary>
        /// Player Helper class is returned to pull HASH or PlayerName attached to Hash.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static PlayerHelper GetClientHelper(Client client)
        {
            return client.GetData("DominoAccount") as PlayerHelper;
        }

        /// <summary>
        /// Get potential online client.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Client GetClientByName(string name)
        {
            return NAPI.Pools.GetAllPlayers().Find(x => x.Name == name);
        } 


    }
}
