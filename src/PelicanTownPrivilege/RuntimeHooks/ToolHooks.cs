using System;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.RuntimeHooks
{
    /// <summary>
    /// Makes the Privilege Hoe and Privilege Planter work. Every hook first checks the mod's own tags, so all
    /// other hoes and tools keep their vanilla behaviour.
    /// </summary>
    [HarmonyPatch(typeof(Hoe), nameof(Hoe.DoFunction))]
    public static class HoeDoFunctionPatch
    {
        public static bool Prefix(Hoe __instance, GameLocation location, int power, Farmer who)
        {
            try
            {
                if (HookServices.Config.EnableWideHoe && PrivilegeTools.IsWideHoe(__instance))
                    return !HookServices.WideHoe.Use(location, who, __instance, power);

                if (HookServices.Config.EnableWidePlanter && PrivilegeTools.IsPlanter(__instance))
                    return !HookServices.Planter.Plant(location, who, __instance);
            }
            catch (Exception ex)
            {
                HookServices.Diagnostics.Error("A privilege tool failed, so the normal hoe action ran instead", ex);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Tool), nameof(Tool.DisplayName), MethodType.Getter)]
    public static class ToolDisplayNamePatch
    {
        public static void Postfix(Tool __instance, ref string __result)
        {
            if (PrivilegeTools.IsWideHoe(__instance)) __result = PrivilegeTools.WideHoeName;
            else if (PrivilegeTools.IsPlanter(__instance)) __result = PrivilegeTools.PlanterName;
        }
    }

    [HarmonyPatch(typeof(Tool), nameof(Tool.getDescription))]
    public static class ToolDescriptionPatch
    {
        public static void Postfix(Tool __instance, ref string __result)
        {
            if (!PrivilegeTools.IsPlanter(__instance)) return;

            int count = PrivilegeTools.GetSeedCount(__instance);
            __result += count > 0
                ? $"\nLoaded: {count} x {PrivilegeTools.SeedName(PrivilegeTools.GetSeedId(__instance))}."
                : $"\nNo seeds loaded. Hold seeds and press {HookServices.Config.LoadSeedsKey}.";
        }
    }

    /// <summary>A planter holding seeds cannot be thrown away, so the seeds are never lost.</summary>
    [HarmonyPatch(typeof(Item), nameof(Item.canBeTrashed))]
    public static class ItemCanBeTrashedPatch
    {
        public static void Postfix(Item __instance, ref bool __result)
        {
            if (__result && PrivilegeTools.IsPlanter(__instance) && PrivilegeTools.GetSeedCount(__instance) > 0)
                __result = false;
        }
    }
}
