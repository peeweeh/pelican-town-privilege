using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// The Privilege Scythe. After each normal swing, cuts grass, clears weeds, and harvests ripe scythe crops across
    /// the whole area. The swing and its combat are unchanged. Soil is never removed.
    /// </summary>
    public sealed class WideScytheService
    {
        private readonly ModConfig config;

        public WideScytheService(ModConfig config)
        {
            this.config = config;
        }

        /// <summary>Runs the area pass for one swing. Called after the normal swing has run.</summary>
        public void Use(GameLocation location, Farmer who, MeleeWeapon scythe)
        {
            if (!ToolFootprint.IsAllowedLocation(location, config)) return;

            foreach (Vector2 tile in ToolFootprint.Compute(who, config.ToolWidth, config.ToolDepth))
            {
                if (!ToolFootprint.CanCut(location, tile)) continue;

                if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature))
                {
                    // Soil stays in place. Its return value is ignored, so only the crop is taken.
                    if (feature is Grass && feature.performToolAction(scythe, 0, tile))
                        location.terrainFeatures.Remove(tile);
                    else if (feature is HoeDirt)
                        feature.performToolAction(scythe, 0, tile);
                    continue;
                }

                if (location.objects.TryGetValue(tile, out StardewValley.Object weed) && weed.performToolAction(scythe))
                    location.objects.Remove(tile);
            }
        }
    }
}
