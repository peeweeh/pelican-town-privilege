using System.Collections.Generic;
using StardewValley;

namespace PelicanTownPrivilege.Tools
{
    /// <summary>Lists every location the player can reach, including building interiors such as coops.</summary>
    public static class LocationScan
    {
        public static List<GameLocation> AllLocations()
        {
            var result = new List<GameLocation>();
            foreach (GameLocation location in Game1.locations)
            {
                result.Add(location);
                foreach (StardewValley.Buildings.Building building in location.buildings)
                {
                    GameLocation indoors = building.indoors.Value;
                    if (indoors != null) result.Add(indoors);
                }
            }
            return result;
        }
    }

    /// <summary>Short on-screen messages for privilege tool actions.</summary>
    public static class Feedback
    {
        public static void Show(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message));
        }
    }
}
