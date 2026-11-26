using System.Reflection;

namespace PelicanTownPrivilege.Configuration
{
    /// <summary>
    /// Settings stored in config.json. Each privilege section can be switched off on its own.
    /// Changes take effect the next time the game starts.
    /// </summary>
    public sealed class ModConfig
    {
        // Pierre's Privilege Counter (Data/Shops: SeedShop)
        public bool EnablePrivilegeShop { get; set; } = true;
        public int ShopPurchasePrice { get; set; } = 1;
        public bool UnlimitedPrivilegeStock { get; set; } = true;
        public int PrivilegeStockLimit { get; set; } = 10;
        public bool EnableWateringStock { get; set; } = true;
        public bool EnableAnimalEquipment { get; set; } = true;
        public bool EnableProcessingMachines { get; set; } = true;
        public bool EnableStorageStock { get; set; } = true;
        public bool EnableRareEggStock { get; set; } = true;
        public bool EnableGiftStock { get; set; } = true;
        public bool EnableGalaxyWeapons { get; set; } = true;
        public bool EnableCropSeeds { get; set; } = true;

        // Clint's Blacksmith (Data/Shops: Blacksmith)
        public bool EnablePremiumTools { get; set; } = true;

        // Wide-area farming tools: Privilege Hoe, Planter, Scythe, Pickaxe, and Axe. ToolWidth and ToolDepth apply to all five.
        public bool EnableWideHoe { get; set; } = true;
        public bool EnableWidePlanter { get; set; } = true;
        public bool EnableWideScythe { get; set; } = true;
        public bool EnableWidePickaxe { get; set; } = true;
        public bool EnableWideAxe { get; set; } = true;
        public bool EnableWideSprinkler { get; set; } = true;
        public int ToolWidth { get; set; } = 9;
        public int ToolDepth { get; set; } = 9;
        public bool AllowWideToolsOutsideFarmAndGreenhouse { get; set; } = false;
        public string LoadSeedsKey { get; set; } = "F";

        // Robin's construction menu (Data/Buildings: Junimo Hut)
        public bool EnableEarlyJunimoHuts { get; set; } = true;
        public int JunimoHutPrice { get; set; } = 0;
        public bool JunimoHutRequiresMaterials { get; set; } = false;
        public bool InstantJunimoConstruction { get; set; } = true;

        // Marnie's shop and incubation (Data/FarmAnimals and incubator machines)
        public bool EnableEarlyBlueChickens { get; set; } = true;
        public int BlueChickenPrice { get; set; } = 1;
        public bool EnableOvernightIncubation { get; set; } = true;
        public bool EnableDailyDinosaurEggs { get; set; } = true;

        public bool EnableDebugLogging { get; set; } = false;

        /// <summary>True when the name is a true/false setting, so catalog rows can reference it by name.</summary>
        public static bool IsToggle(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            PropertyInfo property = typeof(ModConfig).GetProperty(name.Trim(), BindingFlags.Public | BindingFlags.Instance);
            return property != null && property.PropertyType == typeof(bool);
        }

        /// <summary>Returns the value of a true/false setting by name. Unknown names return false.</summary>
        public bool IsEnabled(string toggleName)
        {
            if (!IsToggle(toggleName)) return false;
            PropertyInfo property = typeof(ModConfig).GetProperty(toggleName.Trim(), BindingFlags.Public | BindingFlags.Instance);
            return (bool)property.GetValue(this);
        }
    }
}
