using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// The Privilege Axe. One use chops every tree, breaks every stump and log, and clears every twig and weed in the
    /// area. Fruit trees, crops, soil, buildings, and crafted objects are never touched. Outside the farm and
    /// greenhouse it acts as a normal axe.
    /// </summary>
    public sealed class WideAxeService
    {
        private readonly ModConfig config;

        public WideAxeService(ModConfig config)
        {
            this.config = config;
        }

        /// <summary>Runs one activation. Returns true when handled, which means the normal axe action must not run.</summary>
        public bool Use(GameLocation location, Farmer who, Axe axe, int power)
        {
            if (!ToolFootprint.IsAllowedLocation(location, config)) return false;

            // Same energy formula as Axe.DoFunction, charged once per activation.
            if (!axe.isEfficient.Value)
                who.Stamina -= (2 * power) - who.ForagingLevel * 0.1f;
            axe.lastUser = who;

            // Axe.DoFunction adds the axe's extra power while it works and takes it away afterwards.
            int extraPower = axe.additionalPower.Value;
            axe.UpgradeLevel += extraPower;
            try
            {
                bool struck = false;
                foreach (Vector2 tile in ToolFootprint.Compute(who, config.ToolWidth, config.ToolDepth))
                {
                    if (!ToolFootprint.CanChop(location, tile)) continue;
                    struck = true;
                    ChopTile(location, axe, tile);
                }

                if (struck) location.playSound("axchop", who.Tile);
            }
            finally
            {
                axe.UpgradeLevel -= extraPower;
            }

            return true;
        }

        private static void ChopTile(GameLocation location, Axe axe, Vector2 tile)
        {
            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature) && feature is Tree tree)
            {
                if (tree.performToolAction(axe, 0, tile)) location.terrainFeatures.Remove(tile);
                return;
            }

            if (location.objects.TryGetValue(tile, out StardewValley.Object item) && (item.IsTwig() || item.IsWeeds()))
            {
                if (item.performToolAction(axe))
                {
                    item.performRemoveAction();
                    location.objects.Remove(tile);
                }
                return;
            }

            // Only stumps and logs (resource clumps) reach this point. The game's own tool action breaks them.
            location.performToolAction(axe, (int)tile.X, (int)tile.Y);
        }
    }
}
