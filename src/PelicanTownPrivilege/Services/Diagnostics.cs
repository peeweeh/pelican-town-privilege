using System;
using System.Collections.Generic;
using System.Reflection;
using StardewModdingAPI;
using StardewValley;
using PelicanTownPrivilege.Configuration;

namespace PelicanTownPrivilege.Services
{
    /// <summary>Logging helpers and the data shown by the ptp_status console command.</summary>
    public sealed class Diagnostics
    {
        private readonly IMonitor monitor;
        private readonly ModConfig config;
        private readonly string modVersion;
        private readonly List<string> skipped = new List<string>();

        public Diagnostics(IMonitor monitor, ModConfig config, string modVersion)
        {
            this.monitor = monitor;
            this.config = config;
            this.modVersion = modVersion;
        }

        public void Info(string message) => monitor.Log(message, LogLevel.Info);

        public void Warn(string message) => monitor.Log(message, LogLevel.Warn);

        public void Error(string message, Exception exception) => monitor.Log($"{message}: {exception}", LogLevel.Error);

        public void Debug(string message)
        {
            if (config.EnableDebugLogging) monitor.Log(message, LogLevel.Debug);
        }

        /// <summary>Records a catalog row or action that was skipped, so ptp_status can list it.</summary>
        public void Skip(string label, string reason)
        {
            skipped.Add($"{label}: {reason}");
            Warn($"Skipped {label}: {reason}.");
        }

        public void PrintStatus()
        {
            Info($"Pelican Town Privilege {modVersion} | Stardew Valley {Game1.version} | SMAPI {Constants.ApiVersion}");
            Info("Settings:");
            foreach (PropertyInfo property in typeof(ModConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                Info($"  {property.Name} = {property.GetValue(config)}");

            Info(skipped.Count == 0 ? "Skipped catalog rows: none." : $"Skipped catalog rows ({skipped.Count}):");
            foreach (string line in skipped)
                Info("  " + line);
        }
    }
}
