using GTANetworkAPI;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace DominoBlockchain
{
    public class Accounting
    {
        /*
        /// <summary>
        /// Retrieve the integer amount of available account balance.
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static decimal RetrieveAccountBalance(string accountName, bool preHashed = false)
        {
            var collection = Domino.db.GetCollection<Transaction>("Transactions");

            string accountHash;
            if (preHashed)
            {
                accountHash = accountName;
            } else {
                accountHash = Utility.RetrieveHash(new string[] { accountName });
            }
            
            var recievedTransactions = collection.Find(Query.EQ("Reciever", accountHash));
            var senderTransactions = collection.Find(Query.EQ("Sender", accountHash));

            decimal negativeTransactions = 0;
            decimal positiveTransactions = 0;

            foreach (var transaction in recievedTransactions)
            {
                positiveTransactions += transaction.Amount;
            }

            foreach (var transaction in senderTransactions)
            {
                negativeTransactions += transaction.Amount;
            }

            decimal finalAmount = positiveTransactions - negativeTransactions;

            return finalAmount;
        }

        /// <summary>
        /// Does the account have enough balance?
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool HasEnoughAccountBalance(string accountName, decimal amount, bool prehashed = false)
        {
            decimal accountBalance = RetrieveAccountBalance(accountName, prehashed);

            if (accountBalance >= amount)
            {
                return true;
            }

            return false;
        }

        [Command("send")]
        public void createTransaction(Client client, string reciever, decimal amount)
        {
            amount = Math.Round(amount);

            if (amount < 5)
            {
                client.SendNotification("~r~No.");
                return;
            }

            if (!HasEnoughAccountBalance(client.Name, amount, false))
            {
                client.SendNotification("~r~Not enough account balance.");
                client.SendNotification("~r~Not enough account balance.");
                return;
            }

            Utility.CreateNewTransaction(client.Name, reciever, amount);
            Client cReciever = NAPI.Player.GetPlayerFromName(reciever);

            client.SendNotification($"~w~Attempting to send money to {reciever}. Amount: ${amount}");
            client.SendChatMessage($"~w~Attempting to send money to {reciever}. Amount: ${amount}");
            if (cReciever != null)
            {
                cReciever.SendNotification($"~w~You are recieving money from ~o~{client.Name}~w~.");
                cReciever.SendChatMessage($"~w~You are recieving money from ~o~{client.Name}~w~. ~b~Awaiting verification...");
            }
        }

        [Command("balance")]
        public void balance(Client client)
        {
            decimal amount = RetrieveAccountBalance(client.Name);
            client.SendNotification($"Account Balance: ${amount}");
            client.SendChatMessage($"Account Balance: ${amount}");
        }

        [Command("economy")]
        public void economyBalance(Client client)
        {
            decimal amount = RetrieveAccountBalance(Domino.MainAccountName);

            decimal econ = Domino.MaximumEconomySupply - amount; // Difference

            decimal final = econ + amount;





            client.SendChatMessage($"Economy Health: ~g~{final} / {Domino.MaximumEconomySupply}");
            client.SendChatMessage($"Player Holdings: ~g~{econ} / {Domino.MaximumEconomySupply}");
        }
        */
    }
}
