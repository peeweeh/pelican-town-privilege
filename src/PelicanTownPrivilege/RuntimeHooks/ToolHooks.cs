using System;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.RuntimeHooks
{
    /// <summary>
    /// Makes the privilege tools work. Every hook first checks the mod's own tags, so all other tools keep their
    /// vanilla behaviour.
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

    [HarmonyPatch(typeof(Pickaxe), nameof(Pickaxe.DoFunction))]
    public static class PickaxeDoFunctionPatch
    {
        public static bool Prefix(Pickaxe __instance, GameLocation location, Farmer who)
        {
            try
            {
                if (HookServices.Config.EnableWidePickaxe && PrivilegeTools.IsWidePickaxe(__instance))
                    return !HookServices.WidePickaxe.Use(location, who, __instance);
            }
            catch (Exception ex)
            {
                HookServices.Diagnostics.Error("The privilege pickaxe failed, so the normal pickaxe action ran instead", ex);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Axe), nameof(Axe.DoFunction))]
    public static class AxeDoFunctionPatch
    {
        public static bool Prefix(Axe __instance, GameLocation location, int power, Farmer who)
        {
            try
            {
                if (HookServices.Config.EnableWideAxe && PrivilegeTools.IsWideAxe(__instance))
                    return !HookServices.WideAxe.Use(location, who, __instance, power);
            }
            catch (Exception ex)
            {
                HookServices.Diagnostics.Error("The privilege axe failed, so the normal axe action ran instead", ex);
            }

            return true;
        }
    }

    /// <summary>The scythe keeps its normal swing and combat. The area pass runs after the swing.</summary>
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.DoDamage))]
    public static class MeleeDoDamagePatch
    {
        public static void Postfix(MeleeWeapon __instance, GameLocation location, Farmer who)
        {
            try
            {
                if (HookServices.Config.EnableWideScythe && PrivilegeTools.IsWideScythe(__instance) && who.IsLocalPlayer)
                    HookServices.WideScythe.Use(location, who, __instance);
            }
            catch (Exception ex)
            {
                HookServices.Diagnostics.Error("The privilege scythe area failed, so only the normal swing ran", ex);
            }
        }
    }

    /// <summary>The privilege Iridium Sprinkler waters a 9×9 square. The game reads this radius when it waters and when it draws the area.</summary>
    [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.GetModifiedRadiusForSprinkler))]
    public static class SprinklerRadiusPatch
    {
        public static void Postfix(StardewValley.Object __instance, ref int __result)
        {
            try
            {
                if (HookServices.Config.EnableWideSprinkler && PrivilegeTools.IsWideSprinkler(__instance))
                    __result = ToolFootprint.SprinklerRadius;
            }
            catch (Exception ex)
            {
                HookServices.Diagnostics.Error("The privilege sprinkler radius failed, so the normal radius ran", ex);
            }
        }
    }

    [HarmonyPatch(typeof(Tool), nameof(Tool.DisplayName), MethodType.Getter)]
    public static class ToolDisplayNamePatch
    {
        public static void Postfix(Tool __instance, ref string __result)
        {
            string name = PrivilegeTools.NameOf(__instance);
            if (name != null) __result = name;
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
