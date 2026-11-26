using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.TerrainFeatures;
using PelicanTownPrivilege.Configuration;

namespace PelicanTownPrivilege.Tools
{
    /// <summary>Geometry and target rules for the wide area used by the Privilege Hoe and Planter.</summary>
    public static class ToolFootprint
    {
        public const int TileSize = 64;

        /// <summary>Radius of the privilege Iridium Sprinkler's area: 4 tiles each way is a 9×9 square with the sprinkler in the middle.</summary>
        public const int SprinklerRadius = 4;

        /// <summary>
        /// The area starts one tile in front of the player and extends <paramref name="depth"/> tiles forward.
        /// Across, it is <paramref name="width"/> tiles wide and centred on the player. An even width cannot be
        /// centred exactly, so it reaches one tile further to the right (or down) than to the left (or up).
        /// The area rotates with facing.
        /// </summary>
        public static List<Vector2> Compute(Farmer who, int width, int depth)
        {
            var tiles = new List<Vector2>(width * depth);
            int left = (width - 1) / 2;
            int right = width / 2;
            Vector2 origin = who.Tile;

            for (int step = 1; step <= depth; step++)
            {
                switch (who.FacingDirection)
                {
                    case 0: // up
                        for (int dx = -left; dx <= right; dx++) tiles.Add(new Vector2(origin.X + dx, origin.Y - step));
                        break;
                    case 1: // right
                        for (int dy = -left; dy <= right; dy++) tiles.Add(new Vector2(origin.X + step, origin.Y + dy));
                        break;
                    case 2: // down
                        for (int dx = -left; dx <= right; dx++) tiles.Add(new Vector2(origin.X + dx, origin.Y + step));
                        break;
                    default: // left
                        for (int dy = -left; dy <= right; dy++) tiles.Add(new Vector2(origin.X - step, origin.Y + dy));
                        break;
                }
            }

            return tiles;
        }

        public static bool IsAllowedLocation(GameLocation location, ModConfig config)
        {
            return location != null && (config.AllowWideToolsOutsideFarmAndGreenhouse || location.IsFarm || location.IsGreenhouse);
        }

        /// <summary>
        /// True when the wide hoe may dig this tile: on the map, diggable, empty or holding grass or weeds (the hoe
        /// clears those first), with no other object, building, or character on it. Crops, trees, paths, and existing
        /// soil are never touched.
        /// </summary>
        public static bool CanTill(GameLocation location, Vector2 tile)
        {
            if (!location.isTileOnMap(tile)) return false;
            if (location.objects.TryGetValue(tile, out StardewValley.Object item) && !item.IsWeeds()) return false;
            if (location.buildings.Any(building => building.occupiesTile(tile))) return false;
            if (location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") == null) return false;
            if (location.isCharacterAtTile(tile) != null) return false;

            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature))
                return feature is Grass;

            return true;
        }

        /// <summary>
        /// True when the tile is empty tilled soil that accepts this seed. The seed must be the plain item ID the game
        /// uses (see PrivilegeTools.PlantingId). Season and crop rules apply.
        /// </summary>
        public static bool TryGetPlantableSoil(GameLocation location, Vector2 tile, string seedId, out HoeDirt soil)
        {
            return PlantBlocker(location, tile, seedId, out soil) == null;
        }

        /// <summary>Why the planter cannot plant on this tile, or null when it can. Used by the planter's debug log.</summary>
        public static string PlantBlocker(GameLocation location, Vector2 tile, string seedId, out HoeDirt soil)
        {
            soil = null;
            if (string.IsNullOrEmpty(seedId)) return "the seed is unknown to the game";
            if (!location.isTileOnMap(tile)) return "off the map";
            if (location.objects.ContainsKey(tile)) return "an object is on the tile";
            if (location.buildings.Any(building => building.occupiesTile(tile))) return "a building is on the tile";
            if (!location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature)) return "no tilled soil";

            soil = feature as HoeDirt;
            if (soil == null) return $"not tilled soil ({feature.GetType().Name})";
            if (soil.crop != null) return "a crop is already planted";
            if (!soil.canPlantThisSeedHere(seedId)) return "the game refuses this seed on this soil (season or rules)";
            return null;
        }

        /// <summary>True when the scythe acts on this tile: grass, a weed, or a crop that the scythe harvests.</summary>
        public static bool CanCut(GameLocation location, Vector2 tile)
        {
            if (!location.isTileOnMap(tile)) return false;

            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature))
                return feature is Grass || (feature is HoeDirt soil && soil.crop != null && soil.crop.GetHarvestMethod() == HarvestMethod.Scythe);

            return location.objects.TryGetValue(tile, out StardewValley.Object item) && item.IsWeeds();
        }

        /// <summary>True when the pickaxe acts on this tile: a stone, an ore node, or a boulder. Nothing else is touched.</summary>
        public static bool CanBreak(GameLocation location, Vector2 tile)
        {
            if (!location.isTileOnMap(tile)) return false;

            return location.objects.TryGetValue(tile, out StardewValley.Object item) && (item.IsBreakableStone() || item.Name.Contains("Boulder"));
        }

        /// <summary>True when the axe acts on this tile: a tree (fruit trees excluded), a twig, a weed, or a stump or log.</summary>
        public static bool CanChop(GameLocation location, Vector2 tile)
        {
            if (!location.isTileOnMap(tile)) return false;
            if (location.buildings.Any(building => building.occupiesTile(tile))) return false;

            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature) && feature is Tree)
                return true;

            if (location.objects.TryGetValue(tile, out StardewValley.Object item) && (item.IsTwig() || item.IsWeeds()))
                return true;

            int x = (int)tile.X * TileSize;
            int y = (int)tile.Y * TileSize;
            return location.resourceClumps.Any(clump => clump.getBoundingBox().Contains(x, y));
        }
    }
}
