using StardewValley;
using StardewValley.Objects;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// Overnight incubation. Coop incubators and Ostrich Incubators are different systems, so each has its own code path.
    /// Only eggs already sitting in an incubator are finished, and only when the day ends.
    /// </summary>
    public sealed class IncubationService
    {
        public const string OstrichIncubatorName = "Ostrich Incubator";

        private readonly ModConfig config;
        private readonly Diagnostics diagnostics;

        public IncubationService(ModConfig config, Diagnostics diagnostics)
        {
            this.config = config;
            this.diagnostics = diagnostics;
        }

        public void OnDayEnding()
        {
            if (!config.EnableOvernightIncubation) return;

            int coop = CompleteCoopIncubators();
            int ostrich = CompleteOstrichIncubators();
            if (coop + ostrich > 0)
                diagnostics.Info($"Overnight incubation: finished {coop} coop egg(s) and {ostrich} Ostrich Incubator egg(s).");
        }

        /// <summary>Coop incubators are machines inside AnimalHouse interiors.</summary>
        private static int CompleteCoopIncubators()
        {
            int count = 0;
            foreach (GameLocation location in LocationScan.AllLocations())
            {
                if (location is not AnimalHouse house) continue;

                foreach (StardewValley.Object machine in house.objects.Values)
                {
                    if (!machine.bigCraftable.Value) continue;
                    var machineData = machine.GetMachineData();
                    if (machineData == null || !machineData.IsIncubator) continue;
                    if (machine.heldObject.Value == null || machine.MinutesUntilReady <= 0) continue;

                    machine.MinutesUntilReady = 0;
                    count++;
                }
            }
            return count;
        }

        /// <summary>Ostrich Incubators sit on ordinary locations, outside coops.</summary>
        private static int CompleteOstrichIncubators()
        {
            int count = 0;
            foreach (GameLocation location in LocationScan.AllLocations())
            {
                if (location is AnimalHouse) continue;

                foreach (StardewValley.Object machine in location.objects.Values)
                {
                    if (machine.Name != OstrichIncubatorName) continue;
                    if (machine.heldObject.Value == null || machine.MinutesUntilReady <= 0) continue;

                    machine.MinutesUntilReady = 0;
                    count++;
                }
            }
            return count;
        }
    }
}
