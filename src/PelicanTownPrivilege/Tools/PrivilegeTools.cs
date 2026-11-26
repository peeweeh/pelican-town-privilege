using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;

namespace PelicanTownPrivilege.Tools
{
    /// <summary>
    /// Mod-owned tags stored in Item.modData. Every key starts with Prefix, so cleanup can find them and the game
    /// ignores them when the mod is not installed (the item then behaves as an ordinary hoe).
    /// </summary>
    public static class PrivilegeTools
    {
        public const string Prefix = "PelicanTownPrivilege/";
        public const string KindKey = Prefix + "Tool";
        public const string SeedIdKey = Prefix + "SeedId";
        public const string SeedCountKey = Prefix + "SeedCount";
        public const string ShopActionKey = Prefix + "Action";

        public const string WideHoeKind = "WideHoe";
        public const string PlanterKind = "WidePlanter";
        public const string WideScytheKind = "WideScythe";
        public const string WidePickaxeKind = "WidePickaxe";
        public const string UpgradeTrashCanAction = "UpgradeTrashCan";

        public const string WideHoeName = "Privilege Hoe";
        public const string PlanterName = "Privilege Planter";
        public const string WideScytheName = "Privilege Scythe";
        public const string WidePickaxeName = "Privilege Pickaxe";

        public static bool IsWideHoe(Item item) => item is Hoe && HasTag(item, KindKey, WideHoeKind);

        public static bool IsPlanter(Item item) => item is Hoe && HasTag(item, KindKey, PlanterKind);

        public static bool IsWideScythe(Item item) => item is MeleeWeapon && HasTag(item, KindKey, WideScytheKind);

        public static bool IsWidePickaxe(Item item) => item is Pickaxe && HasTag(item, KindKey, WidePickaxeKind);

        /// <summary>The display name of a privilege tool, or null for any other item.</summary>
        public static string NameOf(Item item)
        {
            if (IsWideHoe(item)) return WideHoeName;
            if (IsPlanter(item)) return PlanterName;
            if (IsWideScythe(item)) return WideScytheName;
            if (IsWidePickaxe(item)) return WidePickaxeName;
            return null;
        }

        public static bool IsTrashCanSentinel(Item item) => HasTag(item, ShopActionKey, UpgradeTrashCanAction);

        public static bool HasPrivilegeState(Item item)
        {
            if (item?.modData == null) return false;
            foreach (string key in item.modData.Keys)
                if (key.StartsWith(Prefix, System.StringComparison.Ordinal)) return true;
            return false;
        }

        public static string GetSeedId(Item planter)
        {
            return planter?.modData != null && planter.modData.TryGetValue(SeedIdKey, out string id) ? id : null;
        }

        public static int GetSeedCount(Item planter)
        {
            if (planter?.modData == null || !planter.modData.TryGetValue(SeedCountKey, out string text)) return 0;
            return int.TryParse(text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int count) ? count : 0;
        }

        /// <summary>Stores the seed type and count. A count of zero clears the planter.</summary>
        public static void SetSeeds(Item planter, string seedId, int count)
        {
            if (count <= 0 || string.IsNullOrEmpty(seedId))
            {
                planter.modData.Remove(SeedIdKey);
                planter.modData.Remove(SeedCountKey);
                return;
            }

            planter.modData[SeedIdKey] = seedId;
            planter.modData[SeedCountKey] = count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>The seed's display name, or the raw ID if the game does not know it.</summary>
        public static string SeedName(string seedId)
        {
            if (string.IsNullOrEmpty(seedId)) return "seeds";
            return ItemRegistry.Exists(seedId) ? ItemRegistry.Create(seedId).DisplayName : seedId;
        }

        /// <summary>Removes every mod-owned tag from the item, turning a privilege tool into an ordinary hoe.</summary>
        public static void ClearPrivilegeState(Item item)
        {
            if (item?.modData == null) return;
            var keys = new List<string>();
            foreach (string key in item.modData.Keys)
                if (key.StartsWith(Prefix, System.StringComparison.Ordinal)) keys.Add(key);
            foreach (string key in keys)
                item.modData.Remove(key);
        }

        private static bool HasTag(Item item, string key, string expected)
        {
            return item?.modData != null && item.modData.TryGetValue(key, out string value) && value == expected;
        }
    }
}
