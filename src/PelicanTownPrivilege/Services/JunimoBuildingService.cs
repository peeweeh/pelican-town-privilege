using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Buildings;
using PelicanTownPrivilege.Configuration;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// Robin's Junimo Hut: listed in Robin's menu from the start, free by default, with no materials, built
    /// instantly, and without the Wizard requirement. Only this one building entry changes, so no other magical
    /// buildings unlock.
    /// </summary>
    public sealed class JunimoBuildingService
    {
        public const string JunimoHutId = "Junimo Hut";

        private readonly ModConfig config;
        private readonly Diagnostics diagnostics;

        public JunimoBuildingService(ModConfig config, Diagnostics diagnostics)
        {
            this.config = config;
            this.diagnostics = diagnostics;
        }

        public void EditBuildings(IAssetData asset)
        {
            if (!config.EnableEarlyJunimoHuts) return;

            IDictionary<string, BuildingData> buildings = asset.AsDictionary<string, BuildingData>().Data;
            if (!buildings.TryGetValue(JunimoHutId, out BuildingData hut) || hut == null)
            {
                diagnostics.Warn($"Junimo Hut entry ('{JunimoHutId}') was not found in Data/Buildings. Early Junimo Huts are not active.");
                return;
            }

            hut.BuildCondition = null;
            // Robin's menu lists only buildings whose Builder is Robin.
            hut.Builder = Game1.builder_robin;
            hut.BuildCost = config.JunimoHutPrice;
            if (!config.JunimoHutRequiresMaterials)
                hut.BuildMaterials = new List<BuildingMaterial>();
            if (config.InstantJunimoConstruction)
                hut.BuildDays = 0;

            diagnostics.Info($"Junimo Hut: {hut.BuildCost}g, materials {(config.JunimoHutRequiresMaterials ? "required" : "none")}, " +
                             $"construction {(config.InstantJunimoConstruction ? "instant" : "normal")}, listed in Robin's menu, no Wizard requirement.");
        }
    }
}
