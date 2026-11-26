using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// The Privilege Hoe. One activation tills the whole area in front of the player and costs the same energy
    /// as one normal hoe activation.
    /// </summary>
    public sealed class WideHoeService
    {
        private readonly ModConfig config;

        public WideHoeService(ModConfig config)
        {
            this.config = config;
        }

        /// <summary>Runs one activation. Returns true when handled, which means the normal hoe action must not run.</summary>
        public bool Use(GameLocation location, Farmer who, Hoe hoe, int power)
        {
            if (!ToolFootprint.IsAllowedLocation(location, config))
            {
                Feedback.Show("The Privilege Hoe works on the farm and in the greenhouse.");
                return true;
            }

            // Same energy formula as Hoe.DoFunction, charged once per activation.
            if (!hoe.isEfficient.Value)
                who.Stamina -= (2 * power) - who.FarmingLevel * 0.1f;
            hoe.lastUser = who;

            int tilled = 0;
            foreach (Vector2 tile in ToolFootprint.Compute(who, config.ToolWidth, config.ToolDepth))
            {
                if (!ToolFootprint.CanTill(location, tile)) continue;

                // The game's grass ignores hoes, so this clears a grass tuft and tills the tile in the same use.
                if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature) && feature is Grass)
                    location.terrainFeatures.Remove(tile);

                // Weeds are removed the same way, so a weedy tile is tilled in this use too. They drop nothing.
                if (location.objects.TryGetValue(tile, out StardewValley.Object weed) && weed.IsWeeds())
                    location.objects.Remove(tile);

                if (location.makeHoeDirt(tile))
                {
                    tilled++;
                    location.checkForBuriedItem((int)tile.X, (int)tile.Y, false, false, who);
                    Game1.stats.DirtHoed++;
                }
            }

            if (tilled > 0)
                location.playSound("hoeHit", who.Tile);
            return true;
        }
    }
}
