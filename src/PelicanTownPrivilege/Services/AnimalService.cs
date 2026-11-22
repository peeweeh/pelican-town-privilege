using StardewModdingAPI;
using StardewValley.GameData.FarmAnimals;
using System.Collections.Generic;
using PelicanTownPrivilege.Configuration;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// Early Blue Chickens (Marnie's shop) and daily Dinosaur eggs. Only these two animal entries change.
    /// Shane's friendship events are not involved; the purchase condition is simply removed.
    /// </summary>
    public sealed class AnimalService
    {
        public const string BlueChickenId = "Blue Chicken";
        public const string DinosaurId = "Dinosaur";

        private readonly ModConfig config;
        private readonly Diagnostics diagnostics;

        public AnimalService(ModConfig config, Diagnostics diagnostics)
        {
            this.config = config;
            this.diagnostics = diagnostics;
        }

        public void EditFarmAnimals(IAssetData asset)
        {
            IDictionary<string, FarmAnimalData> animals = asset.AsDictionary<string, FarmAnimalData>().Data;

            if (config.EnableEarlyBlueChickens)
            {
                if (animals.TryGetValue(BlueChickenId, out FarmAnimalData blueChicken) && blueChicken != null)
                {
                    blueChicken.PurchasePrice = config.BlueChickenPrice;
                    blueChicken.UnlockCondition = "TRUE";
                    diagnostics.Info($"Blue Chicken: {config.BlueChickenPrice}g, available immediately. A coop is still required.");
                }
                else
                {
                    diagnostics.Warn($"'{BlueChickenId}' was not found in Data/FarmAnimals. Early Blue Chickens are not active.");
                }
            }

            if (config.EnableDailyDinosaurEggs)
            {
                if (animals.TryGetValue(DinosaurId, out FarmAnimalData dinosaur) && dinosaur != null)
                {
                    dinosaur.DaysToProduce = 1;
                    diagnostics.Info("Dinosaur: produces once per day when fed and cared for.");
                }
                else
                {
                    diagnostics.Warn($"'{DinosaurId}' was not found in Data/FarmAnimals. Daily Dinosaur eggs are not active.");
                }
            }
        }
    }
}
