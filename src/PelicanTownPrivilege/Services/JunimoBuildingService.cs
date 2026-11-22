using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley.GameData.Buildings;
using PelicanTownPrivilege.Configuration;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// Robin's Junimo Hut: available from the start, 1g, no materials, built instantly, and without the Wizard
    /// requirement. Only this one building entry changes, so no other magical buildings unlock.
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
            hut.BuildCost = config.JunimoHutPrice;
            if (!config.JunimoHutRequiresMaterials)
                hut.BuildMaterials = new List<BuildingMaterial>();
            if (config.InstantJunimoConstruction)
                hut.BuildDays = 0;

            diagnostics.Info($"Junimo Hut: {hut.BuildCost}g, materials {(config.JunimoHutRequiresMaterials ? "required" : "none")}, " +
                             $"construction {(config.InstantJunimoConstruction ? "instant" : "normal")}, no Wizard requirement.");
        }
    }
}
