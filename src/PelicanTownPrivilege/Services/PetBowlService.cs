using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Buildings;
using PelicanTownPrivilege.Configuration;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// The dog's Pet Bowl: listed in Robin's menu from the start, free by default, with no materials and instant
    /// construction. Water the bowl each day with a watering can for +6 friendship. A pet without a bowl loses
    /// friendship every day.
    /// </summary>
    public sealed class PetBowlService
    {
        public const string PetBowlId = "Pet Bowl";

        private readonly ModConfig config;
        private readonly Diagnostics diagnostics;

        public PetBowlService(ModConfig config, Diagnostics diagnostics)
        {
            this.config = config;
            this.diagnostics = diagnostics;
        }

        public void EditBuildings(IAssetData asset)
        {
            if (!config.EnablePetBowl) return;

            IDictionary<string, BuildingData> buildings = asset.AsDictionary<string, BuildingData>().Data;
            if (!buildings.TryGetValue(PetBowlId, out BuildingData bowl) || bowl == null)
            {
                diagnostics.Warn($"Pet Bowl entry ('{PetBowlId}') was not found in Data/Buildings. The free Pet Bowl is not active.");
                return;
            }

            bowl.BuildCondition = null;
            // Robin's menu lists only buildings whose Builder is Robin.
            bowl.Builder = Game1.builder_robin;
            bowl.BuildCost = config.PetBowlPrice;
            bowl.BuildMaterials = new List<BuildingMaterial>();
            bowl.BuildDays = 0;

            diagnostics.Info($"Pet Bowl: {bowl.BuildCost}g, no materials, instant construction, listed in Robin's menu.");
        }
    }
}
