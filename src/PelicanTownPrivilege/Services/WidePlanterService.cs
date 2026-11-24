using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// The Privilege Planter. Seeds are loaded explicitly and stored on the tool. Each activation plants one seed
    /// per valid empty tilled tile in the area. Invalid tiles are skipped without using a seed.
    /// </summary>
    public sealed class WidePlanterService
    {
        public const int MaxStackSize = 999;

        private readonly ModConfig config;

        public WidePlanterService(ModConfig config)
        {
            this.config = config;
        }

        /// <summary>Runs one planting activation. Returns true when handled, which means the normal hoe action must not run.</summary>
        public bool Plant(GameLocation location, Farmer who, Hoe planter)
        {
            if (!ToolFootprint.IsAllowedLocation(location, config))
            {
                Feedback.Show("The Privilege Planter works on the farm and in the greenhouse.");
                return true;
            }

            string seedId = PrivilegeTools.GetSeedId(planter);
            int stored = PrivilegeTools.GetSeedCount(planter);
            if (seedId == null || stored <= 0)
            {
                Feedback.Show($"The planter is empty. Hold seeds and press {config.LoadSeedsKey} to load them.");
                return true;
            }

            int planted = 0;
            foreach (Vector2 tile in ToolFootprint.Compute(who, config.ToolWidth, config.ToolDepth))
            {
                if (planted >= stored) break;
                if (!ToolFootprint.TryGetPlantableSoil(location, tile, seedId, out HoeDirt soil)) continue;
                if (soil.plant(seedId, who, isFertilizer: false)) planted++;
            }

            int remaining = stored - planted;
            PrivilegeTools.SetSeeds(planter, seedId, remaining);
            string seedName = DisplayNameOf(seedId);

            if (planted > 0)
            {
                location.playSound("hoeHit", who.Tile);
                Feedback.Show($"Planted {planted} {seedName}. {remaining} left in the planter.");
            }
            else
            {
                Feedback.Show($"Nothing planted: no empty tilled soil in the area accepts {seedName}.");
            }
            return true;
        }

        /// <summary>
        /// Handles the Load key. Holding seeds loads that whole stack into a planter. Holding the planter unloads it.
        /// </summary>
        public void HandleLoadSeedsKey(Farmer who)
        {
            Item held = who.CurrentItem;

            if (PrivilegeTools.IsPlanter(held))
            {
                Unload(who, held);
                return;
            }

            if (held is StardewValley.Object seeds && seeds.Category == StardewValley.Object.SeedsCategory)
            {
                Load(who, seeds);
                return;
            }

            Feedback.Show($"Hold seeds to load them into your planter. Hold the planter to unload it ({config.LoadSeedsKey}).");
        }

        private void Load(Farmer who, StardewValley.Object seeds)
        {
            Item planter = FindPlanter(who);
            if (planter == null)
            {
                Feedback.Show("You need a Privilege Planter in your inventory.");
                return;
            }

            string seedId = seeds.QualifiedItemId;
            string storedId = PrivilegeTools.GetSeedId(planter);
            int storedCount = PrivilegeTools.GetSeedCount(planter);
            if (storedCount > 0 && storedId != seedId)
            {
                Feedback.Show($"The planter already holds {DisplayNameOf(storedId)}. Hold the planter and press {config.LoadSeedsKey} to unload first.");
                return;
            }

            int amount = seeds.Stack;
            PrivilegeTools.SetSeeds(planter, seedId, storedCount + amount);
            who.removeItemFromInventory(seeds);
            Feedback.Show($"Loaded {amount} {DisplayNameOf(seedId)}. The planter holds {storedCount + amount}.");
        }

        private void Unload(Farmer who, Item planter)
        {
            string seedId = PrivilegeTools.GetSeedId(planter);
            int remaining = PrivilegeTools.GetSeedCount(planter);
            if (seedId == null || remaining <= 0)
            {
                Feedback.Show("The planter is empty.");
                return;
            }

            int requested = remaining;
            while (remaining > 0)
            {
                int chunk = Math.Min(remaining, MaxStackSize);
                Item leftover = who.addItemToInventory(ItemRegistry.Create(seedId, chunk));
                int leftoverCount = leftover?.Stack ?? 0;
                remaining -= chunk - leftoverCount;
                if (leftover != null) break;
            }

            PrivilegeTools.SetSeeds(planter, seedId, remaining);
            if (remaining == 0)
                Feedback.Show($"Returned {requested} {DisplayNameOf(seedId)} to your inventory.");
            else
                Feedback.Show($"Inventory full. {remaining} {DisplayNameOf(seedId)} stay in the planter.");
        }

        /// <summary>The planter in hand if one is held, otherwise the first planter in the inventory.</summary>
        private static Item FindPlanter(Farmer who)
        {
            if (PrivilegeTools.IsPlanter(who.CurrentTool)) return who.CurrentTool;
            return who.Items.FirstOrDefault(PrivilegeTools.IsPlanter);
        }

        private static string DisplayNameOf(string qualifiedId) => PrivilegeTools.SeedName(qualifiedId);
    }
}
