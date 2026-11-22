using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// ptp_cleanup prepares a save to load without this mod. It is a dry run unless "apply" is given.
    /// Apply makes a backup first, returns planter seeds to your inventory, and clears the mod's tags so each
    /// privilege tool becomes an ordinary hoe.
    /// </summary>
    public sealed class CleanupService
    {
        private const int MaxStackSize = 999;

        private readonly SaveMigrationService saveMigration;
        private readonly Diagnostics diagnostics;

        public CleanupService(SaveMigrationService saveMigration, Diagnostics diagnostics)
        {
            this.saveMigration = saveMigration;
            this.diagnostics = diagnostics;
        }

        public void Run(bool apply)
        {
            if (!Context.IsWorldReady)
            {
                diagnostics.Info("Load a save first, then run ptp_cleanup.");
                return;
            }

            List<PrivilegeFind> finds = Scan();
            if (finds.Count == 0)
            {
                diagnostics.Info("ptp_cleanup: no privilege tools or seeds found. This save is ready to load without the mod.");
                return;
            }

            diagnostics.Info($"ptp_cleanup: found {finds.Count} privilege tool(s):");
            foreach (PrivilegeFind find in finds)
                diagnostics.Info($"  {find.Kind} in {find.Where}" + (find.SeedCount > 0 ? $", holding {find.SeedCount} x {find.SeedId}" : string.Empty));

            if (!apply)
            {
                diagnostics.Info("Dry run only. Run 'ptp_cleanup apply' to clean these up. A save backup is created first.");
                return;
            }

            if (saveMigration.CreateBackup("before-cleanup") == null)
            {
                diagnostics.Warn("No backup could be created, so nothing was changed.");
                return;
            }

            int returned = 0;
            foreach (PrivilegeFind find in finds)
            {
                if (find.SeedCount > 0)
                {
                    int leftover = ReturnSeeds(find.SeedId, find.SeedCount);
                    returned += find.SeedCount - leftover;
                    if (leftover > 0)
                    {
                        PrivilegeTools.SetSeeds(find.Tool, find.SeedId, leftover);
                        diagnostics.Warn($"Could not fully clean {find.Kind} in {find.Where}: {leftover} seed(s) were not returned. Either your inventory is full (make room, then run ptp_cleanup apply again), or the game does not know seed '{find.SeedId}' (for example, it came from a mod you removed).");
                        continue;
                    }
                }

                PrivilegeTools.ClearPrivilegeState(find.Tool);
            }

            diagnostics.Info($"Returned {returned} seed(s) to your inventory.");
            List<PrivilegeFind> remaining = Scan();
            diagnostics.Info(remaining.Count == 0
                ? "Ready to load without the mod: YES."
                : $"Ready to load without the mod: NO. {remaining.Count} item(s) still need attention (see the warnings above).");
        }

        /// <summary>Looks through every player inventory and every chest for tools that carry mod tags.</summary>
        private static List<PrivilegeFind> Scan()
        {
            var finds = new List<PrivilegeFind>();

            foreach (Farmer farmer in Game1.getAllFarmers())
                AddFinds(farmer.Items, $"{farmer.Name}'s inventory", finds);

            foreach (GameLocation location in LocationScan.AllLocations())
                foreach (StardewValley.Object machine in location.objects.Values)
                    if (machine is Chest chest)
                        AddFinds(chest.Items, $"a chest in {location.Name}", finds);

            return finds;
        }

        private static void AddFinds(IEnumerable<Item> items, string where, List<PrivilegeFind> finds)
        {
            if (items == null) return;

            foreach (Item item in items)
            {
                if (!PrivilegeTools.HasPrivilegeState(item)) continue;

                finds.Add(new PrivilegeFind
                {
                    Tool = item,
                    Where = where,
                    Kind = PrivilegeTools.IsPlanter(item) ? "Privilege Planter" : PrivilegeTools.IsWideHoe(item) ? "Privilege Hoe" : "unknown privilege item",
                    SeedId = PrivilegeTools.GetSeedId(item),
                    SeedCount = PrivilegeTools.GetSeedCount(item)
                });
            }
        }

        /// <summary>Adds seeds to the player's inventory. Returns how many could not fit.</summary>
        private static int ReturnSeeds(string seedId, int count)
        {
            if (string.IsNullOrEmpty(seedId) || count <= 0) return count;
            if (!ItemRegistry.Exists(seedId)) return count;

            int remaining = count;
            while (remaining > 0)
            {
                int chunk = Math.Min(remaining, MaxStackSize);
                Item leftover = Game1.player.addItemToInventory(ItemRegistry.Create(seedId, chunk));
                int leftoverCount = leftover?.Stack ?? 0;
                remaining -= chunk - leftoverCount;
                if (leftover != null) break;
            }
            return remaining;
        }

        private sealed class PrivilegeFind
        {
            public Item Tool { get; set; }
            public string Where { get; set; }
            public string Kind { get; set; }
            public string SeedId { get; set; }
            public int SeedCount { get; set; }
        }
    }
}
