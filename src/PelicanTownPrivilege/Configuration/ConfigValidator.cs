using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace PelicanTownPrivilege.Configuration
{
    /// <summary>Loads config.json, corrects invalid values, and writes the corrected file back.</summary>
    public static class ConfigValidator
    {
        public const int DefaultPrice = 1;
        public const int DefaultJunimoHutPrice = 0;
        public const int DefaultStockLimit = 10;
        public const int DefaultToolWidth = 10;
        public const int DefaultToolDepth = 10;
        public const int MaxToolSize = 15;
        public const string DefaultLoadSeedsKey = "F";

        public static ModConfig Load(IModHelper helper, IMonitor monitor)
        {
            ModConfig config;
            try
            {
                config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            }
            catch (Exception ex)
            {
                monitor.Log($"config.json could not be read ({ex.Message}). Using default settings.", LogLevel.Warn);
                config = new ModConfig();
            }

            List<string> corrections = Correct(config);
            foreach (string message in corrections)
                monitor.Log($"Config corrected: {message}", LogLevel.Warn);
            if (corrections.Count > 0)
                helper.WriteConfig(config);

            monitor.Log("Config loaded. Changes to config.json take effect after restarting the game.", LogLevel.Info);
            return config;
        }

        /// <summary>Fixes invalid values in place and returns one message per correction.</summary>
        public static List<string> Correct(ModConfig config)
        {
            var messages = new List<string>();

            if (config.ShopPurchasePrice < 0)
            {
                messages.Add($"ShopPurchasePrice was {config.ShopPurchasePrice}. Negative prices are not allowed; reset to {DefaultPrice}.");
                config.ShopPurchasePrice = DefaultPrice;
            }
            if (config.JunimoHutPrice < 0)
            {
                messages.Add($"JunimoHutPrice was {config.JunimoHutPrice}. Negative prices are not allowed; reset to {DefaultJunimoHutPrice}.");
                config.JunimoHutPrice = DefaultJunimoHutPrice;
            }
            if (config.BlueChickenPrice < 0)
            {
                messages.Add($"BlueChickenPrice was {config.BlueChickenPrice}. Negative prices are not allowed; reset to {DefaultPrice}.");
                config.BlueChickenPrice = DefaultPrice;
            }
            if (config.PrivilegeStockLimit < 1)
            {
                messages.Add($"PrivilegeStockLimit was {config.PrivilegeStockLimit}. It must be at least 1; reset to {DefaultStockLimit}.");
                config.PrivilegeStockLimit = DefaultStockLimit;
            }
            if (config.ToolWidth < 1 || config.ToolWidth > MaxToolSize)
            {
                messages.Add($"ToolWidth was {config.ToolWidth}. It must be a whole number from 1 to {MaxToolSize}; reset to {DefaultToolWidth}.");
                config.ToolWidth = DefaultToolWidth;
            }
            if (config.ToolDepth < 1 || config.ToolDepth > MaxToolSize)
            {
                messages.Add($"ToolDepth was {config.ToolDepth}. It must be a whole number from 1 to {MaxToolSize}; reset to {DefaultToolDepth}.");
                config.ToolDepth = DefaultToolDepth;
            }
            if (string.IsNullOrWhiteSpace(config.LoadSeedsKey) || !Enum.TryParse(config.LoadSeedsKey.Trim(), true, out SButton _))
            {
                messages.Add($"LoadSeedsKey was '{config.LoadSeedsKey}', which is not a known key name. Reset to {DefaultLoadSeedsKey}.");
                config.LoadSeedsKey = DefaultLoadSeedsKey;
            }

            return messages;
        }
    }
}
