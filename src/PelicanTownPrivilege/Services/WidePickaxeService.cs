using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// The Privilege Pickaxe. One use breaks every stone, ore node, and boulder in the area. Soil, crops, trees,
    /// and crafted objects are never touched. Outside the farm and greenhouse it acts as a normal pickaxe.
    /// </summary>
    public sealed class WidePickaxeService
    {
        /// <summary>Hits a boulder takes before it breaks.</summary>
        private const int BoulderHits = 4;

        /// <summary>The upgrade level the normal pickaxe needs to break boulders (Iridium).</summary>
        private const int BoulderUpgradeLevel = 4;

        private readonly ModConfig config;

        /// <summary>Boulder hits carried into the next use, for boulders that are still in the area.</summary>
        private Dictionary<Vector2, int> boulderHits = new Dictionary<Vector2, int>();

        public WidePickaxeService(ModConfig config)
        {
            this.config = config;
        }

        /// <summary>Runs one activation. Returns true when handled, which means the normal pickaxe action must not run.</summary>
        public bool Use(GameLocation location, Farmer who, Pickaxe pickaxe)
        {
            if (!ToolFootprint.IsAllowedLocation(location, config)) return false;

            // Same energy formula as Pickaxe.DoFunction, charged once per activation.
            int toolPower = who.toolPower.Value;
            if (!pickaxe.isEfficient.Value)
                who.Stamina -= (2 * (toolPower + 1)) - who.MiningLevel * 0.1f;
            pickaxe.lastUser = who;

            int stoneHitPower = Math.Max(1, pickaxe.UpgradeLevel + 1) + pickaxe.additionalPower.Value;
            bool canBreakBoulders = pickaxe.UpgradeLevel >= BoulderUpgradeLevel;
            var nextBoulderHits = new Dictionary<Vector2, int>();
            bool struck = false;
            bool broke = false;

            foreach (Vector2 tile in ToolFootprint.Compute(who, config.ToolWidth, config.ToolDepth))
            {
                if (!ToolFootprint.CanBreak(location, tile) || !location.objects.TryGetValue(tile, out StardewValley.Object item))
                    continue;

                if (item.IsBreakableStone())
                {
                    struck = true;
                    if (item.MinutesUntilReady > 0)
                    {
                        // Ore nodes take several hits, the same as with the normal pickaxe.
                        item.MinutesUntilReady -= stoneHitPower;
                        item.shakeTimer = 200;
                        if (item.MinutesUntilReady > 0)
                        {
                            Game1.createRadialDebris(location, 14, (int)tile.X, (int)tile.Y, Game1.random.Next(2, 5), resource: false);
                            continue;
                        }
                    }

                    BreakStone(location, who, tile, item);
                    broke = true;
                    continue;
                }

                if (!canBreakBoulders) continue;

                struck = true;
                int hits = (boulderHits.TryGetValue(tile, out int previous) ? previous : 0) + toolPower + 1;
                if (hits >= BoulderHits)
                {
                    location.removeObject(tile, showDestroyedObject: false);
                    Game1.createRadialDebris(location, 14, (int)tile.X, (int)tile.Y, Game1.random.Next(2, 5), resource: false);
                    broke = true;
                }
                else
                {
                    nextBoulderHits[tile] = hits;
                    item.shakeTimer = 190;
                }
            }

            boulderHits = nextBoulderHits;
            if (struck) location.playSound("hammer", who.Tile);
            if (broke) location.playSound("stoneCrack", who.Tile);
            return true;
        }

        private static void BreakStone(GameLocation location, Farmer who, Vector2 tile, StardewValley.Object stone)
        {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            Game1.createRadialDebris(location, 14, x, y, Game1.random.Next(2, 5), resource: false);
            location.OnStoneDestroyed(stone.ItemId, x, y, who);
            if (who.stats.Get("Book_Diamonds") != 0 && Game1.random.NextDouble() < 0.0066)
                Game1.createObjectDebris("(O)72", x, y, who.UniqueMultiplayerID, location);
            stone.performRemoveAction();
            location.objects.Remove(tile);
            Game1.stats.RocksCrushed++;
        }
    }
}
