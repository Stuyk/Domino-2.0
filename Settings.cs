using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Domino
{
    public class Settings
    {
        [JsonProperty("DatabaseLocation")]
        public static string DatabaseLocation { get; set; } = @".\Domino";
        [JsonProperty("DatabaseName")]
        public static string DatabaseFile { get; set; } = @"\database.db";
        [JsonProperty("ServerAccount")]
        public static string ServerAccount { get; set; } = "Domino";
        [JsonProperty("TaxAmount")]
        public static decimal TaxAmount { get; set; } = 0.02m;
        [JsonProperty("MaximumEconomySupply")]
        public static ulong MaximumEconomySupply { get; set; } = 5000000;
        [JsonProperty("BlockTime")]
        public static int BlockTime { get; set; } = 30000;
        [JsonProperty("MaxTxPerBlock")]
        public static int MaxTxPerBlock { get; set; } = 128;
        [JsonProperty("DisableBasicCommands")]
        public static bool DisableBasicCommands { get; set; } = false;
        [JsonProperty("DisableNotifications")]
        public static bool DisableNotifications { get; set; } = false;

        public static void Initialize()
        {
            if (!Directory.Exists(DatabaseLocation))
                Directory.CreateDirectory(DatabaseLocation);

            if (!File.Exists(DatabaseLocation + @"\settings.json"))
            {
                SettingsHelper.SaveSettings();
                Console.WriteLine("--> [Domino] Settings not found. Generating default.");
                Console.WriteLine($"Location: {DatabaseLocation}");
            }
                
            SettingsHelper.LoadSettings();
            Console.WriteLine("--> [Domino] Settings found, reading settings.");
            Console.WriteLine($"Location: {DatabaseLocation}");
        }
    }

    public class SettingsHelper {

        public static Settings Settings = new Settings();

        public static void SaveSettings()
        {
            string serialize = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(Settings.DatabaseLocation + @"\settings.json", serialize);
        }

        public static void LoadSettings()
        {
            string deserialize = File.ReadAllText(Settings.DatabaseLocation + @"\settings.json");
            Settings = JsonConvert.DeserializeObject<Settings>(deserialize);
        }
    }
}
